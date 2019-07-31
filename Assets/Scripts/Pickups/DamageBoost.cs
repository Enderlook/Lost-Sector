using UnityEngine;

namespace Effects
{
    public class DamageBoost : Pickupable
    {
        [Header("Configuration")]
        [Tooltip("Damage multiplier.")]
        public float damageMultiplier;
        [Tooltip("Duration of effect in seconds.")]
        public float durationOfEffect;

        public override void Pickup(Player player)
        {
            player.AddEffect(new DamageBoostEffect(damageMultiplier, durationOfEffect));
            base.Pickup(player);
        }
    }

    public class DamageBoostEffect : Effect, IStart, IUpdate, IEnd
    {
        public DamageBoostEffect(float strength, float duration) : base(strength, duration) { }

        public override string Name => "Damage";
        public override bool IsBuff => strength > 0;

        public override bool ReplaceCurrentInstance => true;

        private float initialValue;

        void IStart.OnStart() => initialValue = livingObject.weaponStrengthMultiplier;
        void IUpdate.OnUpdate(float time) => livingObject.weaponStrengthMultiplier = initialValue + strength * Mathf.Pow(duration / maxDuration, .5f);
        void IEnd.OnEnd(bool wasAborted) => livingObject.weaponStrengthMultiplier = initialValue;
    }
}