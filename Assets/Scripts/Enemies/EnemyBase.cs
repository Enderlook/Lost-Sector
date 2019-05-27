using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBase : LivingObject//, ISpawningSubrutine
{
    [Header("Configuration")]
    [Tooltip("Initial impulse.")]
    public Vector2RangeTwo impulse;
        
    protected override void Start()
    {
        Rigidbody2D thisRigidbody2D = rigidbodyHelper.GetRigidbody2D();
        thisRigidbody2D.AddRelativeForce(-impulse.GetVector() * thisRigidbody2D.mass);
        base.Start();
    }

    /*private void OnBecameInvisible()
    {
        Destroy(gameObject);
    }*/

    /// <summary>
    /// Destroy gameObject and spawn an explosion instance on the current location.
    /// </summary>
    protected override void Die()
    {
        GameObject explosion = Instantiate(onDeathExplosionPrefab, Global.explosionsParent);
        explosion.transform.position = rigidbodyHelper.Position;
        explosion.transform.localScale = Vector3.one * onDeathExplosionPrefabScale;
        base.Die();
    }

    //public virtual void Spawn() {}
}

/*public interface ISpawningSubrutine {
    void Spawn();
}*/