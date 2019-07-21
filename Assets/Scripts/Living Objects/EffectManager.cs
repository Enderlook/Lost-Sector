﻿using System.Collections.Generic;
using UnityEngine;

public class EffectManager
{
    private List<Effect> effects = new List<Effect>();
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
        if (effect.ReplaceCurrentInstance)
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

    /// <summary>
    /// Whenever the old instance of this effect should be removed before add this one.
    /// </summary>
    public abstract bool ReplaceCurrentInstance { get; }
    /// <summary>
    /// Initial effect.
    /// </summary>
    protected virtual void OnStart() { }
    /// <summary>
    /// Update the effect.
    /// </summary>
    /// <param name="time">Time in seconds since last frame (<seealso cref="Time.deltaTime"/>).</param>
    protected virtual void OnUpdate(float time) { }
    /// <summary>
    /// Finalize the effect.
    /// </summary>
    /// <param name="wasAborted">Whenever the effect end due duration expiration or if it was aborted by an external force.</param>
    protected virtual void OnEnd(bool wasAborted) { }

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
        OnStart();
    }

    /// <summary>
    /// Update the effect and reduce duration. Also determines if it <seealso cref="shouldBeDisposed"/>.
    /// </summary>
    /// <param name="time">Time in seconds since last frame (<seealso cref="Time.deltaTime"/>).</param>
    public void Update(float time)
    {
        if (duration <= 0)
        {
            OnEnd(false);
            shouldBeDisposed = true;
        }
        else
        {
            OnUpdate(time);
            duration -= time;
        }
    }

    /// <summary>
    /// Force the interruption of the effect.
    /// </summary>
    public void Kill() => OnEnd(true);
}