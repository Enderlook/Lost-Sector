using UnityEngine;

namespace LivingObjectAddons
{
    public class Kamikaze : Movement
    {
        [Header("Configuration")]
        [Tooltip("Seconds waited before start accelerating.")]
        public float waitSecondsBeferoAccelerate;
        [Tooltip("Acceleration in units per second.")]
        public float acceleration;

        private bool isAccelerating = false;
        private float secondsWaitedBeforeAccelerate = 0;

        private Rigidbody2D thisRigidbody;

        public override void OnBuild(LivingObject livingObject) => thisRigidbody = livingObject.rigidbodyHelper.GetRigidbody2D();

        public override void Initialize() => isAccelerating = false;

        public override void Move(float speedMultiplier)
        {
            if (isAccelerating)
                thisRigidbody.AddRelativeForce(new Vector2(0, acceleration) * speedMultiplier);
            else
            {
                secondsWaitedBeforeAccelerate += Time.deltaTime;
                isAccelerating = secondsWaitedBeforeAccelerate > waitSecondsBeferoAccelerate;
            }
        }
    }
}