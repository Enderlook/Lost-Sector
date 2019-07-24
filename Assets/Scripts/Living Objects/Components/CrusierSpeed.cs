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

        protected Rigidbody2D thisRigidbody;
        public virtual void Build(LivingObject livingObject) => thisRigidbody = livingObject.rigidbodyHelper.Rigidbody2D;
        public virtual void Move(float speedMultiplier)
        {
            Vector2 localVelocity = thisRigidbody.transform.InverseTransformVector(thisRigidbody.velocity);
            Vector2 impulse = Vector2.ClampMagnitude(cruiserSpeed - localVelocity, accelerationSpeed * Time.deltaTime);
            thisRigidbody.AddRelativeForce(impulse * thisRigidbody.mass);
        }
    }
}
