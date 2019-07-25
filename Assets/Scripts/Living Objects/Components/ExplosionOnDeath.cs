using UnityEngine;

namespace LivingObjectAddons
{
    public class ExplosionOnDeath : AOEOnDeath
    {
        [Header("Configuration")]
        [Tooltip("Explosion damage.")]
        public float explosionDamage;

        // TODO: Add explosion damage falls-off over distance
        protected override void AffectTarget(RigidbodyHelper target) => target.TakeDamage(explosionDamage);

    }
}