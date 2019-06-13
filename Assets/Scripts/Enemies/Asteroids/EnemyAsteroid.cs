using UnityEngine;

public class EnemyAsteroid : EnemyBase
{
    [Header("Configuration")]
    [Tooltip("Random rotation between two value.")]
    public FloatRangeTwo rotation;

    protected override void Start()
    {
        rigidbodyHelper.GetRigidbody2D().angularVelocity = rotation.Value;
        base.Start();
    }
}
