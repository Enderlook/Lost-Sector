using System.Collections.Generic;
using UnityEngine;

namespace Effects
{
    public class EffectManager
    {
        public List<Effect> effects = new List<Effect>();
        private LivingObject livingObject;

        /// <summary>
        /// Construct <see cref="EffectManager"/>.
        /// </summary>
        /// <param name="livingObject">Instance of the <seealso cref="LivingObject"/> where it's being constructed.</param>
        public EffectManager(LivingObject livingObject) => this.livingObject = livingObject;

        /// <summary>
        /// Add an effect to the creature and track it.
        /// </summary>
        /// <param name="effect">Effect to add.</param>
        public void AddEffect(Effect effect)
        {
            if (effect.ReplaceCurrentInstance && effect.DurationPercent > 0) // Single use effects don't replace current instance because they don't have
            {
                System.Type target = effect.GetType();
                for (int i = 0; i < effects.Count; i++)
                {
                    if (effects[i].GetType() == target)
                    {
                        effects[i].Kill();
                        effects.RemoveAt(i);
                        break;
                    }
                }
            }
            effect.Setup(livingObject);
            if (!effect.shouldBeDisposed) // Some effect don't have duration and so they are disposed after Setup
                effects.Add(effect);
        }

        /// <summary>
        /// Update effects.
        /// </summary>
        /// <param name="time">Time in seconds since last frame (<seealso cref="Time.deltaTime"/>).</param>
        public void Update(float time)
        {
            for (int i = 0; i < effects.Count; i++)
            {
                Effect effect = effects[i];
                if (effect.shouldBeDisposed)
                    effects.RemoveAt(i);
                else
                {
                    effect.Update(time);
                }
            }
        }
    }

    public abstract class Effect
    {
        protected readonly float strength;
        protected readonly float maxDuration;
        protected float duration;
        public bool shouldBeDisposed = false;
        protected LivingObject livingObject;
        private IUpdate update;

        /// <summary>
        /// Name of the effect.
        /// </summary>
        public abstract string Name { get; }
        /// <summary>
        /// Whenever it's a buff or a debuff.
        /// </summary>
        public abstract bool IsBuff { get; }
        /// <summary>
        /// Get duration percent from <c><see cref="duration"/> / <see cref="maxDuration"/></c>
        /// </summary>
        public float DurationPercent {
            get {
                try
                {
                    return duration / maxDuration;
                }
                catch (System.DivideByZeroException)
                {
                    return 0;
                }
            }
        }

        /// <summary>
        /// Whenever the old instance of this effect should be removed before add this one.
        /// </summary>
        public abstract bool ReplaceCurrentInstance { get; }

        /// <summary>
        /// Construct the <see cref="Effect"/>.
        /// </summary>
        /// <param name="strength">Strength of the effect.</param>
        /// <param name="duration">Duration in seconds of the effect.</param>
        public Effect(float strength, float duration)
        {
            this.strength = strength;
            this.duration = duration;
            maxDuration = duration;
        }

        /// <summary>
        /// Initialize the effect.
        /// </summary>
        /// <param name="livingObject">Instance of the <seealso cref="LivingObject"/> where it's being constructed.</param>
        public void Setup(LivingObject livingObject)
        {
            this.livingObject = livingObject;
            this.CastOrNull<IStart>()?.OnStart();
            this.TryCast(out update);
            if (duration <= 0)
                End();
        }

        private void End()
        {
            this.CastOrNull<IEnd>()?.OnEnd(false);
            shouldBeDisposed = true;
        }

        /// <summary>
        /// Update the effect and reduce duration. Also determines if it <seealso cref="shouldBeDisposed"/>.
        /// </summary>
        /// <param name="time">Time in seconds since last frame (<seealso cref="Time.deltaTime"/>).</param>
        public void Update(float time)
        {
            if (duration <= 0)
                End();
            else
            {
                update?.OnUpdate(time);
                duration -= time;
            }
        }

        /// <summary>
        /// Force the interruption of the effect.
        /// </summary>
        public void Kill() => this.CastOrNull<IEnd>()?.OnEnd(true);
    }

    public interface IStart
    {
        /// <summary>
        /// Initial effect.
        /// </summary>
        void OnStart();
    }
    public interface IUpdate
    {
        /// <summary>
        /// Update the effect.
        /// </summary>
        /// <param name="time">Time in seconds since last frame (<seealso cref="Time.deltaTime"/>).</param>
        void OnUpdate(float time);
    }
    public interface IEnd
    {
        /// <summary>
        /// Finalize the effect.
        /// </summary>
        /// <param name="wasAborted">Whenever the effect end due duration expiration or if it was aborted by an external force.</param>
        void OnEnd(bool wasAborted);
    }

}