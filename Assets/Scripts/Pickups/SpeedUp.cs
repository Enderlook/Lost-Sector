using UnityEngine;

public class SpeedUp : Pickupable
{
    [Header("Configuration")]
    [Tooltip("Speed multiplier.")]
    public float speedMultiplier;
    [Tooltip("Duration of effect in seconds.")]
    public float durationOfEffect;

    public override void Pickup(Player player)
    {
        player.AddEffect(new SpeedEffect(speedMultiplier, durationOfEffect,
            (strength, maxDuration, duration, initialValue) => initialValue + strength * Mathf.Pow(duration / maxDuration, .5f)
        ));
    }
}

public class SpeedEffect : Effect
{
    public SpeedEffect(float strength, float duration, StrengthCalculate strengthCalculate) : base(strength, duration)
    {
        this.strengthCalculate = strengthCalculate;
    }

    public override bool ReplaceCurrentInstance => true;

    private float initialValue;

    private StrengthCalculate strengthCalculate;

    protected override void OnStart()
    {
        initialValue = livingObject.SpeedMultiplier;
        base.OnStart();
    }

    protected override void OnUpdate(float time)
    {
        livingObject.SpeedMultiplier = strengthCalculate(strength, maxDuration, duration, initialValue);
        base.OnUpdate(time);
    }

    protected override void OnEnd(bool wasAborted)
    {
        livingObject.SpeedMultiplier = initialValue;
        base.OnEnd(wasAborted);
    }
}

public delegate float StrengthCalculate(float strength, float maxDuration, float duration, float initialValue);