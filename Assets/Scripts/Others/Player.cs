using UnityEngine;

public class Player : LivingObject
{
    [Header("Configuration")]
    [Tooltip("Movement speed.")]
    public float moveSpeed;
    /*public float turnSpeed;
    public float rotationOffset;*/

    [Tooltip("Maximum shield.")]
    public float startingMaxShield = 100;
    private float _maxShield;
    /// <summary>
    /// Maximum amount of shield. <see cref="Shield"/> can't be greater than this value.<br/>
    /// If changes, <see cref="shieldBar"/> will be updated using <seealso cref="HealthBar.UpdateValues(float health, float maxHealth)"/>.<br/>
    /// </summary>
    /// <seealso cref="Shield"/>
    protected float MaxShield {
        get {
            return _maxShield;
        }
        set {
            _maxShield = value;
            shieldBar.UpdateValues(Shield, MaxShield);
        }
    }
    [Tooltip("Starting shield. Set -1 to use Max shield value.")]
    public float startingShield = -1;
    private float _shield;
    /// <summary>
    /// Current amount of shield. It can't be greater than <see cref="MaxShield"/><br/>
    /// If changes, <see cref="shieldBar"/> will be updated using <seealso cref="HealthBar.UpdateValues(float health, float maxHealth)"/>.<br/>
    /// </summary>
    /// <seealso cref="MaxShield"/>
    protected float Shield {
        get {
            return _shield;
        }
        set {
            _shield = value;
            shieldBar.UpdateValues(Shield, MaxShield);
        }
    }

    [Tooltip("Shield recharge rate (points per second).")]
    public float shieldRechargeRate = 10;
    [Tooltip("Amount of time in seconds after receive damage in order to start recharging shield.")]
    public float shieldRechargeDelay = 3f;
    private float currentShieldRechargeDelay = 0f;

    [Tooltip("Weapon configuration.")]
    public Weapon weapon;

    [Header("Build")]
    [Tooltip("Shield bar script.")]
    public HealthBar shieldBar;

    [Tooltip("Shield handler.")]
    public ShieldHandler shieldHandler;

    protected override void Start()
    {
        Shield = InitializeBar(shieldBar, startingMaxShield, startingShield);
        MaxShield = startingMaxShield;
        shieldHandler.Initialize(Shield, MaxShield);
        base.Start();
    }

    private void Update()
    {
        // Position to translate
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = transform.position.z;
        /*// Rotate https://answers.unity.com/questions/798707/2d-look-at-mouse-position-z-rotation-c.html
        Vector3 direction = mousePosition - transform.position;
        float angleTarget = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + rotationOffset;       
        Quaternion lookAt = Quaternion.Euler(0, 0, angleTarget);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, lookAt, turnSpeed * Time.deltaTime);*/

        // Move
        Vector2 newPosition = Vector3.MoveTowards(transform.position, mousePosition, moveSpeed * Time.deltaTime);
        System.Tuple<Vector2, bool> boundaryCheck = Boundary.CheckForBoundaries(newPosition);
        transform.position = boundaryCheck.Item1;
        // Player is punished to try to move outside the screen
        if (boundaryCheck.Item2)
            TakeDamage(5 * Time.deltaTime);

        // Recharge shield
        if (currentShieldRechargeDelay >= shieldRechargeDelay && Shield < MaxShield)
            Shield = ChangeValueSimple(shieldRechargeRate * Time.deltaTime, Shield, MaxShield, true, "shield");
        else
            currentShieldRechargeDelay += Time.deltaTime;

        shieldHandler.UpdateColor(Shield, MaxShield);

        // Shoot
        if (Input.GetMouseButton(0) && weapon.Recharge(Time.deltaTime))
            weapon.Shoot(rigidbodyHelper, Instantiate);
    }

    /// <summary>
    /// Take damage reducing its <see cref="Shield"/> or <see cref="Health"/> if the first is 0.
    /// </summary>
    /// <param name="amount">Amount of damage received. Must be positive.</param>
    /// <param name="displayText">Whenever the damage taken must be shown in a floating text.</param>
    public override void TakeDamage(float amount, bool displayText = false)
    {
        // We ignore display text because we player always spawns floating text for damage.

        currentShieldRechargeDelay = 0;
        System.Tuple<float, float, float> change = ChangeValue(amount, Shield, MaxShield, false, "shield");
        SpawnFloatingText(change.Item2, Color.Lerp(new Color(.5f, 0, .5f), Color.blue, Shield / MaxShield));
        Shield = change.Item1;
        float restDamage = change.Item3;
        if (restDamage > 0)
        {
            // Dynamic damage reduction which increases according to player's current health.
            // TODO: This should be a constant or something like that. Maybe a serializable field for Unity inspector?
            float health_threshold = 0.35f;
            if (Health - restDamage < MaxHealth * health_threshold)
            {
                float damage = restDamage * DamageReductionCalculator(restDamage, Health, MaxHealth, MaxHealth * health_threshold);
                // Damage can't be lower than 1 or the decimal part of restDamage, whatever is lower, regardless of the damage reduction.
                restDamage = Mathf.Min(damage, 1, restDamage % 1 != 0 ? restDamage % 1 : Mathf.Infinity);
            }

            //Health = ChangeValue(restDamage, Health, MaxHealth, false, "health");
            base.TakeDamage(restDamage, false);
        }
    }

    /// <summary>
    /// Reduced damage taking into account the special damage reduction by current health percent.
    /// Using <paramref name="health"/> and <paramref name="health_threshold"/> we can not only reduce damage on Health, but also on Shield if we wanted.
    /// </summary>
    /// <param name="damage">Amount of damage received (positive).</param>
    /// <param name="health">Current amount of health.</param>
    /// <param name="maxHealth">Maximum amount of health.</param>
    /// <param name="health_threshold">Health threshold for damage reduction (from 0 to 1).</param>
    /// <param name="precitionInterval">Interval of damage used in the fake integral. Small numbers increases precision at expenses of performance.</param>
    /// <returns>Damage reduction percent (from 0 to 1).</returns>
    private float DamageReductionCalculator(float damage, float health, float maxHealth, float health_threshold, float precitionInterval = 1)
    {
        float e = Mathf.Exp(1);
        float DamageReduction(float HP) => 1.25f - (e / Mathf.Exp(Mathf.Pow((-HP / maxHealth), 2)) / e);

        float currentHealth = health;
        float remainingDamage;
        // Reduce health above threshold
        if (currentHealth > health_threshold)
        {
            remainingDamage = damage - currentHealth;
            currentHealth -= health_threshold;
        }
        else
            remainingDamage = damage;

        // Fake integral
        // TODO: Apply a real integral
        int currentRemainingDamage = Mathf.FloorToInt(remainingDamage);
        float calculatedDamage = precitionInterval;
        for (; calculatedDamage <= currentRemainingDamage; calculatedDamage += precitionInterval)
        {
            // Damage * Damage reduction
            currentHealth -= 1 * DamageReduction(currentHealth);
        }
        // Remove last decimals
        currentHealth -= (remainingDamage - calculatedDamage) * DamageReduction(currentHealth);
        
        return health - currentHealth;
    }

    protected override void Die()
    {
        GameObject explosion = Instantiate(onDeathExplosionPrefab, Global.explosionsParent);
        explosion.transform.position = transform.position;
        base.Die();
    }
}