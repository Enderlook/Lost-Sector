using UnityEngine;

namespace LivingObjectAddons
{
    public class CrusierSpeed : MonoBehaviour, IBuild, IMove
    {
        [Header("Configuration")]
        [Tooltip("Constant speed of movement.")]
        public Vector2 cruiserSpeed;
        [Tooltip("Thruster acceleration speed.")]
        public float accelerationSpeed;

        private Rigidbody2D thisRigidbody;
        void IBuild.Build(LivingObject livingObject) => thisRigidbody = livingObject.rigidbodyHelper.GetRigidbody2D();
        void IMove.Move(float speedMultiplier)
        {
            // Slowly accelerate to crusierSpeed
            Vector2 speedToReachCrusier = cruiserSpeed * speedMultiplier - thisRigidbody.velocity;
            Vector2 forceToReachCrusier = speedToReachCrusier * thisRigidbody.mass;
            Vector2 forceToApply = Vector2.ClampMagnitude(forceToReachCrusier * Time.deltaTime, accelerationSpeed);
            thisRigidbody.AddRelativeForce(forceToApply);
        }
    }
}
