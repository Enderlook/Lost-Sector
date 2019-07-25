using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class RigidbodyHelper : MonoBehaviour
{
    [Header("Setup")]
    [Tooltip("Audio Source component.")]
    public AudioSource audioSource;

    /// <summary>
    /// Configuration of the owner of this script.
    /// </summary>
    private IRigidbodyHelperConfiguration entity;

    /// <summary>
    /// Set the configuration on the RigidbodyHelper in order to use it. Mandatory.
    /// </summary>
    /// <param name="handler">Configuration of this <see cref="RigidbodyHelper"/></param>
    /// <seealso cref="RigidbodyHelper"/>
    public void SetProperties(IRigidbodyHelperConfiguration configuration) => entity = configuration;

    /// <summary>
    /// Return <see cref="Rigidbody"/> of the gameObject which has this script.
    /// </summary>
    /// <returns><see cref="Rigidbody"/> of this <see cref="RigidbodyHelper"/>'s <see cref="gameObject"/>.</returns>
    /// <seealso cref="RigidbodyHelper"/>.
    public Rigidbody2D Rigidbody2D {
        get {
            if (thisRigidbody2D == null)
                thisRigidbody2D = gameObject.GetComponent<Rigidbody2D>();
            return thisRigidbody2D;
        }
        set => thisRigidbody2D = value;
    }
    private Rigidbody2D thisRigidbody2D;

    // https://answers.unity.com/questions/711749/slow-time-for-a-single-rigid-body.html
    private float speedMultiplier = 1;
    /// <summary>
    /// Change this value affect the speed of the current and future <seealso cref="Rigidbody2D"/>.
    /// </summary>
    public float SpeedMultiplier {
        get => speedMultiplier;
        set {
            Rigidbody2D.mass *= speedMultiplier;
            Rigidbody2D.velocity /= speedMultiplier;
            Rigidbody2D.angularVelocity /= speedMultiplier;
            speedMultiplier = value;
            Rigidbody2D.mass /= speedMultiplier;
            Rigidbody2D.velocity *= speedMultiplier;
            Rigidbody2D.angularVelocity *= speedMultiplier;
        }
    }

    /* TODO:
     * https://forum.unity.com/threads/exposing-fields-with-interface-type-c-solved.49524/
     * https://forum.unity.com/threads/c-interface-wont-show-in-inspector.383886/
     * https://forum.unity.com/threads/understanding-iserializationcallbackreceiver.383757/
     */

    /// <summary>
    /// Calculate damage produced on collision, play impact sound and spawn floating text whenever it should.
    /// </summary>
    /// <param name="collision">Unity <see cref="Collision2D"/>.</param>
    private void OnCollisionEnter2D(Collision2D collision)
    {        
        RigidbodyHelper target = collision.gameObject.GetComponent<RigidbodyHelper>();
        if (target != null)
            target.TakeDamage(CalculateDamage(collision), ShouldDisplayDamage());

        if (audioSource != null)
        {
            IImpactSound impactSound = entity as IImpactSound;
            if (impactSound != null)
                impactSound.ImpactSound.Play(audioSource, collision.relativeVelocity.magnitude);
        }
    }

    /// <summary>
    /// Take damage reducing health or shield.
    /// </summary>
    /// <param name="amount">Amount (positive) of damage received</param>
    /// <param name="displayText">Whenever the damage taken must be shown in a floating text.</param>
    /// <seealso cref="ShouldDisplayDamage()"/>
    /// <seealso cref="CalculateDamage(float impulse)"/>
    public void TakeDamage(float amount, bool displayText = false) => entity.TakeDamage(amount, displayText);

    /// <summary>
    /// Whenever if the opposite <seealso cref="RigidbodyHelper"/> should display or not the floating text when receive damage.<br/>
    /// This property is passed as <code>displayText</code> parameter of <seealso cref=">TakeDamage(float amount, bool displayText = false)"/> in the opposite <seealso cref="RigidbodyHelper"/>.
    /// </summary>
    /// <returns>If damage should be displayed using a <seealso cref="FloatingText"/>.</returns>
    /// <seealso cref="IRigidbodyHelperConfiguration.ShouldDisplayDamage"/>
    public bool ShouldDisplayDamage() => entity.Melee.ShouldDisplayDamage;

    /// <summary>
    /// Calculate damage produced taking into account the <paramref name="impulse"/> of the collision if <see cref="IRigidbodyHelperConfiguration.IsImpactDamageRelativeToImpulse"/> is <see langword="true"/>. On false <see cref="IRigidbodyHelperConfiguration.ImpactDamage"/> is return.
    /// </summary>
    /// <param name="collision">Collision to calculate impact force.</param>
    /// <returns>Damage to inflict.</returns>
    /// <seealso cref="TakeDamage(float amount, bool displayText = false)"/>
    private float CalculateDamage(Collision2D collision)
    {
        if (entity.Melee.IsImpactDamageRelativeToImpulse)
        {
            // https://forum.unity.com/threads/getting-impact-force-not-just-velocity.23746/#post-784422
            float force = 0;
            foreach (ContactPoint2D contact in collision.contacts)
            {
                force += Vector3.Dot(contact.normal, collision.relativeVelocity);
            }
            force *= Rigidbody2D.mass;
            force = Mathf.Abs(force);

            return entity.Melee.ImpactDamage * force;
        }
        else
            return entity.Melee.ImpactDamage;
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (gameObject.GetComponent<Rigidbody2D>() == null)
            Debug.LogWarning($"Gameobject {gameObject.name} lacks of rigidbody2D component.");
    }
#endif
}

public interface IRigidbodyHelperConfiguration
{
    /// <summary>
    /// Take damage reducing its HP.
    /// </summary>
    /// <param name="amount">Amount of HP lost. Must be positive.</param>
    /// <param name="displayText">Whenever the damage taken must be shown in a floating text.</param>
    void TakeDamage(float amount, bool displayText = false);

    LivingObjectAddons.IMelee Melee { get; }
}

public interface IShouldDisplayDamage
{
    /// <summary>
    /// Whenever if the opposite <seealso cref="RigidbodyHelper"/> should display or not the floating text when receive damage.<br/>
    /// This property is passed as <code>displayText</code> parameter of <seealso cref=">TakeDamage(float amount, bool displayText = false)"/> in the opposite <seealso cref="RigidbodyHelper"/>.
    /// </summary>
    /// <seealso cref="RigidbodyHelper.ShouldDisplayDamage()"/>
    bool ShouldDisplayDamage { get; set; }
}

public interface IImpactSound
{
    /// <summary>
    /// Sound played on collision.
    /// </summary>
    Sound ImpactSound { get; }
}