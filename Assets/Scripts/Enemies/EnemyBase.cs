using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// How to combine headers?

public class EnemyBase : LivingObject
{
    [Header("Configurable")]
    [Tooltip("Initial impulse.")]
    public Vector2 impulse;

    /*[Header("Setup")]
    [Tooltip("Rigidbody Helper")]
    public RigidbodyHelper rigidbodyHelper;*/

    protected override void Start()
    {
        rigidbodyHelper./*.gameObject.GetComponent<Rigidbody2D>()*/GetRigidbody2D().AddRelativeForce(impulse);
        base.Start();
    }

    /*private void OnBecameInvisible()
    {
        Destroy(gameObject);
    }*/


    protected override void Die()
    {
        GameObject explosion = Instantiate(onDeathExplosionPrefab, Dynamic.Instance.explosionsParent);
        explosion.transform.position = rigidbodyHelper.Position;
        explosion.transform.localScale = Vector3.one * onDeathExplosionPrefabScale;
        base.Die();
    }
}

