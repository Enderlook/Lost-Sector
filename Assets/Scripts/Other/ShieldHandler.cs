using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class ShieldHandler
{
    [Header("Configuration")]
    [Tooltip("Shield color (usually at max shield). Use black color to use the Shield image color. Alpha channel will be ignored.")]
    public Color maxShieldColor = Color.blue;
    [Tooltip("Shield color at minimum shield. If black, shield won't change of color at low shield. Alpha channel will be ignored.")]
    public Color minShieldColor = Color.magenta;

    [Header("Setup")]
    [Tooltip("Shield sprite game object.")]
    public GameObject shieldGameObject;
    private SpriteRenderer shieldImage;

    private float maxShield;

    /// <summary>
    /// Initialize the shield. Mandatory.
    /// Retrieves the SpriteRenderer from the gameObject.
    /// maxShieldColor and minShieldColor alphas will be set to 1 and 0, respectively.
    /// Executes UpdateColor using maxShield and current shield.
    /// </summary>
    public void Initialize(float shield, float maxShield)
    {
        shieldImage = shieldGameObject.GetComponent<SpriteRenderer>();
        maxShieldColor.a = 1;
        minShieldColor.a = 0;

        UpdateColor(shield, maxShield);
    }

    /// <summary>
    /// Initialize the shield. Mandatory.
    /// Retrieves the SpriteRenderer from the gameObject.
    /// maxShieldColor and minShieldColor alphas will be set to 1 and 0, respectively.
    /// Executes UpdateColor using maxShield for both maximum shield and current shield.
    /// </summary>
    public void Initialize(float maxShield)
    {
        shieldImage = shieldGameObject.GetComponent<SpriteRenderer>();
        maxShieldColor.a = 1;
        minShieldColor.a = 0;

        UpdateColor(maxShield, maxShield);
    }

    /// <summary>
    /// Update the opacity and color of the shield bubble.
    /// </summary>
    /// <param name="shield"></param>
    /// <param name="maxShield"></param>
    public void UpdateColor(float shield, float maxShield)
    {
        this.maxShield = maxShield;
        UpdateColor(shield);
    }

    /// <summary>
    /// Update the opacity and color of the shield bubble.
    /// Since no maxShield is provided, it will use the last provided maxShield;
    /// </summary>
    /// <param name="shield"></param>
    public void UpdateColor(float shield)
    {
        shieldImage.color = Color.Lerp(minShieldColor, maxShieldColor, shield / maxShield);
    }
}
