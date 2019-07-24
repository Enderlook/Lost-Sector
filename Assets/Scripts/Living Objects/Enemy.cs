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

    protected Rigidbody2D thisRigidbody2D;

    private void Awake() => thisRigidbody2D = rigidbodyHelper.Rigidbody2D;

    protected void Start()
    {
        // Just to be sure...
        coinController.spawninigTransform = thisRigidbody2D.transform;
    }

    protected override void Update()
    {
        if (autoShoot)
        {
            foreach (LivingObjectAddons.Weapon weapon in weapons)
            {
                weapon?.TryShoot(Time.deltaTime);
            }
        }
        base.Update();
    }

    protected override void Initialize()
    {
        thisRigidbody2D.AddRelativeForce((Vector2)impulse * thisRigidbody2D.mass);
        base.Initialize();
    }

    public override void Die(bool suicide = false)
    {
        if (!suicide && !isDead)
            coinController.SpawnCoins((int)moneySpawnedOnDeath);
        base.Die(suicide);
    }
}

