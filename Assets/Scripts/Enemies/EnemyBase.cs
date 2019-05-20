using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBase : LivingObject
{
    [Header("Configurable")]
    [Tooltip("Initial impulse.")]
    public Vector2 impulse;

    [Header("Setup")]
    [Tooltip("Rigidbody Helper")]
    public RigidbodyHelper rigidbodyHelper;

    protected override void Start()
    {
        rigidbodyHelper.gameObject.GetComponent<Rigidbody2D>().AddRelativeForce(impulse);
        base.Start();
    }

    /*private void OnBecameInvisible()
    {
        Destroy(gameObject);
    }*/

    // How to combine headers?

    protected override void Die()
    {
        GameObject explosion = Instantiate(onDeathExplosionPrefab, Dynamic.Instance.explosionsParent);
        explosion.transform.position = rigidbodyHelper.Position;
        base.Die();
    }
}

