using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;

public class CoinController : MonoBehaviour
{
    [Header("Configuration")]
    [Tooltip("Random movement speed.")]
    public FloatRangeTwo movementSpeed;

    [Header("Setup")]
    [Tooltip("Transform to spawn coins.")]
    public Transform spawninigTransform;
    [Tooltip("Coin prefab to spawn.")]
    public GameObject coinPrefab;

    // We want to always check in this order later...
    private static readonly OrderedDictionary PRICES_TABLE = new OrderedDictionary()
    {
        { "gold", 100 }, { "silver", 10 }, { "copper", 1 }
    };

    /// <summary>
    /// Spawn money.
    /// </summary>
    /// <param name="money">Amount of money to spawn.</param>
    public void SpawnCoins(int money)
    {
        if (money > 0)
            SpawnCoinsWithConfig(CalculateCoins(money));
    }

    /// <summary>
    /// Spawn coins.
    /// </summary>
    /// <param name="coinsToSpawn">Amount of each type of coin</param>
    private void SpawnCoinsWithConfig(IEnumerable<System.Tuple<string, int>> coinsToSpawn)
    {
        foreach (System.Tuple<string, int> coinToSpawn in coinsToSpawn)
        {
            for (int i = 0; i < coinToSpawn.Item2; i++)
            {
                GameObject coin = Global.enemySpawner.Spawn(coinPrefab, Global.pickupsParent);

                coin.transform.position = spawninigTransform.position;

                Coin coinScript = coin.GetComponent<Coin>();

                coinScript.SetConfiguration((int)PRICES_TABLE[coinToSpawn.Item1], coinToSpawn.Item1);

                // https://answers.unity.com/questions/1111106/add-force-in-random-direction-with-set-speed.html
                coin.GetComponent<Rigidbody2D>().velocity = Random.insideUnitCircle * (float)movementSpeed;
            }
        }
    }

    // We could use a Dictionary<string, int> also...
    /// <summary>
    /// Calculate the coins that should be spawned.
    /// </summary>
    /// <param name="price">Money worth of coins</param>
    /// <returns><c>System.Tuple<string, int></c> of type of coin and its amount.</returns>
    private IEnumerable<System.Tuple<string, int>> CalculateCoins(int price)
    {
        foreach (DictionaryEntry coin in PRICES_TABLE)
        {
            int amount = price / (int)coin.Value;
            yield return new System.Tuple<string, int>(coin.Key.ToString(), amount);
            price -= amount * (int)coin.Value;
        }
    }
}