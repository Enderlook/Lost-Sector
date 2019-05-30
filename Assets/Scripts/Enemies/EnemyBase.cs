using UnityEngine;

public class EnemyBase : LivingObject
{
    [Header("Configuration")]
    [Tooltip("Initial impulse.")]
    public Vector2RangeTwo impulse;

    protected override void Start()
    {
        Rigidbody2D thisRigidbody2D = rigidbodyHelper.GetRigidbody2D();
        thisRigidbody2D.AddRelativeForce(impulse.GetVector() * thisRigidbody2D.mass);
        base.Start();
    }
}

