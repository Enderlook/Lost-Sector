using UnityEngine;
using UnityEngine.UI;

public class WeaponCooldownDisplay : MonoBehaviour
{
    [Header("Configuration")]
    [Tooltip("Background sprite of weapon.")]
    public Image image;
    [Tooltip("Reverse fill mode.")]
    public bool reverseFill;
    [Tooltip("Configuration by percent.")]
    public ModePercent[] modes;

    [Header("Setup")]
    [Tooltip("Weapon to track cooldown")]
    public LivingObjectAddons.Weapon weapon;
    [Tooltip("Audio Source to play sounds.")]
    public AudioSource audioSource;

    private Sound lastSound;

    private void Update()
    {
        float charge = reverseFill ? 1 - weapon.CooldownPercent : weapon.CooldownPercent;
        image.fillAmount = charge;
        if (modes.Length > 0)
        {
            ModePercent mode = GetMode(1 - weapon.CooldownPercent);
            image.color = mode.color;
            if (audioSource != null && mode.sound != lastSound && lastSound != null && mode.sound != null)
                mode.sound.Play(audioSource, Settings.IsSoundActive);
            lastSound = mode.sound;
        }
    }
    private float GetCharge() => reverseFill ? 1 - weapon.CooldownPercent : weapon.CooldownPercent;

    private ModePercent GetMode(float percent)
    {
        for (int i = modes.Length - 1; i >= 0; i--)
        {
            if (modes[i].percent <= percent)
                return modes[i];
        }
        throw new System.Exception("No matching mode found. This shouldn't be happening.");
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (modes.Length > 0)
        {
            if (modes[0].percent != 0)
            {
                Debug.LogWarning($"Gameobject {gameObject.name} has a the element on index 1 from {nameof(modes)} with a {nameof(ModePercent.percent)} non equal to 0 ({modes[0].percent}). It was fixed.");
                modes[0].percent = 0;
            }
            for (int i = 0; i < modes.Length - 1; i++)
            {
                int next = i + 1;
                if (modes[i].percent >= modes[next].percent)
                {
                    Debug.LogWarning($"Gameobject {gameObject.name} has a the element on index {next} from {nameof(modes)} array with a {nameof(ModePercent.percent)} ({modes[next].percent}) lower or equal than its predecessor ({modes[i].percent}).\nIt should be greater.");
                    modes[next].percent = modes[i].percent;
                }
            }
        }
    }
#endif

    [System.Serializable]
    public class ModePercent
    {
        [Tooltip("Color to show.")]
        public Color color;
        [Tooltip("Sound to play")]
        public Sound sound;
        [Range(0, 1)]
        [Tooltip("Minimal fill amount to show it")]
        public float percent;
    }
}