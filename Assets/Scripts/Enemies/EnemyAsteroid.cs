using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAsteroid : EnemyBase
{
    [Header("Configurable")]
    [Tooltip("Rotation.")]
    public float rotation;

    protected override void Start()
    {
        rigidbodyHelper/*.gameObject.GetComponent<Rigidbody2D>()*/.GetRigidbody2D().angularVelocity = rotation;
        base.Start();
    }

}
