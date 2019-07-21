using LivingObjectAddons;
using UnityEngine;

public class Projectile : MonoBehaviour, IRigidbodyHelperConfiguration, IMelee
{
    [Header("Configuration")]
    [Tooltip("Damage on impact.")]
    public float damage = 1;
    float IMelee.ImpactDamage => damage;
    [Tooltip("Relative damage on impact based on force.")]
    public bool isDamageRelativeToImpulse;
    bool IMelee.IsImpactDamageRelativeToImpulse => false;

    [Tooltip("Should spawn floating damage text on the enemy on collision?")]
    public bool shouldDisplayDamage;
    bool IShouldDisplayDamage.ShouldDisplayDamage => shouldDisplayDamage;

    [Header("Setup")]
    [Tooltip("Impact sound.")]
    public Sound impactSound;
    Sound IImpactSound.ImpactSound => impactSound;

    [Tooltip("RigidbodyHelper script.")]
    public RigidbodyHelper rigidbodyHelper;
    IMelee IRigidbodyHelperConfiguration.Melee => this;

    private void Start()
    {
        rigidbodyHelper.SetProperties(this);
    }

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
        Rigidbody2D rigidbody2D = rigidbodyHelper.Rigidbody2D;
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