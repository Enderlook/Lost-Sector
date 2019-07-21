using UnityEngine;

public class HealthPack : Pickupable
{
    [Header("Configuration")]
    [Tooltip("Health restored on pick up.")]
    public int healthRestored;
    public override void Pickup(Player player) => player.TakeHealing(healthRestored);
}