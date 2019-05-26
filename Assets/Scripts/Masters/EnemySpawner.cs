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
            foreach (GameObject enemyPrefab in enemies.SetDifficulty(difficulty)/*.GetEnemies(difficulty)*/)
            {
                TransformRange spawnRange = spawnPoints[Random.Range(0, spawnPoints.Length - 1)];
                Vector3 position = spawnRange.getVector();

                GameObject enemy = Instantiate(enemyPrefab, Global.enemiesParent);
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
public class Enemies : IEnumerable {
    [Tooltip("Enemy prefabs to spawn.")]
    public EnemyPrefab[] enemyPrefabs;

    private float difficulty;
   
    
    /// <summary>
    /// Get an enemy prefab to spawn.
    /// </summary>
    /// <param name="difficulty">Current difficulty used to base the type of enemy.</param>
    /// <returns>Enemy prefab to spawn</returns>
    /*public GameObject GetEnemyPrefab()
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
    /// <returns>Enemy prefab to spawn and threat of the enemy</returns>
    public System.Tuple<GameObject, float> GetEnemy()
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
    /// Sets current difficulty. Used to determine the amount and type of enemies.
    /// </summary>
    /// <param name="difficulty">Current difficulty.</param>
    /// <returns>Instance of the class itself.</returns>
    public Enemies SetDifficulty(float difficulty)
    {
        this.difficulty = difficulty;
        return this;
    }

    /// <summary>
    /// Get the current difficulty used to spawn enemies.
    /// </summary>
    /// <returns>Difficulty used to spawn enemies.</returns>
    public float GetDifficulty()
    {
        return difficulty;
    }

    /// <summary>
    /// Returns an IEnumerator of enemies that should be spawned.
    /// </summary>
    /// <returns>Enemies that should be spawned.</returns>
    IEnumerator IEnumerable.GetEnumerator()
    {
        // https://stackoverflow.com/questions/558304/can-anyone-explain-ienumerable-and-ienumerator-to-me
        return new EnemiesEnumerator(this);
    }

    /// <summary>
    /// Return a list of enemies ready to be spawned based on the game difficulty.
    /// </summary>
    /// <param name="difficulty">Current difficulty</param>
    /// <returns>List of enemies prefab to spawn.</returns>
    /*public List<GameObject> GetEnemies(float difficulty)
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
    }*/
}

public class EnemiesEnumerator : IEnumerator {
    private Enemies enemies;
    private GameObject currentEnemy;
    private float threat = 0;

    /// <summary>
    /// Constructor of the EnemiesEnumerator class.
    /// </summary>
    /// <param name="enemies">Enemies class which invokes this method.</param>
    public EnemiesEnumerator(Enemies enemies)
    {
        this.enemies = enemies;
    }

    /// <summary>
    /// Current enemy prefab to spawn.
    /// </summary>
    object IEnumerator.Current {
        get {
            return currentEnemy;
        }
    }

    /// <summary>
    /// Set a new enemy prefab to spawn in currentEnemy.
    /// </summary>
    /// <returns>True if currentEnemy was updated. False if there are no more enemies to spawn on this wave.</returns>
    bool IEnumerator.MoveNext()
    {
        while (threat < 5 + (Mathf.Log(enemies.GetDifficulty(), 2) * 2))
        {
            System.Tuple<GameObject, float> enemy = enemies.GetEnemy();
            threat += enemy.Item2;
            currentEnemy = enemy.Item1;
            return true;
        }
        return false;
    }

    /// <summary>
    /// Reset the enumerator.
    /// </summary>
    // Do we need this?
    void IEnumerator.Reset()
    {
        threat = 0;
        enemies.SetDifficulty(0);
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

    /// <summary>
    /// Vector3 of startTransform.
    /// </summary>
    public Vector3 startVector {
        get {
            return startTransform.position;
        }
    }

    /// <summary>
    /// Vector3 of endTransform.
    /// </summary>
    public Vector3 endVector {
        get {
            return endTransform.position;
        }
    }

    /// <summary>
    /// Return a Vector3 position. If notRandom is true it will return the position of the startTransfom. On false, it will return a random Vector3 between the startTransform and the endTransform.
    /// </summary>
    /// <returns>A random Vector3 between the values of start and end transform.</returns>
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