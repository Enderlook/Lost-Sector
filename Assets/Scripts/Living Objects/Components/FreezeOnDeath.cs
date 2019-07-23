using UnityEngine;

namespace LivingObjectAddons
{
    public class FreezeOnDeath : AOEOnDeath
    {
        [Header("Configuration")]
        [Tooltip("Slowdown strength.")]
        public float speedMultiplier;
        [Tooltip("Slowdown duration.")]
        public float durationOfEffect;

        protected override void AffectTarget(RigidbodyHelper target)
        {
            target.GetComponentInParent<LivingObject>()?.AddEffect(new SpeedEffect(speedMultiplier, durationOfEffect,
                (strength, maxDuration, duration, initialValue) => initialValue / (1 + strength * Mathf.Pow(duration / maxDuration, .5f))
            ));
        }
    }
}