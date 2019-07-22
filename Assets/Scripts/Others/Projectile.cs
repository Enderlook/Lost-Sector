using LivingObjectAddons;
using UnityEngine;

public class Projectile : MonoBehaviour, IRigidbodyHelperConfiguration
{
    [Header("Setup")]
    [Tooltip("RigidbodyHelper script.")]
    public RigidbodyHelper rigidbodyHelper;

    private IMelee melee;
    IMelee IRigidbodyHelperConfiguration.Melee => melee;
    
    void IRigidbodyHelperConfiguration.TakeDamage(float amount, bool displayDamage) => Destroy(gameObject);

    bool IRigidbodyHelperConfiguration.IsImpactDamageRelativeToImpulse => false;    
    
    private void Awake() => melee = gameObject.GetComponent<IMelee>();
    private void Start() => rigidbodyHelper.SetProperties(this);
    
    /// <summary>
    /// Configure the projectile properties. Mandatory.
    /// </summary>
    /// <param name="configuration">Configuration of the projectile.</param>
    public void SetProjectileProperties(IProjectileConfiguration configuration)
    {
        transform.position = configuration.SpawnPosition;
        melee.ImpactDamage = configuration.ImpactDamage;
        Rigidbody2D rigidbody2D = rigidbodyHelper.Rigidbody2D;
        melee.ShouldDisplayDamage = configuration.ShouldDisplayDamage;
        melee.IsImpactDamageRelativeToImpulse = configuration.IsImpactDamageRelativeToImpulse;
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

public interface IProjectileConfiguration : IMelee
{
    /// <summary>
    /// Position where the projectile will be spawned.
    /// </summary>
    Vector3 SpawnPosition { get; }
    /// <summary>
    /// Speed of the projectile.
    /// </summary>
    float Speed { get; }
    /// <summary>
    /// Layer mask of the projectile.
    /// </summary>
    int Layer { get; }
}