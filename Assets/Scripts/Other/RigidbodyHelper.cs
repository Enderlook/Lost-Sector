using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IRigidbodyHelperHandler {
    void TakeDamage(float amount);
    float ImpactDamage { get; }
    Sound ImpactSound { get; }
    bool IsImpactDamageRelativeToImpulse { get; }
}

public class RigidbodyHelper : MonoBehaviour
{
    [Header("Setup")]
    [Tooltip("Audio Source component.")]
    public AudioSource audioSource;
    
    private IRigidbodyHelperHandler handler;
    
    /// <summary>
    /// Return Rigidbody2D of the gameObject which has this script.
    /// </summary>
    /// <returns>Rigidbody2D of the RigidbodyHelper's gameObject.</returns>
    public Rigidbody2D GetRigidbody2D()
    {
        return gameObject.GetComponent<Rigidbody2D>();
    }

    public void SetHandler(IRigidbodyHelperHandler handler)
    {
        this.handler = handler;
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

        // Downwards damage doesn't work. Why?
        
        RigidbodyHelper target = collision.gameObject.GetComponent<RigidbodyHelper>();
        if (target != null)
        {
            target.TakeDamage(CalculateDamage(impulse));
        }

        if (audioSource != null && handler.ImpactSound != null)
        {
            handler.ImpactSound.Play(audioSource, collision.relativeVelocity.magnitude);
        }
    }

    /// <summary>
    /// Take damage reducing health or shield.
    /// </summary>
    /// <param name="amount">Amount (positive) of damage received</param>
    public void TakeDamage(float amount)
    {
        handler.TakeDamage(amount);
    }

    private float CalculateDamage(float impulse)
    {
        if (handler.IsImpactDamageRelativeToImpulse)
            return handler.ImpactDamage * impulse;
        else
            return handler.ImpactDamage;
    }

    private void OnValidate()
    {
        if (gameObject.GetComponent<Rigidbody2D>() == null)
            Debug.LogWarning($"Gameobject {gameObject.name} lacks of rigidbody2D component.");
    }
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

    public void Play(AudioSource audioSource, float volumeMultiplier)
    {
        audioSource.pitch = GetPitch();
        audioSource.PlayOneShot(audioClip, GetVolume() * volumeMultiplier);
    }
}
