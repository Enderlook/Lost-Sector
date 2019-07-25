using UnityEngine;

public class RapidFireRate : Pickupable
{
    [Header("Configuration")]
    [Tooltip("Fire rate multiplier.")]
    public float fireRateMultiplier;
    [Tooltip("Duration of effect in seconds.")]
    public float durationOfEffect;

    public override void Pickup(Player player)
    {
        player.AddEffect(new RapidFireRateEffect(fireRateMultiplier, durationOfEffect));
    }
}

public class RapidFireRateEffect : Effect
{
    public RapidFireRateEffect(float strength, float duration) : base(strength, duration) { }

    public override string Name => "Fire Rate";
    public override bool IsBuff => strength > 0;

    public override bool ReplaceCurrentInstance => true;

    private float initialValue;

    protected override void OnStart()
    {
        initialValue = livingObject.fireRateMultiplier;
        base.OnStart();
    }

    protected override void OnUpdate(float time)
    {
        livingObject.fireRateMultiplier = initialValue + strength * Mathf.Pow(duration / maxDuration, .5f);
        base.OnUpdate(time);
    }

    protected override void OnEnd(bool wasAborted)
    {
        livingObject.fireRateMultiplier = initialValue;
        base.OnEnd(wasAborted);
    }
}