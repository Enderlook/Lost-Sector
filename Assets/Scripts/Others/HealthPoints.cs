using UnityEngine;

[System.Serializable]
public class HealthPoints
{
    [Header("Main Configuration")]
    [Tooltip("Maximum Current.")]
    public float startingMax = 100;
    private float _max;
    /// <summary>
    /// Maximum amount. <see cref="Current"/> can't be greater than this value.<br/>
    /// If changes, <see cref="healthBar"/> will be updated using <seealso cref="HealthBar.UpdateValues(float health, float maxHealth)"/>.<br/>
    /// If 0, <see cref="Die()"/> is called.
    /// </summary>
    /// <seealso cref="Current"/>
    public float Max {
        get => _max;
        set {
            _max = value;
            bar.UpdateValues(Current, Max);
            if (Max <= 0 && die != null) die();
        }
    }
    [Tooltip("Starting Current. Set -1 to use Max value.")]
    public float startingCurrent = -1;
    private float _current;
    /// <summary>
    /// Current amount. It can't be greater than <see cref="MaxCurrent"/><br/>
    /// If changes, <see cref="Bar"/> will be updated using <seealso cref="HealthBar.UpdateValues(float health, float maxHealth)"/>.<br/>
    /// If 0, <see cref="Die()"/> is called.
    /// </summary>
    /// <seealso cref="Max"/>
    public float Current {
        get => _current;
        set {
            _current = value;
            bar.UpdateValues(Current, Max);
            if (Current <= 0 && die != null) die();
        }
    }

    [Tooltip("Health bar script.")]
    public HealthBar bar;

    [Header("Regeneration")]
    [Tooltip("Does regenerate?")]
    public bool canRegenerate;
    [Tooltip("Regeneration rate (points per second).")]
    public float regenerateRate = 10;
    [Tooltip("Amount of time in seconds after receive damage in order to start regenerating.")]
    public float regenerationDelay = 3f;
    private float currentRegenerationDelay = 0f;
    [Tooltip("Sound while regenerate.")]
    public Playlist playlist;
    [Tooltip("Audio Source used to play sound.")]
    public AudioSource audioSource;

    public delegate void Die(bool suicide = false);
    private Die die;

    /// <summary>
    /// Ration between <see cref="Current"/> and <see cref="Max"/>.
    /// </summary>
    public float Ratio => Current / Max;

    /// <summary>
    /// Whenever the health bar is showed or hidden.
    /// </summary>
    public bool IsVisible {
        get => bar.IsVisible;
        set => bar.IsVisible = value;
    }

    /// <summary>
    /// Set the die method called when <see cref="Current"/> or <see cref="Max"/> are 0.
    /// </summary>
    /// <param name="Die">Method to call.</param>
    public void SetDie(Die die) => this.die = die;

    /// <summary>
    /// Initializes the bar's Currents, setting the fill of the main bar and returning the current value.
    /// If <see cref="startingCurrent"/> is -1, <see cref="startingMaximumCurrent"/> will be used instead to fill the bar.
    /// </summary>
    public void Initialize()
    {
        float current = startingCurrent == -1 ? startingMax : startingCurrent;
        bar.ManualUpdate(startingMax, current);
        Current = current;
        Max = startingMax;
    }

    /// <summary>
    /// Changes the value of <see cref="Current"/> by <paramref name="amount"/>, and clamp values to 0 and <see cref="Max"/> if <paramref name="allowUnderflow"/> and <paramref name="allowOverflow"/> are <see langword="false"/>, respectively.
    /// </summary>
    /// <param name="amount">Amount to change <see cref="Current"/></param>
    /// <param name="allowOverflow">Whenever <paramref name="amount"/> can increase <see cref="Current"/> over <see cref="Max"/> or not.</param>
    /// <param name="allowUnderflow">Whenever <paramref name="amount"/> can decrease <see cref="Current"/> below 0 or not.</param>
    /// <returns>Amount above <see cref="Max"/> or below 0 if they are allowed. If there isn't overflow nor underflow (or they weren't allowed) it will return 0.</br>
    /// Be warned that underflow below 0 is returned as negative numbers.</returns>
    private float ChangeValue(float amount, bool allowOverflow = false, bool allowUnderflow = false)
    {
        Current += amount;
        float flow = 0;
        if (Current > Max && !allowOverflow)
        {
            flow = Max - Current;
            Current = Max;
        }
        else if (Current < 0 && !allowUnderflow)
        {
            flow = Current;
            Current = 0;
        }
        return flow;
    }

    /// <summary>
    /// Reduce <see cref="Current"/> by <paramref name="amount"/>.
    /// </summary>
    /// <param name="amount">Amount to reduce <see cref="Current"/>.</param>
    /// <param name="allowUnderflow">Whenever <see cref="Current"/> could reach negative values or not.</param>
    /// <returns><c>remaining</c>: Amount clamped below 0. <c>taken</c>: difference between <paramref name="amount"/> and <c>remaining</c>.</returns>
    public (float remaining, float taken) TakeDamage(float amount, bool allowUnderflow = false)
    {
        if (amount < 0)
            Debug.LogWarning($"The amount was negative. {nameof(Current)} is increasing.");
        ResetRegenerationCooldown();
        float remaining = -ChangeValue(-amount, allowUnderflow: allowUnderflow);
        return (remaining, amount - remaining);
    }

    /// <summary>
    /// Reset regeneration cooldown. Useful if the creature is being hitted to halt regeneration.
    /// </summary>
    public void ResetRegenerationCooldown() => currentRegenerationDelay = 0;

    /// <summary>
    /// Increase <see cref="Current"/> by <paramref name="amount"/>.
    /// </summary>
    /// <param name="amount">Amount to increase <see cref="Current"/>.</param>
    /// <param name="allowUnderflow">Whenever <see cref="Current"/> could be higher than <see cref="Max"/> or not.</param>
    /// <returns><c>remaining</c>: Amount clamped above <see cref="Max"/>. <c>taken</c>: difference between <paramref name="amount"/> and <c>remaining</c>.</returns>
    public (float remaining, float taken) TakeHealing(float amount, bool allowOverflow = false)
    {
        if (amount < 0)
            Debug.LogWarning($"The amount was negative. {nameof(Current)} is decreasing.");
        float remaining = ChangeValue(amount, allowOverflow: allowOverflow);
        return (remaining, amount - remaining);
    }

    /// <summary>
    /// Regenerate behaviour.
    /// </summary>
    /// <param name="time">Time used to calculate the regeneration.</param>
    private void Regenerate(float time)
    {
        if (canRegenerate && currentRegenerationDelay >= regenerationDelay && Current < Max)
        {
            ChangeValue(regenerateRate * time);
            PlayRegenerationSound();
        }
        else
            currentRegenerationDelay += time;
    }

    /// <summary>
    /// Executes internal stuff of <see cref="HealthPoints"/>.
    /// </summary>
    /// <param name="time">Time used to calculate stuff.</param>
    public void Update(float time) => Regenerate(time);

    private void PlayRegenerationSound()
    {
        if (audioSource != null && playlist != null && !audioSource.isPlaying)
            playlist.Play(audioSource, Settings.IsSoundActive);
    }
}
