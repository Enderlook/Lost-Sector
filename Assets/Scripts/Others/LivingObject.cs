using UnityEngine;

/* https://forum.unity.com/threads/make-child-unaffected-by-parents-rotation.461161/
 * https://stackoverflow.com/questions/52179975/make-child-unaffected-by-parents-rotation-unity
 * https://docs.unity3d.com/Manual/class-ParentConstraint.html
 */

public class LivingObject : MonoBehaviour, IRigidbodyHelperConfiguration
{
    // TODO: https://forum.unity.com/threads/exposing-fields-with-interface-type-c-solved.49524/

    [Header("Configuration")]
    [Tooltip("Maximum health.")]
    public float startingMaxHealth = 100;
    private float _maxHealth;
    /// <summary>
    /// Maximum amount of health. <see cref="Health"/> can't be greater than this value.<br/>
    /// If changes, <see cref="healthBar"/> will be updated using <seealso cref="HealthBar.UpdateValues(float health, float maxHealth)"/>.<br/>
    /// If 0, <see cref="Die()"/> is called.
    /// </summary>
    /// <seealso cref="Health"/>
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
    /// <summary>
    /// Current amount of health. It can't be greater than <see cref="MaxHealth"/><br/>
    /// If changes, <see cref="healthBar"/> will be updated using <seealso cref="HealthBar.UpdateValues(float health, float maxHealth)"/>.<br/>
    /// If 0, <see cref="Die()"/> is called.
    /// </summary>
    /// <seealso cref="MaxHealth"/>
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
    float IRigidbodyHelperConfiguration.ImpactDamage => relativeImpactDamage;

    bool IRigidbodyHelperConfiguration.IsImpactDamageRelativeToImpulse => true;

    [Tooltip("Should spawn floating damage text on the enemy on collision?")]
    public bool shouldDisplayDamage;
    bool IShouldDisplayDamage.ShouldDisplayDamage => shouldDisplayDamage;

    [Header("Setup")]
    [Tooltip("Impact sound.")]
    public Sound impactSound;
    Sound IRigidbodyHelperConfiguration.ImpactSound => impactSound;

    [Tooltip("Die sound.")]
    public Sound dieSound;
    [Tooltip("Instantiate this explosion prefab on death.")]
    public GameObject onDeathExplosionPrefab;
    [Tooltip("Scale of the explosion prefab.")]
    [Range(0, 100)]
    public float onDeathExplosionPrefabScale = 1;

    [Tooltip("Health bar script.")]
    public HealthBar healthBar;

    [Tooltip("RigidbodyHelper script.")]
    public RigidbodyHelper rigidbodyHelper;

    [Tooltip("FloatingTextController Script")]
    public FloatingTextController floatingTextController;

    protected virtual void Start()
    {
        rigidbodyHelper.SetProperties(this);
        Initialize();
    }

    /// <summary>
    /// Initializes some values that are reused during the enemies recycling.
    /// </summary>
    protected virtual void Initialize()
    {
        rigidbodyHelper.transform.position = transform.position;
        rigidbodyHelper.transform.rotation = transform.rotation;
        Health = InitializeBar(healthBar, startingMaxHealth, startingHealth);
        MaxHealth = startingMaxHealth;
    }

    private void OnEnable() => Initialize();

    /// <summary>
    /// Initializes the bar's values, setting the fill of the main bar and returning the current value.
    /// If <paramref name="startingValue"/> is -1, <paramref name="startingMaximumValue"/> will be used instead to fill the bar.
    /// </summary>
    /// <param name="bar"><see cref="HealthBar"/> to initialize.</param>
    /// <param name="startingMaximumValue">Maximum value of the bar.</param>
    /// <param name="startingValue">Current value of the bar.</param>
    /// <returns>The value used to fill the bar, which can be <paramref name="startingValue"/> or <paramref name="startingMaximumValue"/> (if <c><paramref name="startingValue"/> == -1</c>).</returns>
    /// <seealso cref="HealthBar"/>
    protected float InitializeBar(HealthBar bar, float startingMaximumValue, float startingValue)
    {
        float value = startingValue == -1 ? startingMaximumValue : startingValue;
        bar.ManualUpdate(startingMaximumValue, value);
        return value;
    }

    /// <summary>
    /// Takes healing increasing its <see cref="Health"/>.
    /// </summary>
    /// <param name="amount">Amount of <see cref="Health"/> recovered. Must be positive.</param>
    public void TakeHealing(float amount)
    {
        Health = ChangeValueSimple(amount, Health, MaxHealth, true, "health");
    }

    /// <summary>
    /// Take damage reducing its <see cref="Health"/>.
    /// </summary>
    /// <param name="amount">Amount of <see cref="Health"/> lost. Must be positive.</param>
    /// <param name="displayText">Whenever the damage taken must be shown in a floating text.</param>
    public virtual void TakeDamage(float amount, bool displayDamage = false)
    {
        System.Tuple<float, float, float> change = ChangeValue(amount, Health, MaxHealth, false, "health");
        Health = change.Item1;
        if (displayDamage)
            SpawnFloatingText(change.Item2, Color.Lerp(Color.red, new Color(1, .5f, 0), Health / MaxHealth));
    }

