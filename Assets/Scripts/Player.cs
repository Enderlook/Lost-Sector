using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : LivingObject
{
    [Header("Configurable")]
    [Tooltip("Movement speed.")]
    public float moveSpeed;
    /*public float turnSpeed;
    public float rotationOffset;*/

    private void Update()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = transform.position.z;

        /*// Rotate https://answers.unity.com/questions/798707/2d-look-at-mouse-position-z-rotation-c.html
        Vector3 direction = mousePosition - transform.position;
        float angleTarget = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + rotationOffset;       
        Quaternion lookAt = Quaternion.Euler(0, 0, angleTarget);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, lookAt, turnSpeed * Time.deltaTime);*/

        // Move
        transform.position = Vector3.MoveTowards(transform.position, mousePosition, moveSpeed * Time.deltaTime);
    }

    public override void TakeDamage(float amount)
    {
        // Damage reduction on low health
        if (Health - amount < MaxHealth * 0.35f)
            amount = (Health - amount) + (amount - Health) * (10 + Mathf.Exp(1) / Mathf.Exp(-amount / 10));
        base.TakeDamage(amount);
    }

    protected override void Die()
    {
        GameObject explosion = Instantiate(onDeathExplosionPrefab, Dynamic.Instance.explosionsParent);
        explosion.transform.position = transform.position;
        base.Die();
    }


}
