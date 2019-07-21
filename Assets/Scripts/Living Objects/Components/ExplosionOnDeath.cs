using UnityEngine;

namespace LivingObjectAddons
{
    public class ExplosionOnDeath : AOEOnDeath
    {
        [Header("Configuration")]
        [Tooltip("Explosion damage.")]
        public float explosionDamage;

        protected override void AffectTarget(RigidbodyHelper target)
        {
            // TODO: Add explosion damage falls-off over distance
            target.TakeDamage(explosionDamage);
        }

    }
}