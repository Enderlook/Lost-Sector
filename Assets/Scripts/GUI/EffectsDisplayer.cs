using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Effects
{
    public class EffectsDisplayer : MonoBehaviour
    {
        [Header("Configuration")]
        [Tooltip("Panel to add effects.")]
        public RectTransform panel;
        [Tooltip("Background image used in effects.")]
        public Sprite backgroundSprite;

        [Header("Setup")]
        [Tooltip("Effect catalog.")]
        public EffectCatalog effectCatalog;

        private EffectSlots effectSlots;
        private List<Effect> toUpdate;

        private Color BUFF_COLOR = Color.green;
        private Color DEBUFF_COLOR = Color.red;

        private EffectManager effectManager;

        private void Start() => effectSlots = new EffectSlots(panel, backgroundSprite);

        public void CheckEffects(IEnumerable<Effect> effects)
        {
            toUpdate = new List<Effect>();

            foreach (Effect effect in effects)
            {
                EffectUI effectUI;
                if (effectSlots.TryGetValue(effect, out effectUI))
                    UpdateEffect(effectUI, effect);
                else
                    toUpdate.Add(effect);
            }

            IEnumerator<Effect> toUpdateEnumerable = toUpdate.GetEnumerator();
            
            foreach (EffectSlot effectSlot in effectSlots.GetNonUpdatedEffects(Time.frameCount))
            {
                if (toUpdateEnumerable.MoveNext())
                {
                    effectSlot.effect = toUpdateEnumerable.Current;
                    effectSlot.effectUI.SetActive(true);
                    UpdateEffect(effectSlot.effectUI, toUpdateEnumerable.Current);
                }
                else
                    effectSlot.effectUI.SetActive(false);
            }

            while (toUpdateEnumerable.MoveNext())
            {
                effectSlots.Add(toUpdateEnumerable.Current, Time.frameCount);
                UpdateEffect(effectSlots[toUpdateEnumerable.Current], toUpdateEnumerable.Current);
            }
        }

        private void UpdateEffect(EffectUI effectUI, Effect effect)
        {
            EffectConfiguration effectConfiguration = effectCatalog.Effects[effect.Name];
            if (effect.IsBuff)
            {
                effectUI.background.color = BUFF_COLOR;
                effectUI.image.sprite = effectConfiguration.spriteBuff;
            }
            else
            {
                effectUI.background.color = DEBUFF_COLOR;
                effectUI.image.sprite = effectConfiguration.spriteDebuff;
            }
            effectUI.background.fillAmount = effect.DurationPercent;
            effectUI.lastUpdatedFrame = Time.frameCount;
        }
    }


    public class EffectSlots
    {
        private List<EffectSlot> effectSlots;
        private RectTransform panel;
        private Sprite backgroundSprite;

        private bool Contains(Effect effect) => effectSlots.ContainsBy(e => e.effect == effect);
        
        public EffectSlots(RectTransform panel, Sprite backgroundSprite)
        {
            this.panel = panel;
            this.backgroundSprite = backgroundSprite;
            effectSlots = new List<EffectSlot>();
        }

        public EffectUI this[Effect effect] {
            get => effectSlots.First(e => e.effect == effect).effectUI;
            set {
                if (Contains(effect))
                    effectSlots.First(e => e.effect == effect).effectUI = value;
                else
                    effectSlots.Add(new EffectSlot(effect, value));
            }
        }

        public void Add(Effect key, int currentFrame) => effectSlots.Add(new EffectSlot(key, new EffectUI(key, backgroundSprite, currentFrame, panel)));

        public bool TryGetValue(Effect key, out EffectUI item)
        {
            foreach (EffectSlot effectSlot in effectSlots)
            {
                if (effectSlot.effect == key)
                {
                    item = effectSlot.effectUI;
                    return true;
                }
            }
            item = null;
            return false;
        }

        public IEnumerable<EffectSlot> GetNonUpdatedEffects(int currentFrame) => effectSlots.Where(e => e.effectUI.lastUpdatedFrame < currentFrame);
    }

    public class EffectSlot
    {
        public Effect effect;
        public EffectUI effectUI;

        public EffectSlot(Effect effect, EffectUI effectUI)
        {
            this.effect = effect;
            this.effectUI = effectUI;
        }
    }

    public class EffectUI
    {
        private GameObject gameObject;
        public Image image;
        public Image background;

        public int lastUpdatedFrame;

        public void SetActive(bool isActive) => gameObject.SetActive(isActive);
        public bool IsActive => gameObject.activeSelf;

        public EffectUI(Effect effect, Sprite backgroundSprite, int currentFrame, Transform parentTransform)
        {
            // https://answers.unity.com/questions/948887/what-is-the-correct-syntax-to-create-a-new-object.html
            gameObject = new GameObject("Effect", typeof(RectTransform));
            RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
            gameObject.transform.SetParent(parentTransform, false);
            background = gameObject.AddComponent<Image>();
            background.sprite = backgroundSprite;
            background.preserveAspect = true;
            background.type = Image.Type.Filled;
            background.fillMethod = Image.FillMethod.Radial360;

            GameObject imageGameObject = new GameObject("Image", typeof(RectTransform));
            imageGameObject.transform.SetParent(gameObject.transform, false);
            RectTransform imageRectTransform = imageGameObject.GetComponent<RectTransform>();
            imageRectTransform.anchorMin = Vector2.zero;
            imageRectTransform.anchorMax = Vector2.one;
            imageRectTransform.sizeDelta = Vector2.zero;
            image = imageGameObject.AddComponent<Image>();
            image.preserveAspect = true;

            lastUpdatedFrame = currentFrame;
        }
    }
}