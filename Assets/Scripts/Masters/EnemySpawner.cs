using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Tooltip("Transforms positions used to spawn enemies.")]
    public TransformRange[] spawnPoints;

    [Tooltip("Enemies spawned.")]
    public Enemies enemies;

    [Tooltip("Difficulty.")]
    public float difficulty = 1;

    [Tooltip("Increase of linear difficulty over time.")]
    [Range(0.1f, 10)]
    public float difficultyLinearIncrease = 0.1f;

    [Tooltip("Increase of geometrical difficulty over time.")]
    [Range(1, 10)]
    public float difficultyGeometricalIncrease = 1;

    [Tooltip("Interval of seconds per increase of difficulty.")]
    [Range(1, 600)]
    public float difficultyIncreaseInterval = 5;

    [Tooltip("Health Pack")]
    public GameObject healthPack;
    [Tooltip("Time between spawn attempts of Health Pack")]
    public float healthPackSpawnTime;
    private float healthPackSpawnCharge = 0;
    [Range(0, 1)]
    [Tooltip("Health Pack spawn probability per spawn attempt. From 0 to 1.")]
    public float healthPackSpawnChance;

    private Dictionary<GameObject, List<GameObject>> pool = new Dictionary<GameObject, List<GameObject>>();
    private bool requireWeightsUpdate = true;
    private Coroutine spawnWaveCoroutine;

    private void Start()
    {
        InvokeRepeating("DifficultyIncrease", difficultyIncreaseInterval, difficultyIncreaseInterval);
        StartCoroutine(SpawnWave());
    }

    private void Update()
    {
        if (!Global.menu.ShouldWork)
            return;
        if (healthPackSpawnCharge >= healthPackSpawnTime)
        {
            healthPackSpawnCharge = 0;
            if (Random.Range(0f, 1f) < healthPackSpawnChance)
            {
                GameObject pack = Spawn(healthPack, Global.pickupsParent);
                pack.transform.position = (Vector3)spawnPoints[Random.Range(0, spawnPoints.Length - 1)];
            }
        }
        else
            healthPackSpawnCharge += Time.deltaTime;
    }

    /// <summary>
    /// Look for an inactive <seealso cref="GameObject"/> from <paramref name="prefab"/> to recycle. If not found, instantiate a new one.
    /// </summary>
    /// <param name="prefab">Prefab to look or instantiate.</param>
    /// <returns>Instance of the prefab, either recycled or new. It's your and its scripts responsibility to properly reset.</returns>
    /// <seealso cref="Spawn(GameObject, Transform)"/>
    public GameObject Spawn(GameObject prefab)
    {
        GameObject instance;
        if (pool.TryGetValue(prefab, out List<GameObject> instances))
        {
            List<int> toRemove = new List<int>();
            for (int i = 0; i < instances.Count; i++)
            {
                if (instances[i] == null)
                    toRemove.Add(i);
                else if (!instances[i].activeSelf)
                {
                    instance = instances[i];
                    instance.SetActive(true);
                    return instance;
                }
            }
        }
        else
            pool.Add(prefab, new List<GameObject>());

        instance = Instantiate(prefab);
        pool[prefab].Add(instance);
        return instance;
    }

    /// <summary>
    /// Look for an inactive <seealso cref="GameObject"/> from <paramref name="prefab"/> to recycle. If not found, instantiate a new one.
    /// </summary>
    /// <param name="prefab">Prefab to look or instantiate.</param>
    /// <param name="parent">Assign parent to the instance.</param>
    /// <returns>Instance of the prefab, either recycled or new. It's your and its scripts responsibility to properly reset.</returns>
    /// <seealso cref="Spawn(GameObject)"/>
    public GameObject Spawn(GameObject prefab, Transform parent)
    {
        GameObject instance = Spawn(prefab);
        instance.transform.parent = parent;
        return instance;
    }

    /// <summary>
    /// Spawns enemies on the fly.
    /// </summary>
    /// <returns>This must be executed on a <see cref="StartCoroutine"/>.</returns>
    private IEnumerator SpawnWave()
    {
        // TODO: Custom modifications per enemy.
        while (true)
        {
            if (Global.menu.ShouldWork)
            {
                if (requireWeightsUpdate)
                {
                    enemies.UpdateWeights(difficulty);
                    requireWeightsUpdate = false;
                }
                foreach (GameObject enemyPrefab in enemies.GetEnemies(difficulty))
                {
                    Vector3 position = (Vector3)spawnPoints[Random.Range(0, spawnPoints.Length - 1)];

                    GameObject enemy = Spawn(enemyPrefab, Global.enemiesParent);
                    enemy.transform.position = position;

                    yield return new WaitForSeconds(0.1f);
                }
                yield return new WaitForSeconds(3.25f / Mathf.Log10(difficulty + 1));
            }
            else
            {
                yield return new WaitForSeconds(1);
            }
        }
    }

    /// <summary>
    /// Increase difficulty of the game. This isn't executed per frame but on a Coroutine because it would produce a greater geometrical progression that we don't want.
    /// </summary>
    private void DifficultyIncrease()
    {
        if (Global.menu.ShouldWork)
        {
            difficulty *= difficultyGeometricalIncrease;
            difficulty += difficultyLinearIncrease;
            requireWeightsUpdate = true;
        }
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (difficultyGeometricalIncrease < 1)
            Debug.LogWarning($"The field {nameof(difficultyGeometricalIncrease)} can't be lower than 1.");
        if (difficulty == 0)
        {
            Debug.LogWarning($"The field {nameof(difficulty)} can't be 0.");
        }

        foreach (float field in new float[] { difficulty, difficultyLinearIncrease, difficultyIncreaseInterval })
        {
            if (field < 0)
            {
                Debug.LogWarning($"The field {nameof(field)} can't be lower than 0.");
            }
        }
    }
