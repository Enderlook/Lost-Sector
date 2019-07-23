using UnityEngine;
using UnityEngine.UI;

public class WeaponCooldownDisplay : MonoBehaviour
{
    [Header("Configuration")]
    [Tooltip("Background sprite of weapon.")]
    public Image image;

    [Header("Setup")]
    [Tooltip("Weapon to track cooldown")]
    public LivingObjectAddons.Weapon weapon;

    private void Update()
    {
        image.fillAmount = weapon.CooldownPercent;
    }
}
