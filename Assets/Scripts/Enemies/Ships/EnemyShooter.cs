using UnityEngine;

public class EnemyShooter : EnemyBase
{
    [Header("Configuration")]
    [Tooltip("Constant speed of movement.")]
    public Vector2 cruiserSpeed;
    [Tooltip("Thruster acceleration speed.")]
    public float accelerationSpeed;
    [Tooltip("Weapon configuration.")]
    public Weapon weapon;
    
    private void Update()
    {
        // Slowly accelerate to crusierSpeed
        Vector2 speedToReachCrusier = cruiserSpeed - thisRigidbody2D.velocity;
        Vector2 forceToReachCrusier = speedToReachCrusier * thisRigidbody2D.mass;
        Vector2 forceToApply = Vector2.ClampMagnitude(forceToReachCrusier * Time.deltaTime, accelerationSpeed);
        thisRigidbody2D.AddRelativeForce(forceToApply);

        // Shoot
        if (weapon.Recharge(Time.deltaTime))
            weapon.Shoot(rigidbodyHelper, Instantiate);
    }
}
