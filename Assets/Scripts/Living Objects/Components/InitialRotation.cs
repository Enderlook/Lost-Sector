using UnityEngine;

namespace LivingObjectAddons
{
    public class InitialRotation : OnInitialize
    {
        [Header("Configuration")]
        [Tooltip("Random rotation between two value.")]
        public FloatRangeTwo rotation;

        private Rigidbody2D thisRigidbody2D;

        public override void OnBuild(LivingObject livingObject) => thisRigidbody2D = livingObject.rigidbodyHelper.GetRigidbody2D();

        public override void Initialize() => thisRigidbody2D.angularVelocity = (float)rotation;
    }

}