using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAsteroid : EnemyBase
{
    [Header("Configurable")]
    [Tooltip("Random rotation between two value.")]
    public FloatRangeTwo rotation;

    protected override void Start()
    {
        rigidbodyHelper.GetRigidbody2D().angularVelocity = rotation.GetValue();
        base.Start();
    }
}
