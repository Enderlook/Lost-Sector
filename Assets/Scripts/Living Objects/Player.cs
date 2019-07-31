using UnityEngine;

public class Player : LivingObject
{
    [Header("Configuration")]
    [Tooltip("Shield")]
    public HealthPoints shieldPoints;

    [Tooltip("Movement speed.")]
    public float moveSpeed;

    [Header("Build")]
    [Tooltip("Shield bar script.")]
    public HealthBar shieldBar;

    [Tooltip("Shield handler.")]
    public ShieldHandler shieldHandler;

    private float cumulativeOutOfBoundaryDamage = 0;

    private void Start() => Initialize(); // Fixs a bug where healthPoints are strangely set into 0.

    protected override void Initialize()
    {
        shieldPoints.Initialize();
        shieldHandler.Initialize(shieldPoints.Ratio);
        base.Initialize();
    }

    protected override void Update()
    {
        // Position to translate
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = transform.position.z;

        // Move
        Vector2 newPosition = Vector3.MoveTowards(transform.position, mousePosition, moveSpeed * SpeedMultiplier * Time.deltaTime);
        System.Tuple<Vector2, bool> boundaryCheck = Boundary.CheckForBoundaries(newPosition);
        transform.position = boundaryCheck.Item1;
        // Player is punished to try to move outside the screen
        if (boundaryCheck.Item2)
        {
            cumulativeOutOfBoundaryDamage += 5 * Time.deltaTime;
            if (cumulativeOutOfBoundaryDamage >= 1)
            {
                TakeDamage(1, true);
                cumulativeOutOfBoundaryDamage--;
            }
        }

        shieldPoints.Update(Time.deltaTime);
        shieldHandler.UpdateColor(shieldPoints.Ratio);
        base.Update();
    }

    /// <summary>
    /// Take damage reducing its <see cref="Shield"/> or <see cref="Health"/> if the first is 0.
    /// </summary>
    /// <param name="amount">Amount of damage received. Must be positive.</param>
    /// <param name="displayText">Whenever the damage taken must be shown in a floating text.</param>
    public override void TakeDamage(float amount, bool displayText = false)
    {
        // We ignore display text because we player always spawns floating text for damage.
        float restDamage = shieldPoints.TakeDamage(amount);
        SpawnFloatingText(amount, Color.Lerp(new Color(.5f, 0, .5f), Color.blue, shieldPoints.Ratio));
        if (restDamage > 0)
            base.TakeDamage(restDamage, true);
    }
    public override void Die(bool suicide = false)
    {
        if (!isDead)
            Global.menu.GameOver(false);
        base.Die(suicide);
    }
}