using System.Collections.Generic;
using UnityEngine;

public class FloatingTextController : MonoBehaviour
{
    [Header("Configuration")]
    [Tooltip("Spawning point of prefab. If there are several, a random point will be used.")]
    public Transform[] spawningPoints = new Transform[1];
    [Tooltip("Maximum amount of floating texts at time. New texts will remove old ones. Use 0 for unlimited.")]
    public int maximumAmountFloatingText = 10;

    // https://forum.unity.com/threads/custom-editor-losing-settings-on-play.130889/

    [HideInInspector]
    public bool overrideTimeBeforeDestroy;
    [HideInInspector]
    public float timeBeforeDestroy;
    [HideInInspector]
    public bool overrideTextColor;
    [HideInInspector]
    public Color textColor = Color.red;
    [HideInInspector]
    public bool overrideRandomOffset;
    [HideInInspector]
    public Vector2 randomOffset = Vector2.one;
    [HideInInspector]
    public bool overrideScaleMultiplier;
    [HideInInspector]
    public float scaleMultiplier = 1;
    [HideInInspector]
    public bool overrideDigitPrecision;
    [HideInInspector]
    public int digitPrecision = 0;
    [HideInInspector]
    public bool overrideTypeOfRounding;
    [HideInInspector]
    public FloatingText.TYPE_OF_ROUNDING typeOfRounding = FloatingText.TYPE_OF_ROUNDING.ROUND;

    [Header("Setup")]
    [Tooltip("Floating Text prefab.")]
    public GameObject floatingTextPrefab;
    [Tooltip("Parent transform of all floating texts. Just for organization of scene.\nOptional.\nDO NOT USE A MOVING TRANSFORM!")]
    public Transform floatingTextParent;

    private static Transform floatingTextParentStatic;
    /// <summary>
    /// Set the parent of all Floating Text <see cref="GameObject"/>s spawned by <see cref="FloatingTextController"/>s which <see cref="floatingTextParent"/> is <see langword="null"/>.
    /// </summary>
    /// <param name="floatingTextParent">Parent of all <see cref="FloatingText"/> <see cref="GameObject"/>s.</param>
    public static void SetFloatingTextParentStatic(Transform floatingTextParent) => floatingTextParentStatic = floatingTextParent;
    /// <summary>
    /// Transform used as parent for spawned floating texts.<br/>
    /// <see cref="FloatingTextParent"/> will be returned unless it's <see langword="null"/>. If <see langword="null"/>, <see cref="floatingTextParentStatic"/> will be returned.
    /// </summary>
    private Transform FloatingTextParent {
        get {
            if (floatingTextParent != null)
                return floatingTextParent;
            else
                return floatingTextParentStatic;
        }
    }

    /// <summary>
    /// List of all Floating Text game objects spawned by this <see cref="FloatingTextController"/>.<br/>
    /// The list will only store a number of items equal to <see cref="maximumAmountFloatingText"/>. More items will override old ones.
    /// </summary>
    private List<GameObject> floatingTextList = new List<GameObject>();

    /// <summary>
    /// Spawns a floating text and return its <see cref="FloatingText"/> script.<br/>
    /// </summary>
    /// <returns></returns>
    private FloatingText SpawnFloatingTextBase()
    {
        GameObject floatingText;
        if (floatingTextParent != null)
            floatingText = Instantiate(floatingTextPrefab, FloatingTextParent);
        else
            floatingText = Instantiate(floatingTextPrefab);
        floatingText.transform.position = spawningPoints[Mathf.RoundToInt(Random.Range(0, spawningPoints.Length))].position;

        AddToFloatingTextList(floatingText);
        return floatingText.GetComponent<FloatingText>();
    }

    /// <summary>
    /// Add the <paramref name="floatingText"/> to <see cref="floatingTextList"/>.<br/>
    /// In addition, it checks if the amount of current floating texts is between the allowed by <see cref="maximumAmountFloatingText"/>. If surpassed, it will destroy them.
    /// </summary>
    /// <param name="floatingText"><see cref="GameObject"/> of a Floating Text</param>
    private void AddToFloatingTextList(GameObject floatingText)
    {
        if (maximumAmountFloatingText > 0 && floatingTextList.Count >= maximumAmountFloatingText)
        {
            Destroy(floatingTextList[0]);
            floatingTextList.RemoveAt(0);
        }
        floatingTextList.Add(floatingText);
    }

    /// <summary>
    /// Spawns a floating text.<br/>
    /// All the configuration don't provided in this method will be replaced by the configuration already set on <see cref="FloatingTextController"/>, or, if also null, on the <see cref="floatingTextPrefab"/>.
    /// </summary>
    /// <param name="text">Text to display.</param>
    /// <param name="textColor">Color of the text.</param>
    /// <param name="scaleMultiplier">Scale multiplier to current scale.</param>
    /// <param name="timeBeforeDestroy">Time in seconds before destroy itself.</param>
    /// <param name="randomOffset">Random offset applied on spawn of the floating text.</param>
    public void SpawnFloatingText(string text, Color? textColor = null, float? scaleMultiplier = null, float? timeBeforeDestroy = null, Vector2? randomOffset = null)
    {
        FloatingText floatingTextScript = SpawnFloatingTextBase();

        floatingTextScript.SetConfiguration(text,
            textColor != null ? textColor : this.textColor,
            scaleMultiplier != null ? scaleMultiplier : this.scaleMultiplier,
            timeBeforeDestroy != null ? timeBeforeDestroy : this.timeBeforeDestroy,
            randomOffset != null ? randomOffset : this.randomOffset
        );
    }

    /// <summary>
    /// Spawns a floating text.<br/>
    /// All the configuration don't provided in this method will be replaced by the configuration already set on <see cref="FloatingTextController"/>, or, if also null, on the <see cref="floatingTextPrefab"/>.
    /// </summary>
    /// <param name="number">Number to display.</param>
    /// <param name="numberColor">Color of the number.</param>
    /// <param name="digitPrecision">Amount of decimals able to show (more decimals will be rounded by <paramref name="typeOfRounding"/>).</param>
    /// <param name="scaleMultiplier">Scale multiplier to current scale.</param>
    /// <param name="timeBeforeDestroy">Time in seconds before destroy itself.</param>
    /// <param name="randomOffset">Random offset applied on spawn of the floating text.</param>
    /// <param name="typeOfRounding">Type of rounding used to round the number to the given <paramref name="digitPrecision"/></param>
    public void SpawnFloatingText(float number, Color? numberColor = null, int? digitPrecision = null, float? scaleMultiplier = null, float? timeBeforeDestroy = null, Vector2? randomOffset = null, FloatingText.TYPE_OF_ROUNDING? typeOfRounding = null)
    {
        FloatingText floatingTextScript = SpawnFloatingTextBase();

        floatingTextScript.SetConfiguration(number,
            numberColor != null ? numberColor : textColor,
            scaleMultiplier != null ? scaleMultiplier : this.scaleMultiplier,
            timeBeforeDestroy != null ? timeBeforeDestroy : this.timeBeforeDestroy,
            randomOffset != null ? randomOffset : this.randomOffset,
            digitPrecision != null ? digitPrecision : this.digitPrecision,
            typeOfRounding != null ? typeOfRounding : this.typeOfRounding
        );
    }
}
