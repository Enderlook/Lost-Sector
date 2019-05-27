using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* https://forum.unity.com/threads/make-child-unaffected-by-parents-rotation.461161/
 * https://stackoverflow.com/questions/52179975/make-child-unaffected-by-parents-rotation-unity
 * https://docs.unity3d.com/Manual/class-ParentConstraint.html
 */

public class LivingObject : MonoBehaviour, IRigidbodyHelperConfiguration {
    // TODO: https://forum.unity.com/threads/exposing-fields-with-interface-type-c-solved.49524/

    [Header("Configuration")]
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

    [Tooltip("Relative damage on impact based on force.")]
    public float relativeImpactDamage;
    float IRigidbodyHelperConfiguration.ImpactDamage {
        get {
            return relativeImpactDamage;
        }
    }

    bool IRigidbodyHelperConfiguration.IsImpactDamageRelativeToImpulse {
        get {
            return true;
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

    [Tooltip("Die sound.")]
    public Sound dieSound;
    [Tooltip("Instanciate this explosion prefab on death.")]
    public GameObject onDeathExplosionPrefab;
    [Tooltip("Scale of the explosion prefab.")]
    [Range(0, 100)]
    public float onDeathExplosionPrefabScale;

    [Tooltip("Health bar script.")]
    public HealthBar healthBar;

    [Tooltip("RigidbodyHelper script.")]
    public RigidbodyHelper rigidbodyHelper;

    protected virtual void Start()
    {
        Health = InitializeBar(healthBar, startingMaxHealth, startingHealth);
        MaxHealth = startingMaxHealth;

        rigidbodyHelper.SetProperties(this);
    }

    /// <summary>
    /// Initializes the bar's values, setting the fill of the main bar and returning the current value.
    /// If startingValue is -1, startingMaximumValue will be used instead to fill the bar.
    /// The returned value is the value used to fill the bar, which can be startingValue or statingMaximumValue (if startingValue is -1).
    /// </summary>
    /// <param name="bar"></param>
    /// <param name="startingMaximumValue"></param>
    /// <param name="startingValue"></param>
    /// <returns></returns>
    protected float InitializeBar(HealthBar bar, float startingMaximumValue, float startingValue)
    {
        float value = startingValue == -1 ? startingMaximumValue : startingValue;
        bar.ManualUpdate(startingMaximumValue, value);
        return value;
    }

    /// <summary>
    /// Takes healing increasing its HP.
    /// </summary>
    /// <param name="amount">Amount of HP recovered. Must be positive</param>
    public void TakeHealing(float amount)
    {
        Health = ChangeValue(amount, Health, MaxHealth, true, "health");       
    }

    /// <summary>
    /// Take damage reducing its HP. Values must be positive.
    /// </summary>
    /// <param name="amount">Amount of HP lost. Must be positive.</param>
    public virtual void TakeDamage(float amount)
    {
        Health = ChangeValue(amount, Health, MaxHealth, false, "health");
    }

    // Can I use this? https://stackoverflow.com/questions/1402803/passing-properties-by-reference-in-c-sharp
    /// <summary>
    /// Amount must (should) be positive.
    /// If isAdding is true, amount will be added to variable. On false, amount will be reduced to variable.
    /// By any mean, variable will be greater than maximum or lower than 0. If that happens, the result will be replaced by the corresponding boundary and rest will store the difference between the surpassed value and the boundary.
    /// The values returned are the result of variable +/- amount (determined by isAdding) and the rest. If there is no rest it'll be 0. 
    /// </summary>
    /// <param name="amount"></param>
    /// <param name="variable"></param>
    /// <param name="maximum"></param>
    /// <param name="isAdding"></param>
    /// <param name="keyword"></param>
    /// <returns></returns>
    protected System.Tuple<float, float> ChangeValueWithRemain(float amount, float variable, float maximum, bool isAdding, string keyword)
    {
        //if ((shouldBePossitive && amount < 0) || (!shouldBePossitive && amount > 0))
        if (amount < 0)
            Debug.LogWarning($"{(isAdding ? "healing" : "damage")} amount was negative. The creature is {(isAdding ? "decreasing" : "increasing")} {keyword}.");

        if (!isAdding)
            amount = -amount;

        float rest = 0;
                
        if (variable + amount < 0)
        {
            rest = -(variable + amount);
            variable = 0;
        }
        else if (variable + amount > maximum)
        {
            rest = variable + amount - maximum;
            variable = maximum;
        }
        else
            variable += amount;

        return new System.Tuple<float, float>(variable, rest);
    }

    /// <summary>
    /// Amount must (should) be positive.
    /// If isAdding is true, amount will be added to variable. On false, amount will be reduced to variable.
    /// By any mean, variable will be greater than maximum or lower than 0. If that happens, the result will be replaced by the corresponding boundary.
    /// The value returned is the result of variable +/- amount (determined by isAdding).
    /// </summary>
    /// <param name="amount"></param>
    /// <param name="variable"></param>
    /// <param name="maximum"></param>
    /// <param name="isAdding"></param>
    /// <param name="keyword"></param>
    /// <returns></returns>
    protected float ChangeValue(float amount, float variable, float maximum, bool isAdding, string keyword)
    {
        return ChangeValueWithRemain(amount, variable, maximum, isAdding, keyword).Item1;
    }

    /// <summary>
    /// Destroy gameObject.
    /// </summary>
    protected virtual void Die()
    {
        Destroy(gameObject);
    }
}