using UnityEngine;
using UnityEngine.UI;

public class WeaponCooldownDisplay : MonoBehaviour
{
    [Header("Configuration")]
    [Tooltip("Background sprite of weapon.")]
    public Image image;
    [Tooltip("Reverse fill mode.")]
    public bool reverseFill;
    [Tooltip("Colors to use.")]
    public ColorPercent[] colors;

    [Header("Setup")]
    [Tooltip("Weapon to track cooldown")]
    public LivingObjectAddons.Weapon weapon;

    private void Update()
    {
        image.fillAmount = reverseFill ? 1 - weapon.CooldownPercent : weapon.CooldownPercent;
        if (colors.Length > 0)
            image.color = GetColor(1 - weapon.CooldownPercent);
    }

    private Color GetColor(float percent)
    {
        for (int i = colors.Length - 1; i >= 0; i--)
        {
            if (colors[i].percent <= percent)
                return colors[i].color;
        }
        throw new System.Exception("No matching color found. This shouldn't be happening.");
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (colors.Length > 0)
        {
            if (colors[0].percent != 0)
            {
                Debug.LogWarning($"Gameobject {gameObject.name} has a the element on index 1 from {nameof(colors)} with a {nameof(ColorPercent.percent)} non equal to 0 ({colors[0].percent}). It was fixed.");
                colors[0].percent = 0;
            }
            for (int i = 0; i < colors.Length - 1; i++)
            {
                int next = i + 1;
                if (colors[i].percent >= colors[next].percent)
                {
                    Debug.LogWarning($"Gameobject {gameObject.name} has a the element on index {next} from {nameof(colors)} array with a {nameof(ColorPercent.percent)} ({colors[next].percent}) lower or equal than its precessor ({colors[i].percent}).\nIt should be greater.");
                    colors[next].percent = colors[i].percent;
                }
            }
        }
    }
#endif

    [System.Serializable]
    public class ColorPercent
    {
        [Tooltip("Color to show.")]
        public Color color;
        [Range(0, 1)]
        [Tooltip("Minimal fill amount to show it")]
        public float percent;
    }
}