using UnityEngine;

namespace LivingObjectAddons
{
    public class InitialRotation : MonoBehaviour, IBuild, IInitialize
    {
        [Header("Configuration")]
        [Tooltip("Random rotation between two value.")]
        public FloatRangeTwo rotation;

        private Rigidbody2D thisRigidbody2D;

        void IBuild.Build(LivingObject livingObject) => thisRigidbody2D = livingObject.rigidbodyHelper.GetRigidbody2D();
        void IInitialize.Initialize() => thisRigidbody2D.angularVelocity = (float)rotation;
    }

}