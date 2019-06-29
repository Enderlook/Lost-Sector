using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPack : Pickupable
{
    [Header("Configuration")]
    [Tooltip("Health restored on pick up.")]
    public int healthRestored;
    [Tooltip("Initial impulse.")]
    public Vector2RangeTwo impulse;

    private Rigidbody2D thisRigidbody2D;
    public override Rigidbody2D Rigidbody2D => thisRigidbody2D;
    public override bool ShouldBeDestroyedOnPickup => true;

    private void Start()
    {
        thisRigidbody2D = gameObject.GetComponent<Rigidbody2D>();
        thisRigidbody2D.AddRelativeForce((Vector2)impulse * thisRigidbody2D.mass);
    }

    /// <summary>
    /// Heal the <paramref name="livingObject"/> who pickup it by <see cref="healthRestored"/>.
    /// </summary>
    /// <param name="livingObject"><see cref="LivingObject"/> to heal.</param>
    public override void Pickup(LivingObject livingObject) => livingObject.TakeHealing(healthRestored);

    private void OnValidate()
    {
        Rigidbody2D rigidbody2D = GetComponent<Rigidbody2D>();
        if (rigidbody2D == null)
            Debug.LogWarning($"Game object {gameObject.name} lacks of an Rigidbody2D Component.");
    }
}
