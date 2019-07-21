using UnityEngine;

namespace LivingObjectAddons
{
    public class CrusierSpeed : Movement
    {
        [Header("Configuration")]
        [Tooltip("Constant speed of movement.")]
        public Vector2 cruiserSpeed;
        [Tooltip("Thruster acceleration speed.")]
        public float accelerationSpeed;

        private Rigidbody2D thisRigidbody;
        public override void OnBuild(LivingObject livingObject) => thisRigidbody = livingObject.rigidbodyHelper.GetRigidbody2D();

        public override void Move(float speedMultiplier)
        {
            // Slowly accelerate to crusierSpeed
            Vector2 speedToReachCrusier = cruiserSpeed * speedMultiplier - thisRigidbody.velocity;
            Vector2 forceToReachCrusier = speedToReachCrusier * thisRigidbody.mass;
            Vector2 forceToApply = Vector2.ClampMagnitude(forceToReachCrusier * Time.deltaTime, accelerationSpeed);
            thisRigidbody.AddRelativeForce(forceToApply);
        }
    }
}
