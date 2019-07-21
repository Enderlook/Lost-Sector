using UnityEngine;

namespace LivingObjectAddons
{
    public class Ram : MonoBehaviour, IMelee
    {
        [Header("Configuration")]
        [Tooltip("Damage on hit.")]
        public float damageOnHit = 1;
        float IMelee.ImpactDamage => damageOnHit;

        [Tooltip("Relative damage on impact based on force.")]
        public bool isDamageRelativeToImpulse;
        bool IMelee.IsImpactDamageRelativeToImpulse => isDamageRelativeToImpulse;

        [Tooltip("Should spawn floating damage text on the enemy on collision?")]
        public bool shouldDisplayDamage;
        bool IShouldDisplayDamage.ShouldDisplayDamage => shouldDisplayDamage;

        [Header("Setup")]
        [Tooltip("Impact sound.")]
        public Sound impactSound;
        Sound IImpactSound.ImpactSound => impactSound;
    }

    public interface IMelee : IImpactSound, IShouldDisplayDamage
    {
        /// <summary>
        /// Damage produced on collision impact.<br/>
        /// </summary>
        float ImpactDamage { get; }
        /// <summary>
        /// Whenever <seealso cref="IMelee.ImpactDamage"/> should or not be calculated taking into account the collision impulse.
        /// </summary>
        bool IsImpactDamageRelativeToImpulse { get; }
    }
}