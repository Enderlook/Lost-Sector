using UnityEngine;
using UnityEngine.UI;

public class FloatingText : MonoBehaviour
{
    public enum TYPE_OF_ROUNDING { ROUND, CEIL, FLOOR, TRUNC }

    [Header("Configuration")]
    [Header("Overridable by Floating Text Controller", order = 2)]
    [Tooltip("Time before self destroy in seconds. If 0, duration of the animation will be used.")]
    public float timeBeforeDestroy;
    [Tooltip("Text color.")]
    public Color textColor = Color.red;
    [Tooltip("Random spawn offset.")]
    public Vector2 randomOffset = Vector2.one;
    [Tooltip("Multiply the scale of the canvas by this value.")]
    public float scaleMultiplier = 1;

    [Tooltip("Digit precision (decimals) for numbers. Whenever a float is given to show, the number is rounded by a certain amount of digits.")]
    public int digitPrecision = 0;
    [Tooltip("Determines how decimal digits are rounded.")]
    public TYPE_OF_ROUNDING typeOfRounding = TYPE_OF_ROUNDING.ROUND;


    [Header("Setup")]
    [Tooltip("Text component to write.")]
    public Text text;
    [Tooltip("Animator component of text.")]
    public Animator animator;

    private void Start()
    {
        text.color = textColor;
        transform.localScale = Vector3.one * scaleMultiplier;
        transform.Translate(RandomOffset());
        // https://forum.unity.com/threads/how-to-find-animation-clip-length.465751/
        float durationOfClip = animator.runtimeAnimatorController.animationClips[0].length;
        Destroy(gameObject, timeBeforeDestroy == 0 ? durationOfClip : timeBeforeDestroy);
    }

    /// <summary>
    /// Configure the <see cref="FloatingText"/>.<br/>
    /// Used to override the configuration of the <see cref="FloatingText"/> Prefab.
    /// </summary>
    /// <param name="text">Text to display.</param>
    /// <param name="textColor">Color of the text.</param>
    /// <param name="scaleMultiplier">Scale multiplier to current scale.</param>
    /// <param name="timeBeforeDestroy">Time in seconds before destroy itself.</param>
    /// <param name="randomOffset">Random offset applied on spawn of the floating text.</param>
    /// <seealso cref="SetText(string)"/>
    /// <seealso cref="SetTextColor(Color)"/>
    /// <seealso cref="SetScaleMultiplier(float)"/>
    /// <seealso cref="SetTimeBeforeDestroy(float)"/>
    /// <seealso cref="SetRandomOffset(Vector2)"/>
    /// <seealso cref="SetConfiguration(float, Color?, float?, float?, Vector2?, int?, TYPE_OF_ROUNDING?)"/>
    public void SetConfiguration(string text, Color? textColor = null, float? scaleMultiplier = null, float? timeBeforeDestroy = null, Vector2? randomOffset = null)
    {
        SetText(text);
        if (textColor != null)
            SetTextColor((Color)textColor);
        if (scaleMultiplier != null)
            SetScaleMultiplier((float)scaleMultiplier);
        if (timeBeforeDestroy != null)
            SetTimeBeforeDestroy((float)timeBeforeDestroy);
        if (randomOffset != null)
            SetRandomOffset((Vector2)randomOffset);
    }

