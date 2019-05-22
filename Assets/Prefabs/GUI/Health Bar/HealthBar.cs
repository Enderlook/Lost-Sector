using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour {

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
    Image healthImage;
    RectTransform healthTransform;

    [Tooltip("Represent the amount of recent damage received. Use null to deactivate it.")]
    public Image damageBar = null;
    [Tooltip("Represent the amount of recent healing received. Use null to deactivate it.")]
    public GameObject healingBar = null;
    private Image healingImage;
    private RectTransform healingTransform;

    [Tooltip("Check to ceil health values (round up), useful if health is float, to avoid show 0 HP on bar while you still have 0.44 or below HP. On false, normal round will be performed.")]
    public bool ceilValues = true;

    private float maxHealth;
    private float health;

    private void Awake()
    {
        healthImage = healthBar.GetComponent<Image>();
        healthTransform = healthBar.GetComponent<RectTransform>();

        healingImage = healingBar.GetComponent<Image>();
        healingTransform = healingBar.GetComponent<RectTransform>();

        if (maxHealthColor == Color.black)
            maxHealthColor = healthImage.color;
    }

    /// <summary>
    /// Modify the health bar values without producing any animation effects (sliding the bar or changing the numbers).
    /// The health bar fill will be instataneously set without producing animation. Health numbers will also change inmediately.
    /// Both damage bar and healing bar fill will be set to 0, halting any current animation on them.
    /// Designed to intialize the health bar by first time.
    /// </summary>
    /// <param name="health"></param>
    /// <param name="maxHealth"></param>
    public void ManualUpdate(float health, float maxHealth)
    {
        this.health = health;
        this.maxHealth = maxHealth;

        healthImage.fillAmount = this.health / this.maxHealth;
        damageBar.fillAmount = 0;
        healingImage.fillAmount = 0;
    }

    /// <summary>
    /// Modify the health bar values without producing any animation effects (sliding the bar or changing the numbers).
    /// The health bar fill will be instataneously set without producing animation. Health numbers will also change inmediately.
    /// Both damage bar and healing bar fill will be set to 0, halting any current animation on them.
    /// Both current health and maximum health will be asigned by maxHealth.
    /// Designed to intialize the health bar by first time.
    /// </summary>
    /// <param name="maxHealth"></param>
    public void ManualUpdate(float maxHealth)
    {
        ManualUpdate(maxHealth, maxHealth);
    }

    private void Update()
    {
        // Unfill the damage and healing bar per frame
        if (damageBar != null && damageBar.fillAmount > 0)
            damageBar.fillAmount -= Time.deltaTime;
        if (healingImage != null && healingImage.fillAmount > 0)
            healingImage.fillAmount -= Time.deltaTime;

        if (minHealthColor != Color.black)
        {
            healthImage.color = Color.Lerp(minHealthColor, maxHealthColor, healthImage.fillAmount + damageBar.fillAmount - healingImage.fillAmount);
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
                      dynamicHealth = health * dynamicPercent;
                textNumber.text = string.Format(textShowed, Rounding(dynamicHealth), Rounding(maxHealth), Rounding(dynamicHealth / maxHealth * 100));
            }
            else
            {
                textNumber.text = string.Format(textShowed, Rounding(health), Rounding(maxHealth), Rounding(health / maxHealth * 100));
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
    public void UpdateValues(float health, float maxhealth)
    {
        this.maxHealth = maxhealth;
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
    private void Set(float value) { Change(value - health); }

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
}
