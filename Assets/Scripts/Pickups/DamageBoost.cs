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

    public class DamageBoostEffect : WeaponEffect<LivingObjectAddons.Weapon>, IUpdate, IEnd
    {
        public DamageBoostEffect(float strength, float duration) : base(strength, duration) { }

        public override string Name => "Damage";
        public override bool IsBuff => strength > 0;

        public override bool ReplaceCurrentInstance => true;

        private float[] initialValues;
        private float Strength => strength * Mathf.Pow(duration / maxDuration, .5f);

        public override void OnStart()
        {
            base.OnStart();
            initialValues = new float[weapons.Count];
            for (int i = 0; i < initialValues.Length; i++)
            {
                initialValues[i] = weapons[i].strengthMultiplier;
            }
        }

        void IUpdate.OnUpdate(float time)
        {
            for (int i = 0; i < weapons.Count; i++)
            {
                weapons[i].strengthMultiplier = initialValues[i] + Strength;
            }
        }

        void IEnd.OnEnd(bool wasAborted)
        {
            for (int i = 0; i < weapons.Count; i++)
            {
                weapons[i].strengthMultiplier = initialValues[i];
            }
        }
    }
}