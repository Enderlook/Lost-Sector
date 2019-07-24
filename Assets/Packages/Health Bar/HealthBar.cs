using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [Header("Configuration")]
    [Tooltip("How numbers are shown, {0} is health, {1} is maximum health and {2} is percent of health. Eg: {0} / {1} ({2}%)")]
    public string textShowed = "{0} / {1} ({2}%)";
    [Tooltip("If damage or healing bars are active you can choose to add dynamic numbers.")]
    public bool dynamicNumbers;

    [Tooltip("Health bar color (usually at max health). Use black color to use the Health image UI color.")]
    public Color maxHealthColor = Color.green;
    [Tooltip("Health bar color at minimum health. If black, health won't change of color at low health.")]
    public Color minHealthColor = Color.red;

    [Header("Setup")]
    [Tooltip("Used to show numbers of health. Use null to deactivate it.")]
    public Text textNumber;

    [Tooltip("Represent object health.")]
    public GameObject healthBar;
    private Image healthImage;
    private RectTransform healthTransform;

    [Tooltip("Represent the amount of recent damage received. Use null to deactivate it.")]
    public Image damageBar = null;
    [Tooltip("Represent the amount of recent healing received. Use null to deactivate it.")]
    public GameObject healingBar = null;
    private Image healingImage;
    private RectTransform healingTransform;

    [Tooltip("Check to ceil health values (round up), useful if health is float, to avoid show 0 HP on bar while you still have 0.44 or below HP. On false, normal round will be performed.")]
    public bool ceilValues = true;

    [Tooltip("Used to show or hide the health bar. If null, it will show and hide each part by separate instead of just the canvas.")]
    public Canvas canvas;
    [Tooltip("Only used to hide or show in case Canvas is null.")]
    public Image frame;
    [Tooltip("Only used to hide or show in case Canvas is null.")]
    public Image background;

    private float maxHealth;
    private float health;

    /// <summary>
    /// Whenever the health bar is showed or hidden.<br/>
    /// Take into account that the script is still enabled and will update the health bar even if it's hidden.
    /// </summary>
    /// <seealso cref="IsEnabled"/>
    public bool IsVisible {
        get => canvas != null ? canvas.enabled : isVisible;
        set {
            isVisible = value;
            if (canvas != null)
                canvas.enabled = isVisible;
            else
            {
                healthImage.enabled = isVisible;
                if (textNumber != null)
                    textNumber.enabled = isVisible;
                if (damageBar != null)
                    damageBar.enabled = isVisible;
                if (healingImage != null)
                    healingImage.enabled = isVisible;
                if (frame != null)
                    frame.enabled = isVisible;
                if (background != null)
                    background.enabled = isVisible;
            }
        }
    }
    private bool isVisible = false;

    /// <summary>
    /// Whenever the health bar will be updated each frame or not.<br/>
    /// Take into account that the script is still enabled but it won't be updated on each frame. Also, it's still visible.
    /// </summary>
    /// <seealso cref="IsHidden"/>
    public bool IsEnabled { get => isEnabled; set => isEnabled = value; }
    private bool isEnabled = true;

    private void Awake() => Setup();

    /// <summary>
    /// Modify the health bar values without producing any animation effects (sliding the bar or changing the numbers).
    /// The health bar fill will be instantaneously set without producing animation. Health numbers will also change immediately.
    /// Both damage bar and healing bar fill will be set to 0, halting any current animation on them.
    /// Designed to initialize the health bar by first time.
    /// </summary>
    /// <param name="health"></param>
    /// <param name="maxHealth"></param>
    public void ManualUpdate(float health, float maxHealth)
    {
        this.health = health;
        this.maxHealth = maxHealth;

        // Fix bug, this shouldn't be happening
        if (healthImage == null)
            healthImage = healthBar.GetComponent<Image>();

        healthImage.fillAmount = this.health / this.maxHealth;
        if (damageBar != null)
            damageBar.fillAmount = 0;
        if (healingBar != null)
            healingImage.fillAmount = 0;
    }

    /// <summary>
    /// Modify the health bar values without producing any animation effects (sliding the bar or changing the numbers).
    /// The health bar fill will be instantaneously set without producing animation. Health numbers will also change immediately.
    /// Both damage bar and healing bar fill will be set to 0, halting any current animation on them.
    /// Both current health and maximum health will be assigned by maxHealth.
    /// Designed to initialize the health bar by first time.
    /// </summary>
    /// <param name="maxHealth"></param>
    public void ManualUpdate(float maxHealth)
    {
        ManualUpdate(maxHealth, maxHealth);
    }

    /// <summary>
    /// Get the <see cref="healthImage"/> color taking into account the percentage of remaining health.
    /// </summary>
    /// <returns>Color of the <see cref="healthImage"/></returns>
    private Color GetHealthColor()
    {
        return Color.Lerp(minHealthColor, maxHealthColor, healthImage.fillAmount + (damageBar != null ? damageBar.fillAmount : 0) - (healingBar != null ? healingImage.fillAmount : 0));
    }

    private void Update()
    {
        if (IsEnabled)
        {
            // Unfill the damage and healing bar per frame
            if (damageBar != null && damageBar.fillAmount > 0)
                damageBar.fillAmount -= Time.deltaTime;
            if (healingImage != null && healingImage.fillAmount > 0)
                healingImage.fillAmount -= Time.deltaTime;

            if (minHealthColor != Color.black)
            {
                healthImage.color = GetHealthColor();
            }
            else
            {
                healthImage.color = maxHealthColor;
            }

            if (textNumber != null)
            {
                if (dynamicNumbers)
                {
                    float dynamicPercent = healthImage.fillAmount + damageBar.fillAmount - healingImage.fillAmount,
                          dynamicHealth = maxHealth * dynamicPercent;
                    textNumber.text = string.Format(textShowed, Rounding(dynamicHealth), Rounding(maxHealth), Rounding(dynamicHealth / maxHealth * 100));
                }
                else
                {
                    textNumber.text = string.Format(textShowed, Rounding(health), Rounding(maxHealth), Rounding(health / maxHealth * 100));
                }
            }
        }
    }

    private float Rounding(float value)
    {
        if (ceilValues)
            return Mathf.Ceil(value);
        else
            return Mathf.Round(value);
    }

    /// <summary>
    /// Modify the current health and maximum health.
    /// This method will automatically calculate, show and animate the health bar, damage bar, healing bar and health number.
    /// </summary>
    /// <param name="health"></param>
    /// <param name="maxhealth"></param>
    public void UpdateValues(float health, float maxHealth)
    {
        this.maxHealth = maxHealth;
        Set(health);
    }

    /// <summary>
    /// Modify the current health.
    /// This method will automatically calculate, show and animate the health bar, damage bar, healing bar and health number.
    /// </summary>
    /// <param name="health"></param>
    public void UpdateValues(float health)
    {
        Set(health);
    }

    /*void Heal(float amount) { Change(amount); }
    void Damage(float amount) { Change(-amount); }*/
    /// <summary>
    /// Set the new health value and updates bars.
    /// </summary>
    /// <seealso cref="Change(float)"/>
    /// <param name="value">New <seealso cref="health"/> value.</param>
    private void Set(float value) { Change(value - health); }

    /// <summary>
    /// Updates bars and set the <see cref="health"/>.
    /// </summary>
    /// <param name="amount">Amount to add to <see cref="health"/>.</param>
    private void Change(float amount)
    {
        if (amount == 0)
            return;

        float old_health = health;
        health += amount;

        // Don't allow health be greater than maximum health nor lower than 0
        if (health > maxHealth)
        {
            health = maxHealth;
            amount = maxHealth - old_health;
        }
        else if (health < 0)
        {
            health = 0;
            amount = old_health;
        }

        // Fill the health bar
        healthImage.fillAmount = health / maxHealth;

        if (amount < 0)
        {
            amount = -amount;
            if (damageBar != null)
            {
                damageBar.fillAmount += amount / maxHealth;
                // Move the damage bar adjacent (next to the end) of the health bar 
                damageBar.transform.localPosition = new Vector3(healthTransform.rect.width * healthImage.fillAmount, 0, 0);
            }
            if (healingBar != null)
            {
                // On damage, the healing bar is reduced to avoid overlapping
                healingImage.fillAmount -= amount / maxHealth;
                healingBar.transform.localPosition = new Vector3(healthTransform.rect.width * healthImage.fillAmount - healingTransform.rect.width, 0, 0);
            }
        }
        else if (amount > 0)
        {
            if (healingBar != null)
            {
                healingImage.fillAmount += amount / maxHealth;
                // Move the healing bar adjacent (next to the end) of the health bar but overlap part of it by its filled part
                healingBar.transform.localPosition = new Vector3(healthTransform.rect.width * healthImage.fillAmount - healingTransform.rect.width, 0, 0);
            }
            if (damageBar != null)
            {
                // On healing, the damage bar is reduced to avoid overlapping
                damageBar.fillAmount -= amount / maxHealth;
                damageBar.transform.localPosition = new Vector3(healthTransform.rect.width * healthImage.fillAmount, 0, 0);
            }
        }
        // Fix bug, preventing the healing bar overflow from the left side
        if (healingBar != null && healingTransform.rect.width < (healingTransform.rect.width * healingImage.fillAmount - healingBar.transform.localPosition.x))
        {
            healingImage.fillAmount -= ((healingTransform.rect.width * healingImage.fillAmount - healingBar.transform.localPosition.x) - healingTransform.rect.width) / healingTransform.rect.width;
        }
    }

