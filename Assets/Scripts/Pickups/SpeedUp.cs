using UnityEngine;

namespace Effects
{
    public class SpeedUp : Pickupable
    {
        [Header("Configuration")]
        [Tooltip("Speed multiplier.")]
        public float speedMultiplier;
        [Tooltip("Duration of effect in seconds.")]
        public float durationOfEffect;

        public override void Pickup(Player player)
        {
            player.AddEffect(new SpeedEffect(speedMultiplier, durationOfEffect,
                (strength, maxDuration, duration, initialValue) => initialValue + strength * Mathf.Pow(duration / maxDuration, .5f)
            ));
            base.Pickup(player);
        }
    }

    public class SpeedEffect : Effect, IStart, IUpdate, IEnd
    {
        public SpeedEffect(float strength, float duration, StrengthCalculate strengthCalculate) : base(strength, duration)
        {
            this.strengthCalculate = strengthCalculate;
        }

        public override string Name => "Speed";
        public override bool IsBuff => strengthCalculate(strength, 1, 1, 1) > 1;

        public override bool ReplaceCurrentInstance => true;

        private float initialValue;

        private StrengthCalculate strengthCalculate;

        void IStart.OnStart() => initialValue = livingObject.SpeedMultiplier;
        void IUpdate.OnUpdate(float time) => livingObject.SpeedMultiplier = strengthCalculate(strength, maxDuration, duration, initialValue);
        void IEnd.OnEnd(bool wasAborted) => livingObject.SpeedMultiplier = initialValue;
    }

    public delegate float StrengthCalculate(float strength, float maxDuration, float duration, float initialValue);
}