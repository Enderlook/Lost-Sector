﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAsteroid : EnemyBase
{
    [Header("Configuration")]
    [Tooltip("Rotation.")]
    public float rotation;

    protected override void Start()
    {
        rigidbodyHelper.GetRigidbody2D().angularVelocity = Random.value > 0.5 ? rotation : -rotation;
        base.Start();
    }
}
