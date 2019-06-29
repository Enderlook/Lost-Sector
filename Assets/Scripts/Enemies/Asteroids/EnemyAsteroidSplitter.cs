using System.Linq;
using UnityEngine;

public class EnemyAsteroidSplitter : EnemyAsteroid
{
    [Header("Configuration")]
    [Tooltip("Configuration of gameObjects to spawn on death.")]
    public SpawneableGameObjects spawnsOnDeath;

    protected override void Die()
    {
        spawnsOnDeath.SpawnPrefabs(Instantiate);
        base.Die();
    }
}

[System.Serializable]
public class SpawneableGameObjects
{
    [Tooltip("Spawning points for prefabs.")]
    public Transform[] spawningPoints;
    [Tooltip("Prefabs types able spawn.")]
    public SpawneableGameObject[] prefabsToSpawn;
    [Tooltip("Amount of prefabs to spawn.")]
    public FloatRangeTwo amountToSpawn;
    [Tooltip("If false, it will be spawn a number of game objects equal to Amount To Spawn value, randomly distributed by chance. On true, Amount To Spawn will be ignored and weight will be used to determine the amount of each prefab (instead of their chance).")]
    public bool notRandom;

    /// <summary>
    /// Get a random prefab from the <see cref="prefabsToSpawn"/>.
    /// </summary>
    /// <returns>Random prefab to spawn.</returns>
    private GameObject GetRandomPrefab()
    {
        float totalWeight = prefabsToSpawn.Sum(prefabs => prefabs.weight);
        float chosenWeight = Random.value * totalWeight;

        float currentWeight = 0;
        foreach (SpawneableGameObject prefab in prefabsToSpawn)
        {
            currentWeight += prefab.weight;
            if (currentWeight >= chosenWeight)
            {
                return prefab.prefab;
            }
        }
        throw new System.Exception("This shouldn't be happening!!!");
    }

    /// <summary>
    /// Spawn the defined game objects.
    /// </summary>
    /// <param name="Instantiate">Instantiate UnityEngine method.</param>
    public void SpawnPrefabs(System.Func<GameObject, Transform, GameObject> Instantiate)
    {
        void Spawn(GameObject gameObjectToSpawn)
        {
            GameObject spawned = Instantiate(gameObjectToSpawn, Global.enemiesParent);
            Transform spawningTransform = spawningPoints[Random.Range(0, spawningPoints.Length - 1)];
            RigidbodyHelper spawnedRigidbodyHelper = spawned.GetComponent<Enemy>().rigidbodyHelper; // Or spawned.GetComponentInChildren<RigidbodyHelper>(); ?
            spawnedRigidbodyHelper.transform.position = spawningTransform.position;
            spawnedRigidbodyHelper.transform.transform.rotation = spawningTransform.rotation;
            // To make them a bit slower
            spawned.GetComponent<Enemy>().impulse *= 0.75f;
        }

        if (notRandom)
        {
            foreach (SpawneableGameObject spawneableGameObject in prefabsToSpawn)
            {
                for (int i = 0; i < spawneableGameObject.weight; i++)
                {
                    Spawn(spawneableGameObject.prefab);
                }
            }
        }
        else
        {
            int total = (int)amountToSpawn;
            for (int i = 0; i < total; i++)
            {
                Spawn(GetRandomPrefab());
            }
        }
    }
}

/// <summary>
/// Stores an spawneable game object and its weight.
/// </summary>
[System.Serializable]
public struct SpawneableGameObject
{
    [Tooltip("GameObject prefab.")]
    public GameObject prefab;

    [Tooltip("Weight rarity.")]
    public float weight;
}