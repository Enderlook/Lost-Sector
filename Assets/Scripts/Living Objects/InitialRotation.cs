using UnityEngine;

namespace LivingObjectAddons
{
    public class InitialRotation : OnInitialize
    {
        [Header("Configuration")]
        [Tooltip("Random rotation between two value.")]
        public FloatRangeTwo rotation;

        private RigidbodyHelper rigidbodyHelper;

        public override void OnStart(LivingObject livingObject) => rigidbodyHelper = livingObject.rigidbodyHelper;

        public override void Initialize() => rigidbodyHelper.GetRigidbody2D().angularVelocity = (float)rotation;
    }
}