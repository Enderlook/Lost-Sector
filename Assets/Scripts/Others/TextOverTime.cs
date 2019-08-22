public class TextOverTime
{
    private readonly float targetValue;
    private float showed;
    private const float PERCENT_RATE = .35f;

    /// <summary>
    /// Generate a counter that increases until show a <paramref name="targetValue"/>.
    /// </summary>
    /// <param name="targetValue">Value target to show.</param>
    public TextOverTime(float targetValue)
    {
        this.targetValue = targetValue;
    }

    /// <summary>
    /// Update counter.
    /// </summary>
    /// <param name="time">Time passed since las update.</param>
    /// <returns>New value to show.</returns>
    public float Update(float time)
    {
        if (showed < targetValue)
        {
            showed += targetValue * PERCENT_RATE * time;
            if (showed > targetValue)
                showed = targetValue;
        }
        return showed;
    }
}