using UnityEngine;

public class EnemyAsteroid : Enemy
{
    [Header("Configuration")]
    [Tooltip("Random rotation between two value.")]
    public FloatRangeTwo rotation;

    protected override void Initialize()
    {
        rigidbodyHelper.GetRigidbody2D().angularVelocity = rotation.Value;
        base.Initialize();
    }
}
