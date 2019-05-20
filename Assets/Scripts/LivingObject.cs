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

    protected virtual void Start()
    {
        /*float health = startingHealth == -1 ? startingMaxHealth : startingHealth;
        healthBar.ManualUpdate(startingMaxHealth, health);*/
        Health = InitializeBar(healthBar, startingMaxHealth, startingHealth);
        MaxHealth = startingMaxHealth;
    }

    /// <summary>
    /// Values must be positive.
    /// </summary>
    /// <param name="amount"></param>
    public void TakeHealing(float amount)
    {
        Health = ChangeValue(amount, Health, MaxHealth, true, "health");
        /*if (amount < 0)
        {
            Debug.LogError("Healing amount was negative. The creature lose health.");

        }
        if (Health + amount > MaxHealth)
            Health = MaxHealth;
        else
            Health += amount;*/
        //ChangeValue(amount);
    }

    /// <summary>
    /// Values must be positive.
    /// </summary>
    /// <param name="amount"></param>
    public virtual void TakeDamage(float amount)
    {
        Health = ChangeValue(amount, Health, MaxHealth, false, "health");
        /*if (amount < 0)
        {
            Debug.LogWarning("Damage amount was negative. The creature recover health.");
        }
        if (Health - amount < 0)
            Health = 0;
        else
            Health -= amount;*/
        //ChangeValue(-amount);
    }

    /*private void ChangeValue(float amount)
    {
        if (Health + amount < 0)
            Health = 0;
        else if (Health + amount > MaxHealth)
            Health = MaxHealth;
        else
            Health = amount;
    }*/

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
    protected /*System.Tuple<float, float>*/ (float value, float remain) ChangeValueWithRemain(float amount, float variable, float maximum, bool isAdding, string keyword)
    {
        //if ((shouldBePossitive && amount < 0) || (!shouldBePossitive && amount > 0))
        if (amount < 0)
            Debug.LogWarning($"{(isAdding ? "healing" : "damage")} amount was negative. The creature {(isAdding ? "decreasing" : "increasing")} {keyword}.");

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
            variable = MaxHealth;
        }
        else
            variable += amount;

        return /* new System.Tuple<float, float>(variable, rest)*/ (value: variable, remain: rest);
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
        return ChangeValueWithRemain(amount, variable, maximum, isAdding, keyword).value; //.Item1;
    }

    protected virtual void Die()
    {
        Destroy(gameObject);
    }
}