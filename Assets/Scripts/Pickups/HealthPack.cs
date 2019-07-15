using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPack : Pickupable
{
    [Header("Configuration")]
    [Tooltip("Health restored on pick up.")]
    public int healthRestored;
    public override void Pickup(LivingObject livingObject) => livingObject.TakeHealing(healthRestored);
}