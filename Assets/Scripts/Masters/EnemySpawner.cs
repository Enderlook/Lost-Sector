using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    // TODO: COMPLETE REWORK!!!
    public GameObject enemyPrefab;    
    //public Enemies test;
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
        while (true)
        {
            for (int i = 0; i < 5 + (Mathf.Log(difficulty, 2) * 2); i++)
            {
                TransformRange spawnRange = spawnPoints[Random.Range(0, spawnPoints.Length - 1)];
                Vector3 position = spawnRange.getVector();
                Vector3 impulse = new Vector2(Random.Range(0, 15), Random.Range(50, 300));

                GameObject enemy = Instantiate(enemyPrefab, Global.enemiesParent);
                enemy.transform.position = position;
                enemy.GetComponent<EnemyBase>().impulse = -impulse; // - for downwards impulse

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
            float multiplier = (float)dataTable.Compute(weightFactorFormula.Replace("x", difficulty.ToString()), "");

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
    public GameObject GetEnemyPrefab(float difficulty)
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

/*[System.Serializable]
public class SpawnSide {
public enum SPAWN_SIDES { TOP, BOTTOM, LEFT, RIGHT, CUSTOM };

[Tooltip("Spawn position mode. If there are more than one, randomly will be chosen one")]
public SPAWN_SIDES[] spawnSides = new SPAWN_SIDES[] { SPAWN_SIDES.TOP };

/* TODO:
 * https://forum.unity.com/threads/draw-a-field-only-if-a-condition-is-met.448855/#post-3435603
 * https://answers.unity.com/questions/54010/is-it-possible-to-dynamically-disable-or-validate.html
 */
/*[Tooltip("Custom spawning random position (only use if spawnSides is CUSTOM). Made in pairs of random spawning lines.")]
public TransformRange[] customSpawnPositions;

// TODO
[Tooltip("If wait until the before thing is done, and then wait Seconds Before Start in order to do something. If false, it won't wait until the thing before is done.")]
public bool waitUntilFinishBefore;
[Tooltip("Amount of seconds waited before start doing something.")]
public float secondsBeforeStart;
[Tooltip("Amount of seconds waited between each done thing.")]
public float intervalSeconds;

private Vector3 GetSpawnLocation(Vector3 worldDimensions, SPAWN_SIDES side)
{
    switch (side)
    { // Not use break statement due to return usage
        case SPAWN_SIDES.TOP:
            return new Vector3(Random.Range(-worldDimensions.x, worldDimensions.x), worldDimensions.y);
        case SPAWN_SIDES.BOTTOM:
            return new Vector3(Random.Range(-worldDimensions.x, worldDimensions.x), -worldDimensions.y);
        case SPAWN_SIDES.LEFT:
            return new Vector3(-worldDimensions.x, Random.Range(-worldDimensions.y, worldDimensions.y));
        case SPAWN_SIDES.RIGHT:
            return new Vector3(-worldDimensions.x, Random.Range(-worldDimensions.y, worldDimensions.y));
        case SPAWN_SIDES.CUSTOM:
            TransformRange spawnPositionRange = customSpawnPositions[Random.Range(0, customSpawnPositions.Length) - 1];
            return new Vector3(Random.Range(spawnPositionRange.startVector.x, spawnPositionRange.endVector.x), Random.Range(spawnPositionRange.startVector.x, spawnPositionRange.endVector.y));
        default:
            // https://stackoverflow.com/questions/105372/how-do-i-enumerate-an-enum-in-c
            Debug.LogError(new System.Exception($"The side {side} isn't none of the possible values of SPAWN_SIDES = {System.Enum.GetValues(typeof(SPAWN_SIDES)).Cast<SPAWN_SIDES>().Select(e => $"{e} = {System.Convert.ToInt32(e)}")})"));
            // If I don't add this, IDE says that not all the possible paths return a value
            throw new System.Exception($"The side {side} isn't none of the possible values of SPAWN_SIDES = {System.Enum.GetValues(typeof(SPAWN_SIDES)).Cast<SPAWN_SIDES>().Select(e => $"{e} = {System.Convert.ToInt32(e)}")})");
    }
}

private Vector3 GetSpawnLocation(Vector3 worldDimensions)
{
    return GetSpawnLocation(worldDimensions, spawnSides[Random.Range(0, spawnSides.Length) - 1]);
}
}*/
