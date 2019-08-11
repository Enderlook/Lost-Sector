using UnityEngine;

namespace LivingObjectAddons
{
    public class Kamikaze : MonoBehaviour, IBuild, IInitialize, IMove
    {
        [Header("Configuration")]
        [Tooltip("Seconds waited before start accelerating.")]
        public float waitSecondsBeferoAccelerate;
        [Tooltip("Acceleration in units per second.")]
        public float acceleration;
        [Tooltip("Max velocity.")]
        public float maxVelocity;

        private bool isAccelerating = false;
        private float secondsWaitedBeforeAccelerate = 0;

        private Rigidbody2D thisRigidbody;

        void IBuild.Build(LivingObject livingObject) => thisRigidbody = livingObject.rigidbodyHelper.Rigidbody2D;
        void IInitialize.Initialize() => isAccelerating = false;
        void IMove.Move(float speedMultiplier)
        {
            if (isAccelerating)
            {
                if (thisRigidbody.velocity.magnitude < maxVelocity)
                    thisRigidbody.AddRelativeForce(new Vector2(0, acceleration) * speedMultiplier * Time.deltaTime);
            }
            else
            {
                secondsWaitedBeforeAccelerate += Time.deltaTime;
                isAccelerating = secondsWaitedBeforeAccelerate > waitSecondsBeferoAccelerate;
            }
        }
    }
}