    // TODO: https://stackoverflow.com/questions/1402803/passing-properties-by-reference-in-c-sharp
    /// <summary>
    /// If <paramref name="isAdding"/> is <see langword="true"/>, <paramref name="amount"/> will be added to <paramref name="variable"/>. On <see langword="false"/>, <paramref name="amount"/> will be reduced to <paramref name="variable"/>.
    /// By any mean, <paramref name="variable"/> will be greater than <paramref name="maximum"/> or lower than 0. If that happens, the result will be replaced by the corresponding boundary (either 0 or <paramref name="maximum"/>) and the <c>rest</c> will store the difference between the surpassed value and the boundary.
    /// The values returned are the result of variable +/- amount (determined by isAdding) and the rest. If there is no rest it'll be 0. 
    /// </summary>
    /// <param name="amount">Amount to add or reduct of <paramref name="variable"/>. Must be positive.</param>
    /// <param name="variable">Variable where <paramref name="amount"/> will be added or reduced.</param>
    /// <param name="maximum">Hight limit of the result. Result will be clamped between this and 0.</param>
    /// <param name="isAdding">Whenever <paramref name="amount"/> will be added (<see langword="true"/>) or reduced (<see langword="false"/>) to <paramref name="variable"/>.</param>
    /// <param name="keyword"></param>
    /// <returns><c>Item1</c> is the new <paramref name="variable"/> value.<br/>
    /// <c>Item2</c> total damage taken is always positive.<br/>
    /// <c>Item3</c> is the rest damage that wasn't able to take.</returns>
    /// <see cref="ChangeValueSimple(float amount, float variable, float maximum, bool isAdding, string keyword)"/>
    protected System.Tuple<float, float, float> ChangeValue(float amount, float variable, float maximum, bool isAdding, string keyword)
    {
        //if ((shouldBePossitive && amount < 0) || (!shouldBePossitive && amount > 0))
        float total = 0;
        if (amount < 0)
            Debug.LogWarning($"{(isAdding ? "healing" : "damage")} amount was negative. The creature is {(isAdding ? "decreasing" : "increasing")} {keyword}.");

        if (!isAdding)
            amount = -amount;

        float rest = 0;

        if (variable + amount < 0)
        {
            rest = -(variable + amount);
            total += variable;
            variable = 0;
        }
        else if (variable + amount > maximum)
        {
            rest = variable + amount - maximum;
            total += maximum - variable;
            variable = maximum;
        }
        else
        {
            variable += amount;
            total += amount;
        }

        return new System.Tuple<float, float, float>(variable, Mathf.Abs(total), rest);
    }

    /// <summary>
    /// <summary>
    /// If <paramref name="isAdding"/> is <see langword="true"/>, <paramref name="amount"/> will be added to <paramref name="variable"/>. On <see langword="false"/>, <paramref name="amount"/> will be reduced to <paramref name="variable"/>.
    /// By any mean, <paramref name="variable"/> will be greater than <paramref name="maximum"/> or lower than 0. If that happens, the result will be replaced by the corresponding boundary (either 0 or <paramref name="maximum"/>) and the <c>rest</c> will store the difference between the surpassed value and the boundary.
    /// The values returned are the result of variable +/- amount (determined by isAdding) and the rest. If there is no rest it'll be 0. 
    /// </summary>
    /// <param name="amount">Amount to add or reduct of <paramref name="variable"/>. Must be positive.</param>
    /// <param name="variable">Variable where <paramref name="amount"/> will be added or reduced.</param>
    /// <param name="maximum">Hight limit of the result. Result will be clamped between this and 0.</param>
    /// <param name="isAdding">Whenever <paramref name="amount"/> will be added (<see langword="true"/>) or reduced (<see langword="false"/>) to <paramref name="variable"/>.</param>
    /// <param name="keyword"></param>
    /// <return>New <paramref name="variable"/> value.</return>
    /// <seealso cref="ChangeValue(float amount, float variable, float maximum, bool isAdding, string keyword)"/>
    protected float ChangeValueSimple(float amount, float variable, float maximum, bool isAdding, string keyword)
    {
        return ChangeValue(amount, variable, maximum, isAdding, keyword).Item1;
    }

    /// <summary>
    /// Destroy <see cref="gameObject"/> and spawn an explosion prefab instance on current location.
    /// </summary>
    protected virtual void Die()
    {
        GameObject explosion = Instantiate(onDeathExplosionPrefab, Global.explosionsParent);
        explosion.transform.position = rigidbodyHelper.Position;
        explosion.transform.localScale = Vector3.one * onDeathExplosionPrefabScale;
        Destroy(explosion);
        gameObject.SetActive(false);
        //Destroy(gameObject);
    }

    /// <summary>
    /// Spawn a floating text above the creature.
    /// </summary>
    /// <param name="text">Text to display.</param>
    /// <param name="textColor">Color of the text.</param>
    /// <param name="checkIfPositive">Only display if the number is positive.</param>
    protected void SpawnFloatingText(float text, Color? textColor, bool checkIfPositive = true)
    {
        if (floatingTextController != null && (!checkIfPositive || text > 0))
            floatingTextController.SpawnFloatingText(text, textColor);
    }
}