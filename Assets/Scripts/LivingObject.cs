using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* https://forum.unity.com/threads/make-child-unaffected-by-parents-rotation.461161/
 * https://stackoverflow.com/questions/52179975/make-child-unaffected-by-parents-rotation-unity
 * https://docs.unity3d.com/Manual/class-ParentConstraint.html
 */

public class LivingObject : MonoBehaviour {
    [Header("Configurable")]
    [Tooltip("Maximum health.")]
    public float startingMaxHealth = 100;
    private float _maxHealth;
    protected float MaxHealth {
        get {
            return _maxHealth;
        }
        set {
            _maxHealth = value;
            healthBar.UpdateValues(Health, MaxHealth);
            if (MaxHealth <= 0) Die();
        }
    }
    [Tooltip("Starting health. Set -1 to use Max Health value.")]
    public float startingHealth = -1;
    private float _health;
    protected float Health {
        get {
            return _health;
        }
        set {
            _health = value;
            healthBar.UpdateValues(Health, MaxHealth);
            if (Health <= 0) Die();
        }
    }
    [Tooltip("Relative damage on impact based on focer.")]
    public float relativeImpactDamage;


    [Header("Setup")]
    [Tooltip("Impact sound.")]
    public Sound impactSound;
    [Tooltip("Die sound.")]
    public Sound dieSound;
    [Tooltip("Instanciate this explosion prefab on death.")]
    public GameObject onDeathExplosionPrefab;
    [Tooltip("Scale of the explosion prefab.")]
    [Range(0, 100)]
    public float onDeathExplosionPrefabScale;

    [Tooltip("Health bar script.")]
    public HealthBar healthBar;

    protected virtual void Start()
    {
        float health = startingHealth == -1 ? startingMaxHealth : startingHealth;
        healthBar.ManualStart(startingMaxHealth, health);
        Health = health;
        MaxHealth = startingMaxHealth;
    }

    public void TakeHealing(float amount)
    {
        if (amount < 0)
        {
            Debug.LogWarning($"Healing amount was negative. The creature lose health.");
        }
        Health += amount;
    }
    public virtual void TakeDamage(float amount)
    {
        if (amount < 0)
        {
            Debug.LogWarning($"Damage amount was negative. The creature recover health.");
        }
        Health -= amount;
    }

    protected virtual void Die()
    {
        Destroy(gameObject);
    }
}