using UnityEngine;

public class Projectile : MonoBehaviour, IRigidbodyHelperConfiguration
{
    [Header("Configuration")]
    [Tooltip("Damage on impact.")]
    public float damage = 1;
    float IRigidbodyHelperConfiguration.ImpactDamage => damage;

    [Tooltip("Should spawn floating damage text on the enemy on collision?")]
    public bool shouldDisplayDamage;
    bool IShouldDisplayDamage.ShouldDisplayDamage => shouldDisplayDamage;

    [Header("Setup")]
    [Tooltip("Impact sound.")]
    public Sound impactSound;
    Sound IRigidbodyHelperConfiguration.ImpactSound => impactSound;

    [Tooltip("RigidbodyHelper script.")]
    public RigidbodyHelper rigidbodyHelper;

    private void Start()
    {
        rigidbodyHelper.SetProperties(this);
    }

    bool IRigidbodyHelperConfiguration.IsImpactDamageRelativeToImpulse => false;

    void IRigidbodyHelperConfiguration.TakeDamage(float amount, bool displayDamage)
    {
        // We are a bullet, we don't have HP... yet
        Destroy(gameObject);
    }

    /// <summary>
    /// Configure the projectile properties. Mandatory.
    /// </summary>
    /// <param name="configuration">Configuration of the projectile.</param>
    public void SetProjectileProperties(IProjectileConfiguration configuration)
    {
        transform.position = configuration.SpawnPosition;
        damage = configuration.Damage;
        Rigidbody2D rigidbody2D = rigidbodyHelper.GetRigidbody2D();
        shouldDisplayDamage = configuration.ShouldDisplayDamage;
        // You never know when you might need to rotate the parent, that is why we use AddRelativeForce() and transform.up instead of just AddForce()
        rigidbody2D.AddRelativeForce(transform.up * configuration.Speed * rigidbody2D.mass);

        // https://forum.unity.com/threads/change-gameobject-layer-at-run-time-wont-apply-to-child.10091/ See post #post-1627654, #post-1819585, #post-3405070, #post-3676213. Get your own conclusions.
        // There could be more gameObjects to change layer
        foreach (var transform in gameObject.GetComponentsInChildren<Transform>(true))
        {
            transform.gameObject.layer = configuration.Layer;
        }
        gameObject.layer = configuration.Layer;
    }
}

public interface IProjectileConfiguration : IShouldDisplayDamage
{
    /// <summary>
    /// Position where the projectile will be spawned.
    /// </summary>
    Vector3 SpawnPosition { get; }
    /// <summary>
    /// Damage done by the projectile.
    /// </summary>
    float Damage { get; }
    /// <summary>
    /// Speed of the projectile.
    /// </summary>
    float Speed { get; }
    /// <summary>
    /// Layer mask of the projectile.
    /// </summary>
    int Layer { get; }
}

[System.Serializable]
public class Weapon : IProjectileConfiguration
{
    [Header("Configuration")]
    [Tooltip("Damage on hit.")]
    public float damageOnHit = 1;
    [Tooltip("Firerate (shoots per second).")]
    public float firerate = 1;
    [Tooltip("Speed.")]
    public float speed = 1;

    [Header("Setup")]
    [Tooltip("Transform point where projectiles will be spawn.")]
    public Transform shootingPosition;
    [Tooltip("Projectile prefab.")]
    public GameObject projectilePrefab;
    [Tooltip("Shooting sound.")]
    public Sound shootingSound;
    [Tooltip("Layer mask of the projectile")]
    public LayerMask layer;

    Vector3 IProjectileConfiguration.SpawnPosition => shootingPosition.position;
    float IProjectileConfiguration.Damage => damageOnHit;
    float IProjectileConfiguration.Speed => speed;
    int IProjectileConfiguration.Layer => layer.ToLayer();

    [Tooltip("Should spawn floating damage text on the enemy on collision?")]
    public bool shouldDisplayDamage;
    bool IShouldDisplayDamage.ShouldDisplayDamage {
        get {
            return shouldDisplayDamage;
        }
    }

    private float cooldownTime = 0f;

    /// <summary>
    /// Reduce <see cref="cooldownTime"/> time and checks if the weapon's <see cref="cooldownTime"/> is over.
    /// </summary>
    /// <param name="deltaTime"><see cref="Time.deltaTime"/></param>
    /// <returns><see langword="true"/> if the weapon is ready to shoot, <see langword="false"/> if it's on cooldown.</returns>
    public bool Recharge(float deltaTime)
    {
        return (cooldownTime -= deltaTime) <= 0f;
    }

    /// <summary>
    /// Reset <see cref="cooldownTime"/> time to maximum.
    /// </summary>
    public void ResetCooldown()
    {
        cooldownTime = 1 / firerate;
    }

    /// <summary>
    /// Generate an instance of a projectile an shoot it.
    /// In addition, cooldown is reseted and a shooting sound is played.
    /// </summary>
    /// <param name="rigidbodyHelper">RigibodyHelper of the shooter.</param>
    /// <param name="Instantiate">Instantiate UnityEngine method.</param>
    public void Shoot(RigidbodyHelper rigidbodyHelper, System.Func<GameObject, Transform, GameObject> Instantiate)
    {
        ResetCooldown();
        GameObject projectile = Instantiate(projectilePrefab, Global.projectilesParent);
        // Just to be sure. We don't really need to set rotation for our game
        projectile.transform.rotation = shootingPosition.rotation;
        projectile.GetComponent<Projectile>().SetProjectileProperties(this);
        shootingSound.Play(rigidbodyHelper.audioSource, 1);
    }

    /// <summary>
    /// Try to shoot a projectile. It will check for the <see cref="cooldownTime"/>, and if possible, shoot.
    /// </summary>
    /// <param name="rigidbodyHelper">RigibodyHelper of the shooter</param>
    /// <param name="Instantiate">Instantiate UnityEngine method.</param>
    /// <param name="deltaTime"><see cref="Time.deltaTime"/></param>
    /// <returns><see langword="true"/> if the weapon shoot, <see langword="false"/> if it's still on cooldown.</returns>
    public bool TryShoot(RigidbodyHelper rigidbodyHelper, System.Func<GameObject, Transform, GameObject> Instantiate, float deltaTime)
    {
        if (Recharge(deltaTime))
        {
            Shoot(rigidbodyHelper, Instantiate);
            return true;
        }
        return false;
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (1 < ((IProjectileConfiguration)this).Layer && ((IProjectileConfiguration)this).Layer < 31)
            Debug.LogWarning($"The field {nameof(layer)} should only contain a single layer.");
    }
#endif
}
