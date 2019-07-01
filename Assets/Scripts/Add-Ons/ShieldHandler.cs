using UnityEngine;

[System.Serializable]
public class ShieldHandler
{
    [Header("Configuration")]
    [Tooltip("Shield color (usually at max shield). Use black color to use the Shield image color. Alpha channel will be ignored.")]
    public Color maxShieldColor = Color.blue;
    [Tooltip("Shield color at minimum shield. If black, shield won't change of color at low shield. Alpha channel will be ignored.")]
    public Color minShieldColor = Color.magenta;

    [Header("Setup")]
    [Tooltip("Shield sprite.")]
    public SpriteRenderer shieldImage;

    /// <summary>
    /// Initialize the shield. Mandatory.<br/>
    /// Get and store the <see cref="SpriteRenderer"/> from the <see cref="gameObject"/>.<br/>
    /// <see cref="maxShieldColor"/> and <see cref="minShieldColor"/> alphas will be set to 1 and 0, respectively.<br/>
    /// Executes <see cref="UpdateColor(float shieldRatio)"/> using <paramref name="shieldRatio"/>.
    /// <param name="shieldRatio">Percent shield fill.</param>
    /// </summary>
    public void Initialize(float shieldRatio)
    {
        maxShieldColor.a = 1;
        minShieldColor.a = 0;
        UpdateColor(shieldRatio);
    }

    /// <summary>
    /// Update the opacity and color of the shield bubble.
    /// </summary>
    /// <param name="shieldRatio">Percent shield fill.</param>
    public void UpdateColor(float shieldRatio) => shieldImage.color = Color.Lerp(minShieldColor, maxShieldColor, shieldRatio);
}