#endif
}

[System.Serializable]
public class EnemyPrefab
{
    [Tooltip("Enemy prefab to spawn.")]
    public GameObject prefab;
    [Tooltip("Weighted rarity.")]
    public float weight = 1;
    [Tooltip("Threat level of the enemy. Used to not spawn a few weak enemies nor a lot o strong enemies.")]
    public float threat;

    [Tooltip("Minimal difficulty in order to allow the spawn of this enemy.")]
    public float minimalDifficulty;
    [Tooltip("Multiplies the weight rarity based on difficulty above the Minimal Difficulty. The letter x will be replaced by the difference between difficulty and Minimal Difficulty.")]
    public string weightFactorFormula = "1";

    private float calculatedWeight;

    /// <summary>
    /// Return the weighted rarity of the enemy to spawn. The weight changes according to the current difficulty.
    /// </summary>
    /// <returns>Weighted rarity.</returns>
    /// <seealso cref="UpdateWeight(float)"/>
    public float Weight => calculatedWeight;

    /// <summary>
    /// Updates the weight multiplier which changes according to the current difficulty.
    /// </summary>
    /// <param name="difficulty">Difficulty used to calculate weight.</param>
    public void UpdateWeight(float difficulty)
    {
        if (difficulty < minimalDifficulty)
            calculatedWeight = 0;
        else
        {
            // TODO: This could be done safer using a custom math class...
            // https://stackoverflow.com/questions/333737/evaluating-string-342-yield-int-18
            // https://www.codeproject.com/Questions/1031807/Evaluating-an-expression-using-DataTable-Compute-m
            // TODO: https://social.msdn.microsoft.com/Forums/en-US/2ee4bbbd-e18b-49b7-a119-57ef748e4f28/how-to-convert-a-string-operation-to-a-math-operation?forum=csharpgeneral
            // And: https://rosettacode.org/wiki/Parsing/Shunting-yard_algorithm#C.23
            DataTable dataTable = new DataTable();
            calculatedWeight = float.Parse(dataTable.Compute(weightFactorFormula.Replace("x", difficulty.ToString()).Replace(",", "."), "").ToString());
        }
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (weight <= 0)
            Debug.LogWarning($"The field {nameof(weight)} can't be lower or equal than 0.");
        if (prefab == null)
            Debug.LogWarning($"The field {nameof(weight)} can't be null.");
        // TODO: How to check minimalThreat?
    }
#endif
}

[System.Serializable]
public class Enemies
{
    [Tooltip("Enemy prefabs to spawn.")]
    public EnemyPrefab[] enemyPrefabs;

    /// <summary>
    /// Get an enemy prefab to spawn and its treat.
    /// </summary>
    /// <returns>Enemy prefab to spawn and threat of the enemy.</returns>
    public System.Tuple<GameObject, float> GetEnemy(float difficulty)
    {
        float totalWeight = enemyPrefabs.Sum(enemy => enemy.Weight);
        float chosenWeight = Random.value * totalWeight;

        float currentWeight = 0;
        foreach (EnemyPrefab enemy in enemyPrefabs)
        {
            currentWeight += enemy.Weight;
            if (currentWeight >= chosenWeight)
            {
                return new System.Tuple<GameObject, float>(enemy.prefab, enemy.threat);
            }
        }
        throw new System.Exception("This shouldn't be happening!!!");
    }

    /// <summary>
    /// Returns an <see cref="IEnumerable"/> of enemies that should be spawned.
    /// </summary>
    /// <returns>Enemies that should be spawned.</returns>
    public IEnumerable GetEnemies(float difficulty)
    {
        float threat = 0;
        while (threat < 3 + (Mathf.Pow(difficulty, 0.4f) * 1.5))
        {
            System.Tuple<GameObject, float> enemy = GetEnemy(difficulty);
            threat += enemy.Item2;
            yield return enemy.Item1;
        }
    }

    /// <summary>
    /// Updates the weight multipliers of all <seealso cref="enemyPrefabs"/> which changes according to the current difficulty.
    /// </summary>
    /// <param name="difficulty">Difficulty used to calculate weight.</param>
    public void UpdateWeights(float difficulty)
    {
        foreach (EnemyPrefab enemy in enemyPrefabs)
        {
            enemy.UpdateWeight(difficulty);
        }
    }
}