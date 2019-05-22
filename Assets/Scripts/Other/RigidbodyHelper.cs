using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageTaker {
    void TakeDamage(float amount);
}

public interface IImpactDamage {
    float ImpactDamage { get; }
}

public interface IImpactSound {
    Sound ImpactSound { get; }
}

public interface IRigibodyHelperHandler : IDamageTaker, IImpactDamage, IImpactSound { }

public class RigidbodyHelper : MonoBehaviour
{
    [Header("Setup")]
    /*[Tooltip("Living Object script.")]
    public LivingObject livingObject;*/
    [Tooltip("Audio Source component.")]
    public AudioSource audioSource;

    /*public IDamageTaker damageTaker;
    public IImpactDamage impactDamage;
    public IImpactSound impactSound;*/

    private /* public RigibodyHelperHandler*/ IRigibodyHelperHandler handler;

    public void SetHandler(IRigibodyHelperHandler handler)
    {
        this.handler = handler;
    }

    /// <summary>
    /// Return Rigidbody2D of the gameobject which has this script.
    /// </summary>
    /// <returns></returns>
    public Rigidbody2D GetRigidbody2D()
    {
        return gameObject.GetComponent<Rigidbody2D>();
    }

    //private Rigidbody2D thisRigidbody2D;

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

    /*private void Start()
    {
        thisRigidbody2D = gameObject.GetComponent<Rigidbody2D>();
    }*/

    /*private void OnValidate()
    {
        if (gameObject.GetComponent<Rigidbody2D>() == null)
            Debug.LogWarning($"Gameobject {gameObject.name} lacks of Rigibody2D component");
    }*/

    /* TODO:
     * https://forum.unity.com/threads/exposing-fields-with-interface-type-c-solved.49524/
     * https://forum.unity.com/threads/c-interface-wont-show-in-inspector.383886/
     * https://forum.unity.com/threads/understanding-iserializationcallbackreceiver.383757/
     */

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // https://forum.unity.com/threads/getting-impact-force-not-just-velocity.23746/
        float impulse = 0f;
        foreach (ContactPoint2D contactPoint in collision.contacts)
        {
            impulse += contactPoint.normalImpulse;
        }

        // Downwards damage doesn't work. Why?
        
        RigidbodyHelper target = collision.gameObject.GetComponent<RigidbodyHelper>();
        if (target != null)
        {
            target.TakeDamage(/*livingObject.relativeImpactDamage*/ /*impactDamage*/handler.ImpactDamage * impulse);
            //target.livingObject.TakeDamage(livingObject.relativeImpactDamage * impulse);
        }

        if (audioSource != null && /*livingObject.impactSound*/ /*impactSound*/handler.ImpactSound != null)
        {
            /*livingObject.impactSound*/ /*impactSound*/handler.ImpactSound.Play(audioSource, collision.relativeVelocity.magnitude);
        }
    }

    public void TakeDamage(float amount)
    {
        /*livingObject*/ /*damageTaker*/handler.TakeDamage(amount);
    }
}

//[System.Serializable]
/*public class RigibodyHelperHandler : IImpactDamage, IImpactSound, IDamageTaker {
    //[Tooltip("Living Object script.")]
    public LivingObject livingObject;


    public void TakeDamage(float amount)
    {
        ((IDamageTaker)livingObject).TakeDamage(amount);
    }

    public Sound ImpactSound {
        get {
            return ((IImpactSound)livingObject).ImpactSound;
        }
    }

    public float ImpactDamage {
        get {
            return ((IImpactDamage)livingObject).ImpactDamage;
        }
    }
}*/

/*public class RigibodyHelperHandler2 {
    public IImpactDamage ImpactDamage;
    public IImpactSound ImpactSound;
    public IDamageTaker DamageTaker;

    public RigibodyHelperHandler2(IImpactDamage impactDamage, IImpactSound impactSound, IDamageTaker damageTaker)
    {
        ImpactDamage = impactDamage;
        ImpactSound = impactSound;
        DamageTaker = damageTaker;
    }
}*/


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
