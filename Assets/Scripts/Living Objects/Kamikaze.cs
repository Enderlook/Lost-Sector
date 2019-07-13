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

        private RigidbodyHelper rigidbodyHelper;

        public override void OnStart(LivingObject livingObject) => rigidbodyHelper = livingObject.rigidbodyHelper;

        public override void Initialize() => isAccelerating = false;

        public override void Move()
        {
            if (isAccelerating)
                rigidbodyHelper.GetRigidbody2D().AddRelativeForce(new Vector2(0, acceleration));
            else
            {
                secondsWaitedBeforeAccelerate += Time.deltaTime;
                isAccelerating = secondsWaitedBeforeAccelerate > waitSecondsBeferoAccelerate;
            }
        }
    }
}