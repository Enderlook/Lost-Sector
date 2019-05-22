using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour, IRigibodyHelperHandler {

    [Header("Configuration")]
    [Tooltip("Damage on impact.")]
    public float damage;

    [Header("Setup")]
    [Tooltip("Impact sound.")]
    public Sound impactSound;
    Sound IImpactSound.ImpactSound {
        get {
            return impactSound;
        }
    }

    [Tooltip("RigibodyHelper script.")]
    public RigidbodyHelper rigidbodyHelper;

    // Should I remove public and add IInterface.etc?
    // Wouldnt't this raise error since SET shouldn't exist?
    //public float ImpactDamage => damage;
    public float ImpactDamage {
        get {
            return damage;
        }
    }
    
    // Is this against the Interface Segregation Principle?
    public void TakeDamage(float amount) {
        /* We are a bullet, we don't have HP... yet */
        Destroy(gameObject);
    }

    private void Start()
    {
        rigidbodyHelper.SetHandler(this);
    }
}
