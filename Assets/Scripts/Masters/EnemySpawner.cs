using System.Collections;
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
        // TODO: This should stop on player death...
        // TODO: Custom modifications per enemy.
        while (true)
        {
            foreach (GameObject enemyPrefab in enemies.GetEnemies(difficulty))
            {
                TransformRange spawnRange = spawnPoints[Random.Range(0, spawnPoints.Length - 1)];
                Vector3 position = spawnRange.GetVector();

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

    /// <summary>
    /// Increase difficulty of the game. This isn't executed per frame but on a Coroutine because it would produce a greater geometrical progression that we don't want.
    /// </summary>
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

    /// <summary>
    /// Return the weighted rarity of the enemy to spawn. The weight changes according to the current difficulty.
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
            // https://www.codeproject.com/Questions/1031807/Evaluating-an-expression-using-DataTable-Compute-m
            // TODO: https://social.msdn.microsoft.com/Forums/en-US/2ee4bbbd-e18b-49b7-a119-57ef748e4f28/how-to-convert-a-string-operation-to-a-math-operation?forum=csharpgeneral
            // And: https://rosettacode.org/wiki/Parsing/Shunting-yard_algorithm#C.23
            DataTable dataTable = new DataTable();
            float multiplier = float.Parse(dataTable.Compute(weightFactorFormula.Replace("x", difficulty.ToString()).Replace(",", "."), "").ToString());
            return weight * multiplier;
        }
    }

    private void OnValidate()
    {
        if (weight <= 0)
            Debug.LogWarning($"The field {nameof(weight)} can't be lower or equal than 0.");
        if (prefab == null)
            Debug.LogWarning($"The field {nameof(weight)} can't be null.");
        // TODO: How to check minimalThreat?
    }
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
        float totalWeight = enemyPrefabs.Sum(enemy => enemy.GetWeight(difficulty));
        float chosenWeight = Random.value * totalWeight;

        float currentWeight = 0;
        foreach (EnemyPrefab enemy in enemyPrefabs)
        {
            currentWeight += enemy.GetWeight(difficulty);
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
        while (threat < 5 + (Mathf.Log(difficulty, 2) * 2))
        {
            System.Tuple<GameObject, float> enemy = GetEnemy(difficulty);
            threat += enemy.Item2;
            yield return enemy.Item1;
        }
    }
}