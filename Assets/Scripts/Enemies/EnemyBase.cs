using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBase : LivingObject//, ISpawningSubrutine
{
    [Header("Configurable")]
    [Tooltip("Initial impulse.")]
    public Vector2 impulse;
    
    protected override void Start()
    {
        Rigidbody2D thisRigidbody2D = rigidbodyHelper.GetRigidbody2D();
        thisRigidbody2D.AddRelativeForce(-new Vector2(Random.Range(0, 15), Random.Range(50, 300)) * thisRigidbody2D.mass);
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

    //public virtual void Spawn() {}
}

/*public interface ISpawningSubrutine {
    void Spawn();
}*/