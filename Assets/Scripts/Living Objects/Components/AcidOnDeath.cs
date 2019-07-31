using UnityEngine;

namespace LivingObjectAddons
{
    public class AcidOnDeath : AOEOnDeath
    {
        [Header("Configuration")]
        [Tooltip("Acid damage per tick.")]
        public float acidDamage;
        [Tooltip("Ticks per second.")]
        public float ticksPerSecond;
        [Tooltip("Acid duration.")]
        public float durationOfEffect;

        protected override void AffectTarget(RigidbodyHelper target)
        {
            target.GetComponentInParent<LivingObject>()?.AddEffect(new Effects.AcidEffect(acidDamage, durationOfEffect, ticksPerSecond));
        }
    }
}

namespace Effects
{
    public class AcidEffect : Effect, IUpdate
    {
        private float ticksPerSecond;
        private float cooldown = 0;

        public AcidEffect(float strength, float duration, float ticksPerSecond) : base(strength, duration) => this.ticksPerSecond = ticksPerSecond;

        public override bool ReplaceCurrentInstance => false;

        public override string Name => "Acid";
        public override bool IsBuff => false;

        void IUpdate.OnUpdate(float time)
        {
            if (cooldown <= 0)
            {
                livingObject.TakeDamage(strength * Mathf.Pow(duration / maxDuration, .5f), true);
                cooldown = 1 / ticksPerSecond;
            }
            else
                cooldown -= Time.deltaTime;
        }
    }
}