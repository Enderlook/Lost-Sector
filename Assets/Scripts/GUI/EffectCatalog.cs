using System.Collections.Generic;
using UnityEngine;

namespace Effects
{
    [CreateAssetMenu(fileName = "Effect Catalog", menuName = "Effect Catalog")]
    public class EffectCatalog : ScriptableObject
    {
        [Tooltip("Configuration of each effect.")]
        public EffectConfiguration[] effects;

        [HideInInspector]
        public Dictionary<string, EffectConfiguration> effectsDictionary;
        public Dictionary<string, EffectConfiguration> Effects {
            get {
                if (effectsDictionary == null)
                {
                    effectsDictionary = new Dictionary<string, EffectConfiguration>();
                    foreach (EffectConfiguration effectConfiguration in effects)
                    {
                        effectsDictionary.Add(effectConfiguration.name, effectConfiguration);
                    }
                }
                return effectsDictionary;
            }
        }
    }

    [System.Serializable]
    public class EffectConfiguration
    {
        [Tooltip("Name of the effect.")]
        public string name;
        [Tooltip("Sprite used if effect is a buff.")]
        public Sprite spriteBuff;
        [Tooltip("Sprite used if effect is a debuff.")]
        public Sprite spriteDebuff;
    }
}