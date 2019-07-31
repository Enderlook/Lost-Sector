using UnityEngine;

namespace Effects
{
    public class ExplosiveBurstCharge : Pickupable
    {
        [Header("Configuration")]
        [Tooltip("Charge in percent.")]
        [Range(0, 1)]
        public float percentCharge;

        public override void Pickup(Player player)
        {
            player.AddEffect(new RapidFireRateEffect<LivingObjectAddons.ExplosiveBurstWeapon>(initialCharge: percentCharge));
            base.Pickup(player);
        }
    }

}