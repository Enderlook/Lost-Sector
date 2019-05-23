using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour, IRigibodyHelperHandler {

    [Header("Configuration")]
    [Tooltip("Damage on impact.")]
    public float damage = 1;

    [Header("Setup")]
    [Tooltip("Impact sound.")]
    public Sound impactSound;
    Sound IImpactSound.ImpactSound {
        get {
            return impactSound;
        }
    }

    [Tooltip("RigibodyHelper script.")]
    public RigidbodyHelper rigidbodyHelper;

    private void Start()
    {
        rigidbodyHelper.SetHandler(this);
    }
    public float ImpactDamage {
        get {
            return damage;
        }
    }
    public void TakeDamage(float amount) {
        /* We are a bullet, we don't have HP... yet */
        Destroy(gameObject);
    }
    public void SetProjectilProperties(IProjectileConfiguration configuration)
    {
        transform.position = configuration.SpawnPosition;
        damage = configuration.Damage;
        Rigidbody2D rigidbody2D = rigidbodyHelper.GetRigidbody2D();
        rigidbody2D.AddForce(new Vector2(0, configuration.Speed * rigidbody2D.mass));
        // Just to be sure
        //rigidbody2D.AddRelativeForce(transform.forward * configuration.Speed * rigidbody2D.mass);
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

    public void ResetCooldown()
    {
        cooldownTime = 1 / firerate;
    }

    public void PlayShootingSound(AudioSource audioSource, float volumeMultiplier)
    {
        shootingSound.Play(audioSource, volumeMultiplier);
    }

    public float GetCooldown()
    {
        return cooldownTime;
    }
}
