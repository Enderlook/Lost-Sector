using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShooter : EnemyBase
{
    [Header("Configuration")]
    [Tooltip("Constant speed of movement.")]
    public Vector2 cruiserSpeed;
    [Tooltip("Thruster acceleration speed.")]
    public float accelerationSpeed;
    [Tooltip("Rotation speed in degrees per second.")]
    public float rotationSpeed;
    [Tooltip("Weapon configuration.")]
    public Weapon weapon;
    
    private Rigidbody2D thisRigidbody2D;

    private float targetRotation;
    protected override void Start()
    {
        thisRigidbody2D = rigidbodyHelper.GetRigidbody2D();
        targetRotation = thisRigidbody2D.rotation;
        base.Start();
    }

    private void Update()
    {
        // Rotate
        // TODO: Rotation should be fixed using angular velocity not rotation
        float currentRotation = thisRigidbody2D.rotation;
        float isClockwise = (((thisRigidbody2D.rotation - targetRotation + 360) % 360) > 180) ? 1 : -1;
        float rotation = Time.deltaTime * rotationSpeed;
        if (thisRigidbody2D.angularVelocity != 0)
        {
            thisRigidbody2D.angularVelocity *= 0.75f + Time.deltaTime * 0.25f;
            if (Mathf.Abs(thisRigidbody2D.angularVelocity) < rotation || thisRigidbody2D.angularVelocity == 0)
                thisRigidbody2D.angularVelocity = 0;
            else
                thisRigidbody2D.angularVelocity += rotation * (thisRigidbody2D.angularVelocity > 0 ? -1 : 1);
        }
        else
        {
            if (((thisRigidbody2D.rotation < targetRotation) && (thisRigidbody2D.rotation + rotation > targetRotation)) ||
                ((thisRigidbody2D.rotation > targetRotation) && (thisRigidbody2D.rotation + rotation < targetRotation)) ||
                thisRigidbody2D.rotation == targetRotation)
                thisRigidbody2D.rotation = targetRotation;
            else
            {
                thisRigidbody2D.rotation += rotation * isClockwise;
            }
        }

        //Debug.Log(thisRigidbody2D.angularVelocity);
        //Debug.Log(Mathf.Abs(thisRigidbody2D.angularVelocity) < rotation);

        // Slowly accelerate to crusierSpeed
        Vector2 speedToReachCrusier = cruiserSpeed - thisRigidbody2D.velocity;
        Vector2 forceToReachCrusier = speedToReachCrusier * thisRigidbody2D.mass;
        Vector2 forceToApply = Vector2.ClampMagnitude(forceToReachCrusier * Time.deltaTime, accelerationSpeed);
        thisRigidbody2D.AddRelativeForce(forceToApply);

        // Shoot
        if (weapon.Recharge(Time.deltaTime))
            weapon.Shoot(rigidbodyHelper, Instantiate);
    }
}
