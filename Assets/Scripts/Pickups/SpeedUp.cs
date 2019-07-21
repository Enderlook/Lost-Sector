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
        player.AddEffect(new SpeedUpEffect(speedMultiplier, durationOfEffect));
    }
}

public class SpeedUpEffect : Effect
{
    public SpeedUpEffect(float strength, float duration) : base(strength, duration) { }

    public override bool ReplaceCurrentInstance => true;

    private float initialValue;

    protected override void OnStart()
    {
        initialValue = livingObject.speedMultiplier;
        base.OnStart();
    }

    protected override void OnUpdate(float time)
    {
        livingObject.speedMultiplier = initialValue + strength * Mathf.Pow(duration / maxDuration, .5f);
        base.OnUpdate(time);
    }

    protected override void OnEnd(bool wasAborted)
    {
        livingObject.speedMultiplier = initialValue;
        base.OnEnd(wasAborted);
    }
}