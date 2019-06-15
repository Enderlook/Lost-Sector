using UnityEngine;

public class EnemyBase : LivingObject
{
    [Header("Configuration")]
    [Tooltip("Initial impulse.")]
    public Vector2RangeTwo impulse;

    [Tooltip("Money spawned on death.\nOnly integers will be used.\nIf 0, no coins will be spawned.")]
    public FloatRangeTwo moneySpawnedOnDeath;

    [Header("Setup")]
    [Tooltip("Coins. spawner controller.")]
    public CoinController coinController;

    protected override void Start()
    {
        Rigidbody2D thisRigidbody2D = rigidbodyHelper.GetRigidbody2D();
        thisRigidbody2D.AddRelativeForce(impulse.GetVector() * thisRigidbody2D.mass);

        // Just to be sure...
        coinController.spawninigTransform = thisRigidbody2D.transform;

        base.Start();
    }

    protected override void Die()
    {
        coinController.SpawnCoins(moneySpawnedOnDeath.ValueInt);
        base.Die();
    }
}

