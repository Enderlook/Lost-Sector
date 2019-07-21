using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Pickupable : MonoBehaviour, ICanBePickedUp
{
    [Header("Configuration")]
    [Tooltip("Initial impulse.")]
    public Vector2RangeTwo impulse;

    private Rigidbody2D thisRigidbody2D;

    Rigidbody2D ICanBePickedUp.Rigidbody2D => thisRigidbody2D;

    private void Start()
    {
        transform.parent = Global.pickupsParent;
        thisRigidbody2D = gameObject.GetComponent<Rigidbody2D>();
        thisRigidbody2D.AddRelativeForce((Vector2)impulse * thisRigidbody2D.mass);
    }
    protected virtual void OnValidate()
    {
        Rigidbody2D rigidbody2D = GetComponent<Rigidbody2D>();
        if (rigidbody2D == null)
            Debug.LogWarning($"Game object {gameObject.name} lacks of an Rigidbody2D Component.");
    }

    public abstract void Pickup(LivingObject livingObject);
}
