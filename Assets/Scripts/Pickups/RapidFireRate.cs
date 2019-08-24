using System.Collections.Generic;
using LivingObjectAddons;
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
            player.AddEffect(new RapidFireRateEffect<SimpleWeapon>(fireRateMultiplier, durationOfEffect));
            base.Pickup(player);
        }
    }

    public abstract class WeaponEffect<T> : Effect, IStart where T : Weapon
    {
        public WeaponEffect(float strength, float duration) : base(strength, duration) { }

        protected List<T> weapons = new List<T>();

        public virtual void OnStart()
        {
            foreach (Weapon weapon in livingObject.weapons)
            {
                if (weapon.TryCast(out T w))
                    weapons.Add(w);
            }
        }
    }

    public class RapidFireRateEffect<T> : WeaponEffect<T>, IUpdate where T : Weapon
    {
        public RapidFireRateEffect(float strength = 0, float duration = 0, float initialCharge = 0) : base(strength, duration) => this.initialCharge = initialCharge;

        public override string Name => "Fire Rate";
        public override bool IsBuff => strength > 0;

        public override bool ReplaceCurrentInstance => true;
        private float initialCharge;

        private float Strength => strength * Mathf.Pow(duration / maxDuration, .5f);

        public override void OnStart()
        {
            base.OnStart();
            if (initialCharge > 0)
                weapons?.ForEach(e => e.Recharge(e.TotalCooldown * initialCharge));
        }

        void IUpdate.OnUpdate(float time) => weapons?.ForEach(e => e.Recharge(time * Strength));
    }
}