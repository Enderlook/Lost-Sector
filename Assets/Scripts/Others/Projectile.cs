using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour, IRigidbodyHelperConfiguration {

    [Header("Configuration")]
    [Tooltip("Damage on impact.")]
    public float damage = 1;
    float IRigidbodyHelperConfiguration.ImpactDamage {
        get {
            return damage;
        }
    }

    [Header("Setup")]
    [Tooltip("Impact sound.")]
    public Sound impactSound;
    Sound IRigidbodyHelperConfiguration.ImpactSound {
        get {
            return impactSound;
        }
    }

    [Tooltip("RigidbodyHelper script.")]
    public RigidbodyHelper rigidbodyHelper;

    private void Start()
    {
        rigidbodyHelper.SetProperties(this);
    }
    
    bool IRigidbodyHelperConfiguration.IsImpactDamageRelativeToImpulse {
        get {
            return false;
        }
    }

    void IRigidbodyHelperConfiguration.TakeDamage(float amount) {
        // We are a bullet, we don't have HP... yet
        Destroy(gameObject);
    }
    
    /// <summary>
    /// Configure the projectile properties. Mandatory for usage of the projectile class.
    /// </summary>
    /// <param name="configuration">Configuration of the projectile.</param>
    public void SetProjectileProperties(IProjectileConfiguration configuration)
    {
        transform.position = configuration.SpawnPosition;
        damage = configuration.Damage;
        Rigidbody2D rigidbody2D = rigidbodyHelper.GetRigidbody2D();
        // You never know when you might need to rotate the parent, that is why we use AddRelativeForce() and transform.up instead of just AddForce()
        rigidbody2D.AddRelativeForce(transform.up * configuration.Speed * rigidbody2D.mass);
    }
}

public interface IProjectileConfiguration {
    Vector3 SpawnPosition { get; }
    float Damage { get; }
    float Speed { get; }
}

[System.Serializable]
public class Weapon : IProjectileConfiguration {
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

    Vector3 IProjectileConfiguration.SpawnPosition => shootingPosition.position;
    float IProjectileConfiguration.Damage => damageOnHit;
    float IProjectileConfiguration.Speed => speed;

    private float cooldownTime = 0f;

    /// <summary>
    /// Reduce cooldown time by deltaTime and checks if the weapon's cooldown is over. Returns true if the weapon is ready to shoot.
    /// </summary>
    /// <param name="deltaTime">Time from last frame (Time.deltaTime).</param>
    /// <returns>true if the weapon is ready to shoot, false if it's on cooldown.</returns>
    public bool Recharge(float deltaTime)
    {
        if (cooldownTime <= 0f)
            return true;
        else
        {
            cooldownTime -= deltaTime;
            return false;
        }
    }

    /// <summary>
    /// Reset cooldown time to maximum.
    /// </summary>
    public void ResetCooldown()
    {
        cooldownTime = 1 / firerate;
    }

    /// <summary>
    /// Play the fire sound.
    /// </summary>
    /// <param name="audioSource">AudioSource where the sound will be played.</param>
    /// <param name="volumeMultiplier">Volume of the sound, from 0 to 1.</param>
    public void PlayShootingSound(AudioSource audioSource, float volumeMultiplier)
    {
        shootingSound.Play(audioSource, volumeMultiplier);
    }
}