    /// <summary>
    /// Configure the <see cref="FloatingText"/>.<br/>
    /// Used to override the configuration of the <see cref="FloatingText"/> Prefab.
    /// </summary>
    /// <param name="number">Number to display.</param>
    /// <param name="numberColor">Color of the number.</param>
    /// <param name="scaleMultiplier">Scale multiplier to current scale.</param>
    /// <param name="timeBeforeDestroy">Time in seconds before destroy itself.</param>
    /// <param name="randomOffset">Random offset applied on spawn of the floating text.</param>
    /// <param name="digitPrecision">Amount of decimals able to show (more decimals will be rounded by <paramref name="typeOfRounding"/>).</param>
    /// <param name="typeOfRounding">Type of rounding used to round the number to the given <paramref name="digitPrecision"/></param>
    /// <seealso cref="SetText(string)"/>
    /// <seealso cref="SetTextColor(Color)"/>
    /// <seealso cref="SetScaleMultiplier(float)"/>
    /// <seealso cref="SetTimeBeforeDestroy(float)"/>
    /// <seealso cref="SetRandomOffset(Vector2)"/>
    /// <seealso cref="SetConfiguration(string, Color?, float?, float?, Vector2?)"/>
    public void SetConfiguration(float number, Color? numberColor = null, float? scaleMultiplier = null, float? timeBeforeDestroy = null, Vector2? randomOffset = null, int? digitPrecision = null, TYPE_OF_ROUNDING? typeOfRounding = null)
    {
        typeOfRounding = typeOfRounding == null ? this.typeOfRounding : typeOfRounding;
        digitPrecision = digitPrecision == null ? this.digitPrecision : digitPrecision;
        float toShow = number * Mathf.Pow(10, (float)digitPrecision);
        switch (typeOfRounding)
        {
            case TYPE_OF_ROUNDING.CEIL:
                toShow = Mathf.Ceil(toShow);
                break;
            case TYPE_OF_ROUNDING.FLOOR:
                toShow = Mathf.Floor(toShow);
                break;
            case TYPE_OF_ROUNDING.ROUND:
                toShow = Mathf.Round(toShow);
                break;
            case TYPE_OF_ROUNDING.TRUNC:
                // https://answers.unity.com/questions/626082/why-is-there-no-mathftruncate.html
                toShow = (int)toShow;
                break;
        }
        SetConfiguration((toShow / Mathf.Pow(10, (float)digitPrecision)).ToString(), numberColor, scaleMultiplier, timeBeforeDestroy, randomOffset);
    }

    /// <summary>
    /// Set the scale multiplier of the floating text canvas.<br/>
    /// Used to override the configuration of the <see cref="FloatingText"/> Prefab.
    /// </summary>
    /// <param name="scale">Scale multiplier to current scale.</param>
    public void SetScaleMultiplier(float scale) => this.scaleMultiplier = scale;

    /// <summary>
    /// Set the text of the floating text.<br/>
    /// Used to override the configuration of the <see cref="FloatingText"/> Prefab.
    /// </summary>
    /// <param name="text">Text to display.</param>
    public void SetText(string text) => this.text.text = text;

    /// <summary>
    /// Set the text color.<br/>
    /// This value will be applied on <see cref="Start()"/>.<br/>
    /// Used to override the configuration of the <see cref="FloatingText"/> Prefab.
    /// </summary>
    /// <param name="textColor">Color of the text.</param>
    public void SetTextColor(Color textColor) => this.textColor = textColor;

    /// <summary>
    /// Set the time countdown before destroy itself.<br/>
    /// If <paramref name="timeBeforeDestroy"/> is 0, the animation duration will be used.<br/>
    /// This value will be applied on <see cref="Start()"/>.<br/>
    /// Used to override the configuration of the <see cref="FloatingText"/> Prefab.
    /// </summary>
    /// <param name="timeBeforeDestroy">Time in seconds before destroy itself.</param>
    public void SetTimeBeforeDestroy(float timeBeforeDestroy) => this.timeBeforeDestroy = timeBeforeDestroy;

    /// <summary>
    /// Set the random offset position of the floating text to avoid spawning it always on the same place.<br/>
    /// This value will be applied on <see cref="Start()"/> using <seealso cref="RandomOffset()"/>.<br/>
    /// Used to override the configuration of the <see cref="FloatingText"/> Prefab.
    /// </summary>
    /// <param name="randomOffset">Random offset applied on spawn of the floating text.</param>
    public void SetRandomOffset(Vector2 randomOffset) => this.randomOffset = randomOffset;

    private Vector2 RandomOffset()
    {
        return new Vector2(Random.Range(-randomOffset.x, randomOffset.x), Random.Range(-randomOffset.y, randomOffset.y));
    }
}
