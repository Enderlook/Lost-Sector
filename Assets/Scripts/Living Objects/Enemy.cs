using System;
using UnityEngine;

public class Enemy : LivingObject
{
    [Header("Configuration")]
    [Tooltip("Initial impulse.")]
    public Vector2RangeTwo impulse;

    [Tooltip("Money spawned on death.\nOnly integers will be used.\nIf 0, no coins will be spawned.")]
    public FloatRangeTwo moneySpawnedOnDeath;

    [Tooltip("If it should always shoot.")]
    public bool autoShoot;

    [Header("Setup")]
    [Tooltip("Coins. spawner controller.")]
    public CoinController coinController;

    // Just to be sure...
    protected void Start() => coinController.spawninigTransform = rigidbodyHelper.Rigidbody2D.transform;

    protected override void Update()
    {
        if (autoShoot)
        {
            Array.ForEach(weapons, e => e.TryShoot(Time.deltaTime));
        }
        base.Update();
    }

    protected override void Initialize()
    {
        rigidbodyHelper.Rigidbody2D.AddRelativeForce((Vector2)impulse * rigidbodyHelper.Rigidbody2D.mass);
        base.Initialize();
    }

    public override void Die(bool suicide = false)
    {
        if (!suicide && !isDead)
            coinController.SpawnCoins((int)moneySpawnedOnDeath);
        base.Die(suicide);
    }
}

