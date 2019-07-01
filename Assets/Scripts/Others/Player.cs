using UnityEngine;

public class Player : LivingObject
{
    [Header("Configuration")]
    [Tooltip("Shield")]
    public HealthPoints shieldPoints;

    [Tooltip("Movement speed.")]
    public float moveSpeed;

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

    protected override void Initialize()
    {
        shieldPoints.Initialize();
        base.Initialize();
    }

    private void Update()
    {
        // Position to translate
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = transform.position.z;

        // Move
        Vector2 newPosition = Vector3.MoveTowards(transform.position, mousePosition, moveSpeed * Time.deltaTime);
        System.Tuple<Vector2, bool> boundaryCheck = Boundary.CheckForBoundaries(newPosition);
        transform.position = boundaryCheck.Item1;
        // Player is punished to try to move outside the screen
        if (boundaryCheck.Item2)
            TakeDamage(5 * Time.deltaTime);

        // Recharge shield
        if (currentShieldRechargeDelay >= shieldRechargeDelay && shieldPoints.Current < shieldPoints.Max)
            shieldPoints.Current = ChangeValueSimple(shieldRechargeRate * Time.deltaTime, shieldPoints.Current, shieldPoints.Max, true, "shield");
        else
            currentShieldRechargeDelay += Time.deltaTime;

        shieldHandler.UpdateColor(shieldPoints.Current, shieldPoints.Max);

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
        System.Tuple<float, float, float> change = ChangeValue(amount, shieldPoints.Current, shieldPoints.Max, false, "shield");
        SpawnFloatingText(change.Item2, Color.Lerp(new Color(.5f, 0, .5f), Color.blue, shieldPoints.Current / shieldPoints.Max));
        shieldPoints.Current = change.Item1;
        float restDamage = change.Item3;
        if (restDamage > 0)
        {
            base.TakeDamage(restDamage, true);
        }
    }
    protected override void Die()
    {
        Global.menu.GameOver(false);
        GameObject explosion = Instantiate(onDeathExplosionPrefab, Global.explosionsParent);
        explosion.transform.position = transform.position;
        base.Die();
    }
}