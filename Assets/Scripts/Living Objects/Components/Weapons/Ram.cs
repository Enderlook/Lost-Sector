using UnityEngine;

namespace LivingObjectAddons
{
    public class Ram : MonoBehaviour, IMelee, IImpactSound
    {
        [Header("Configuration")]
        [Tooltip("Damage on hit.")]
        public float damageOnHit = 1;
        float IMelee.ImpactDamage { get => damageOnHit; set => damageOnHit = value; }

        [Tooltip("Relative damage on impact based on force.")]
        public bool isDamageRelativeToImpulse;
        bool IMelee.IsImpactDamageRelativeToImpulse { get => isDamageRelativeToImpulse; set => isDamageRelativeToImpulse = value; }

        [Tooltip("Should spawn floating damage text on the enemy on collision?")]
        public bool shouldDisplayDamage;
        bool IShouldDisplayDamage.ShouldDisplayDamage { get => shouldDisplayDamage; set => shouldDisplayDamage = value; }

        [Header("Setup")]
        [Tooltip("Impact sound.")]
        public Sound impactSound;
        Sound IImpactSound.ImpactSound => impactSound;
    }

    public interface IMelee : IShouldDisplayDamage
    {
        /// <summary>
        /// Damage produced on collision impact.<br/>
        /// </summary>
        float ImpactDamage { get; set; }
        /// <summary>
        /// Whenever <seealso cref="ImpactDamage"/> should or not be calculated taking into account the collision impulse.
        /// </summary>
        bool IsImpactDamageRelativeToImpulse { get; set; }
    }
}