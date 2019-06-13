using UnityEngine;

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
    public void SetProperties(IRigidbodyHelperConfiguration configuration)
    {
        entity = configuration;
    }

    /// <summary>
    /// Return <see cref="Rigidbody"/> of the gameObject which has this script.
    /// </summary>
    /// <returns><see cref="Rigidbody"/> of this <see cref="RigidbodyHelper"/>'s <see cref="gameObject"/>.</returns>
    /// <seealso cref="RigidbodyHelper"/>.
    public Rigidbody2D GetRigidbody2D()
    {
        return gameObject.GetComponent<Rigidbody2D>();
    }

    /// <summary>
    /// Current position.
    /// </summary>
    public Vector3 Position {
        get {
            return transform.position;
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
            target.TakeDamage(CalculateDamage(impulse), ShouldDisplayDamage());
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
    /// <param name="displayText">Whenever the damage taken must be shown in a floating text.</param>
    /// <seealso cref="ShouldDisplayDamage()"/>
    /// <seealso cref="CalculateDamage(float impulse)"/>
    public void TakeDamage(float amount, bool displayText = false)
    {
        entity.TakeDamage(amount, displayText);
    }

    /// <summary>
    /// Whenever if the opposite <seealso cref="RigidbodyHelper"/> should display or not the floating text when receive damage..<br/>
    /// This property is passed as <code>displayText</code> parameter of <seealso cref=">TakeDamage(float amount, bool displayText = false)"/> in the opposite <seealso cref="RigidbodyHelper"/>.
    /// </summary>
    /// <returns>If damage should be displayed using a <seealso cref="FloatingText"/>.</returns>
    /// <seealso cref="IRigidbodyHelperConfiguration.ShouldDisplayDamage"/>
    public bool ShouldDisplayDamage()
    {
        return entity.ShouldDisplayDamage;
    }

    /// <summary>
    /// Calculate damage produced taking into account the <paramref name="impulse"/> of the collision if <see cref="IRigidbodyHelperConfiguration.IsImpactDamageRelativeToImpulse"/> is <see langword="true"/>. On false <see cref="IRigidbodyHelperConfiguration.ImpactDamage"/> is return.
    /// </summary>
    /// <param name="impulse">Impulse or force produced on <see cref="Collision2D"/>.</param>
    /// <returns>Damage to inflict.</returns>
    /// <seealso cref="TakeDamage(float amount, bool displayText = false)"/>
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

public interface IRigidbodyHelperConfiguration : IShouldDisplayDamage
{
    /// <summary>
    /// Take damage reducing its HP.
    /// </summary>
    /// <param name="amount">Amount of HP lost. Must be positive.</param>
    /// <param name="displayText">Whenever the damage taken must be shown in a floating text.</param>
    void TakeDamage(float amount, bool displayText = false);
    /// <summary>
    /// Damage produced on collision impact.<br/>
    /// If <seealso cref="IRigidbodyHelperConfiguration.IsImpactDamageRelativeToImpulse"/> is <see langword="true"/>, it's your responsibility to calculate damage taking into account impulse.
    /// </summary>
    /// <seealso cref="RigidbodyHelper.OnCollisionEnter2D(Collision2D)"/>
    /// <seealso cref="RigidbodyHelper.CalculateDamage(float)"/>
    float ImpactDamage { get; }
    /// <summary>
    /// Sound played on collision.
    /// </summary>
    Sound ImpactSound { get; }
    /// <summary>
    /// Whenever <seealso cref="IRigidbodyHelperConfiguration.ImpactDamage"/> should or not be calculated taking into account the collision impulse.
    /// </summary>
    /// <seealso cref="RigidbodyHelper.OnCollisionEnter2D(Collision2D)"/>
    /// <seealso cref="RigidbodyHelper.CalculateDamage(float)"/>
    bool IsImpactDamageRelativeToImpulse { get; }

}

public interface IShouldDisplayDamage
{
    /// <summary>
    /// Whenever if the opposite <seealso cref="RigidbodyHelper"/> should display or not the floating text when receive damage.<br/>
    /// This property is passed as <code>displayText</code> parameter of <seealso cref=">TakeDamage(float amount, bool displayText = false)"/> in the opposite <seealso cref="RigidbodyHelper"/>.
    /// </summary>
    /// <seealso cref="RigidbodyHelper.ShouldDisplayDamage()"/>
    bool ShouldDisplayDamage { get; }
}