#if UNITY_EDITOR
    /// <summary>
    /// Update color of health bar.
    /// </summary>
    private void OnValidate()
    {
        Setup();
        if ((healthImage = healthBar.GetComponent<Image>()) == null)
            Debug.LogWarning($"Gameobject {gameObject.name} has a {nameof(healthBar)} which lacks of {nameof(Image)} component.");
        else
            healthImage.color = GetHealthColor();
    }
#endif

    /// <summary>
    /// Get the require components <seealso cref="healthImage"/>, <seealso cref="healthTransform"/>, <seealso cref="healingImage"/>, <seealso cref="healingTransform"/> and set <seealso cref="maxHealth"/>.
    /// </summary>
    private void Setup()
    {
        healthImage = healthBar.GetComponent<Image>();
        healthTransform = healthBar.GetComponent<RectTransform>();

        if (healingBar != null)
        {
            healingImage = healingBar.GetComponent<Image>();
            healingTransform = healingBar.GetComponent<RectTransform>();
        }

        if (maxHealthColor == Color.black)
            maxHealthColor = healthImage.color;
    }

    /// <summary>
    /// Returns <seealso cref="Image.fillAmount"/> of the <see cref="healthBar"/>.<br/>
    /// </summary>
    public float HealthBarPercentFill => healthImage.fillAmount;
    /// <summary>
    /// Returns <seealso cref="Image.fillAmount"/> of the <see cref="healingBar"/>.<br/>
    /// Warning! If <see cref="healingBar"></see> is <see langword="null"/> it will return <see langword="null"/>.
    /// </summary>
    public float? HealingBarPercentFill => healingImage?.fillAmount;
    /// <summary>
    /// Returns <seealso cref="Image.fillAmount"/> of the <see cref="damageBar"/>.<br/>
    /// Warning! If <see cref="damageBar"/> is <see langword="null"/> it will return <see langword="null"/>.
    /// </summary>
    public float? DamageBarPercentFill => damageBar?.fillAmount;

    /// <summary>
    /// Returns <see langword="true"/> if <seealso cref="Image.fillAmount"/> of the <see cref="healingBar"/> if 0 or <see cref="healingBar"/> is <see langword="null"/>.<br/>
    /// </summary>
    public bool IsHealingBarPercentHide => healingImage == null ? true : healingImage.fillAmount == 0;
    /// <summary>
    /// Returns <see langword="true"/> if <seealso cref="Image.fillAmount"/> of the <see cref="damageBar"/> if 0 or <see cref="damageBar"/> is <see langword="null"/>.<br/>
    /// </summary>
    public bool IsDamageBarPercentHide => damageBar == null ? true : damageBar.fillAmount == 0;
}
