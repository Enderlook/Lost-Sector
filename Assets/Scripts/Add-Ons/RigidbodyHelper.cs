using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RigidbodyHelper : MonoBehaviour
{
    [Header("Setup")]
    [Tooltip("Audio Source component.")]
    public AudioSource audioSource;
    
    private IRigidbodyHelperConfiguration entity;

    /// <summary>
    /// Set the configuration on the RigidbodyHelper in order to use it. Mandatory.
    /// </summary>
    /// <param name="handler">Configuration of the RigidbodyHelper</param>
    public void SetProperties(IRigidbodyHelperConfiguration configuration)
    {
        entity = configuration;
    }
        
    /// <summary>
    /// Return Rigidbody2D of the gameObject which has this script.
    /// </summary>
    /// <returns>Rigidbody2D of the RigidbodyHelper's gameObject.</returns>
    public Rigidbody2D GetRigidbody2D()
    {
        return gameObject.GetComponent<Rigidbody2D>();
    }

    /// <summary>
    /// Current position.
    /// </summary>
    public Vector3 Position
    {
        get
        {
            return transform.position;
        }
    }

    /* TODO:
     * https://forum.unity.com/threads/exposing-fields-with-interface-type-c-solved.49524/
     * https://forum.unity.com/threads/c-interface-wont-show-in-inspector.383886/
     * https://forum.unity.com/threads/understanding-iserializationcallbackreceiver.383757/
     */

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // https://forum.unity.com/threads/getting-impact-force-not-just-velocity.23746/
        // Calculate impact force of the collision
        float impulse = 0f;
        foreach (ContactPoint2D contactPoint in collision.contacts)
        {
            impulse += contactPoint.normalImpulse;
        }
        // Kinetic energy? https://www.reddit.com/r/Unity2D/comments/2kil7j/how_to_determine_force_of_impact_when_two_things/
        //float impulse = Mathf.Abs(0.5f * collision.relativeVelocity.sqrMagnitude * (collision.collider.attachedRigidbody.mass + collision.otherCollider.attachedRigidbody.mass));
        //float impulse = Mathf.Abs(0.5f * collision.rigidbody.mass * collision.rigidbody.velocity.sqrMagnitude - 0.5f * collision.otherRigidbody.mass * collision.otherRigidbody.velocity.sqrMagnitude);
        // TODO: READ https://forum.unity.com/threads/how-to-calculate-a-rigidbodys-mass-normalized-energy-for-sleepthreshold.311941/
        // TODO: Add fake mass to player kinematic Rigidbody

        // Downwards damage doesn't work. Why?
        RigidbodyHelper target = collision.gameObject.GetComponent<RigidbodyHelper>();
        if (target != null)
        {
            target.TakeDamage(CalculateDamage(impulse));
        }

        if (audioSource != null && entity.ImpactSound != null)
        {
            entity.ImpactSound.Play(audioSource, collision.relativeVelocity.magnitude);
        }
    }

    /// <summary>
    /// Take damage reducing health or shield.
    /// </summary>
    /// <param name="amount">Amount (positive) of damage received</param>
    public void TakeDamage(float amount)
    {
        entity.TakeDamage(amount);
    }

    private float CalculateDamage(float impulse)
    {
        if (entity.IsImpactDamageRelativeToImpulse)
            return entity.ImpactDamage * impulse;
        else
            return entity.ImpactDamage;
    }

    private void OnValidate()
    {
        if (gameObject.GetComponent<Rigidbody2D>() == null)
            Debug.LogWarning($"Gameobject {gameObject.name} lacks of rigidbody2D component.");
    }
}

public interface IRigidbodyHelperConfiguration {
    void TakeDamage(float amount);
    float ImpactDamage { get; }
    Sound ImpactSound { get; }
    bool IsImpactDamageRelativeToImpulse { get; }
}

[System.Serializable]
public class Sound {
    [Tooltip("Sound clip.")]
    public AudioClip audioClip;

    [Tooltip("Volume. Use range size 1 to avoid random volume.")]
    public float[] volume = new float[2] { 1, 1 };

    [Tooltip("Pitch. Use range size 1 to avoid random volume.")]
    public float[] pitch = new float[2] { 1, 1 };

    private float GetVolume()
    {
        if (volume.Length > 1)
            return Random.Range(volume[0], volume[1]);
        else
            return volume[0];
    }

    private float GetPitch()
    {
        if (pitch.Length > 1)
            return Random.Range(pitch[0], pitch[1]);
        else
            return pitch[0];
    }

    /// <summary>
    /// Play the sound on the specified AudioSource
    /// </summary>
    /// <param name="audioSource">AudioSource where the sound will be played.</param>
    /// <param name="volumeMultiplier">Volume of the sound, from 0 to 1.</param>
    public void Play(AudioSource audioSource, float volumeMultiplier)
    {
        audioSource.pitch = GetPitch();
        audioSource.PlayOneShot(audioClip, GetVolume() * volumeMultiplier);
    }
}
