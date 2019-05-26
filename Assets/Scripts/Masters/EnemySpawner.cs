using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public TransformRange[] spawnPoints;

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

    private void Start()
    {
        InvokeRepeating("DifficultyIncrease", difficultyIncreaseInterval, difficultyIncreaseInterval);
        StartCoroutine(SpawnWave());
    }
    
    IEnumerator SpawnWave()
    {
        // TODO: This should stop on player death....
        // TODO: Custom modifications per enemy.
        while (true)
        {
            foreach (GameObject enemyPrefab in enemies.GetEnemies(difficulty))
            {
                TransformRange spawnRange = spawnPoints[Random.Range(0, spawnPoints.Length - 1)];
                Vector3 position = spawnRange.getVector();

                GameObject enemy = Instantiate(enemyPrefab, Dynamic.Instance.enemiesParent);
                enemy.transform.position = position;

                //https://forum.unity.com/threads/getcomponents-possible-to-use-with-c-interfaces.60596/
                /*ISpawningSubrutine spawningSubrutine = enemy.GetComponent<ISpawningSubrutine>();
                spawningSubrutine.Spawn();*/

                yield return new WaitForSeconds(0.1f);
            }
            yield return new WaitForSeconds(2 / Mathf.Log10(difficulty + 1));
        }
    }

    private void DifficultyIncrease()
    {
        difficulty *= difficultyGeometricalIncrease;
        difficulty += difficultyLinearIncrease;
    }

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

}

[System.Serializable]
public class EnemyPrefab {
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

    /// <summary>
    /// Return the weigthed rarity of the enemy to spawn. The weight changes according to the current difficulty.
    /// </summary>
    /// <param name="difficulty">Difficulty used to calculate weight.</param>
    /// <returns>Weighted rarity.</returns>
    public float GetWeight(float difficulty)
    {
        if (difficulty < minimalDifficulty)
            return 0;
        else
        {
            // TODO: This could be done safer using a custom math class...
            // https://stackoverflow.com/questions/333737/evaluating-string-342-yield-int-18
            DataTable dataTable = new DataTable();
            float multiplier = float.Parse(dataTable.Compute(weightFactorFormula.Replace("x", difficulty.ToString()), "").ToString());
            return weight * multiplier;
        }
    }

    private void OnValidate()
    {
        if (weight <= 0)
            Debug.LogWarning($"The field {nameof(weight)} can't be lower or equal than 0.");
        if (prefab == null)
            Debug.LogWarning($"The field {nameof(weight)} can't be null.");
        // How to check minimalThreat?
    }
}

[System.Serializable] 
public class Enemies {
    [Tooltip("Enemy prefabs to spawn.")]
    public EnemyPrefab[] enemyPrefabs;

    /// <summary>
    /// Get an enemy prefab to spawn.
    /// </summary>
    /// <param name="difficulty">Current difficulty used to base the type of enemy.</param>
    /// <returns>Enemy prefab to spawn</returns>
    /*public GameObject GetEnemyPrefab(float difficulty)
    {
        float totalWeight = enemyPrefabs.Sum((enemy) => enemy.GetWeight(difficulty));
        float chosenWeight = Random.value * totalWeight;

        float currentWeight = 0;
        foreach (EnemyPrefab enemy in enemyPrefabs)
        {
            currentWeight += enemy.weight;
            if (currentWeight >= chosenWeight)
            {
                return enemy.prefab;
            }
        }
        throw new System.Exception("This shouldn't be happening!!!");
    }*/

    /// <summary>
    /// Get an enemy prefab to spawn and its treat.
    /// </summary>
    /// <param name="difficulty">Current difficulty used to base the type of enemy.</param>
    /// <returns>Enemy prefab to spawn and threat of the enemy</returns>
    private System.Tuple<GameObject, float> GetEnemy(float difficulty)
    {
        float totalWeight = enemyPrefabs.Sum((enemy) => enemy.GetWeight(difficulty));
        float chosenWeight = Random.value * totalWeight;

        float currentWeight = 0;
        foreach (EnemyPrefab enemy in enemyPrefabs)
        {
            currentWeight += enemy.weight;
            if (currentWeight >= chosenWeight)
            {
                return new System.Tuple<GameObject, float>(enemy.prefab, enemy.threat);
            }
        }
        throw new System.Exception("This shouldn't be happening!!!");
    }

    /// <summary>
    /// Return a list of enemies ready to be spawned based on the game difficulty.
    /// </summary>
    /// <param name="difficulty">Current difficulty</param>
    /// <returns>List of enemies prefab to spawn.</returns>
    public List<GameObject> GetEnemies(float difficulty)
    {
        float threat = 0;
        List<GameObject> enemies = new List<GameObject>();
        while (threat < 5 + (Mathf.Log(difficulty, 2) * 2))
        {
            System.Tuple<GameObject, float> enemy = GetEnemy(difficulty);
            threat += enemy.Item2;
            enemies.Add(enemy.Item1);
        }
        return enemies;
    }
}

[System.Serializable]
public class TransformRange {
    [Tooltip("Start transform.")]
    public Transform startTransform;
    [Tooltip("End transform.")]
    public Transform endTransform;
    [Tooltip("If not random will use only the start transform.")]
    public bool notRandom = false;

    public Vector3 startVector {
        get {
            return startTransform.position;
        }
    }
    public Vector3 endVector {
        get {
            return endTransform.position;
        }
    }

    /// <summary>
    /// Return a Vector3 position. If notRandom is true it will return the position of the startTransfom. On false, it will return a random Vector3 between the startTransform and the endTransform.
    /// </summary>
    /// <returns></returns>
    public Vector3 getVector()
    {
        if (notRandom)
            return startTransform.position;
        else
            return new Vector3(Random.Range(startTransform.position.x, endTransform.position.x), Random.Range(startTransform.position.y, endTransform.position.y), Random.Range(startTransform.position.z, endTransform.position.z));
    }
}

/*[System.Serializable]
public class Vector2RangeTwo {
    [Tooltip("Start vector.")]
    public Vector2 startVector;
    [Tooltip("End vector.")]
    public //Vector2 endVector;
    [Tooltip("If not random will use only the start vector.")]
    public bool notRandom = false;

    /// <summary>
    /// Return a Vector3 position. If notRandom is true it will return the position of the startVector. On false, it will return a random Vector3 between the startVector and the endVector.
    /// </summary>
    /// <returns></returns>
    public Vector2 getVector()
    {
        if (notRandom)
            return startVector;
        else
            return new Vector2(Random.Range(startVector.x, endVector.x), Random.Range(startVector.y, endVector.y));
    }
}*/