using UnityEngine;

namespace Effects
{
    public class RapidFireRate : Pickupable
    {
        [Header("Configuration")]
        [Tooltip("Fire rate multiplier.")]
        public float fireRateMultiplier;
        [Tooltip("Duration of effect in seconds.")]
        public float durationOfEffect;

        public override void Pickup(Player player)
        {
            player.AddEffect(new RapidFireRateEffect(fireRateMultiplier, durationOfEffect));
            base.Pickup(player);
        }
    }

    public class RapidFireRateEffect : Effect, IStart, IUpdate, IEnd
    {
        public RapidFireRateEffect(float strength, float duration) : base(strength, duration) { }

        public override string Name => "Fire Rate";
        public override bool IsBuff => strength > 0;

        public override bool ReplaceCurrentInstance => true;

        private float initialValue;

        void IStart.OnStart() => initialValue = livingObject.fireRateMultiplier;
        void IUpdate.OnUpdate(float time) => livingObject.fireRateMultiplier = initialValue + strength * Mathf.Pow(duration / maxDuration, .5f);
        void IEnd.OnEnd(bool wasAborted) => livingObject.fireRateMultiplier = initialValue;
    }
}