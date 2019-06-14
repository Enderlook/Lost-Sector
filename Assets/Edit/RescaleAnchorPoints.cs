using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class RescaleAnchorPoints : EditorWindow
{
    private float xScale, yScale;
    private List<Vector2> oldAnchors = new List<Vector2>();

    [MenuItem("Customs/Editor Anchor Scale")]
    private static void Init()
    {
        EditorWindow window = GetWindow(typeof(RescaleAnchorPoints));
        window.Show();
    }

    private void OnGUI()
    {
        RectTransform rectTransform = Selection.activeGameObject.GetComponent<RectTransform>();
        if (rectTransform == null)
            EditorGUILayout.LabelField("The active game object on Inspector must have a Rect Transform component.");

        
        xScale = EditorGUILayout.FloatField("X Rescale Factor", xScale);
        yScale = EditorGUILayout.FloatField("Y Rescale Factor", yScale);

        EditorGUI.BeginDisabledGroup(rectTransform == null);
        if (GUILayout.Button("Rescale"))
            oldAnchors.Add(Rescale(rectTransform, xScale, yScale));
        EditorGUI.EndDisabledGroup();

        EditorGUI.BeginDisabledGroup(oldAnchors.Count < 1);
        if (GUILayout.Button("Undo"))
        {
            SetMaxAnchors(rectTransform, oldAnchors[oldAnchors.Count - 1]);
            oldAnchors.RemoveAt(oldAnchors.Count - 1);
        }
        EditorGUI.EndDisabledGroup();
    }

    /// <summary>
    /// Scale <seealso cref="RectTransform.anchorMax"/> from <paramref name="rectTransform"/> taking into account the offset produced by <seealso cref="RectTransform.anchorMin"/> by <paramref name="xScale"/> and <paramref name="yScale"/>.
    /// </summary>
    /// <param name="rectTransform"><seealso cref="RectTransform"/> to scale.</param>
    /// <param name="xScale">X Multiplier.</param>
    /// <param name="yScale">Y Multiplier.</param>
    /// <returns>New anchor max.</returns>
    private Vector2 Rescale(RectTransform rectTransform, float xScale, float yScale)
    {
        Vector2 anchorMax = rectTransform.anchorMax;
        SetMaxAnchors(rectTransform, ScaleAnchor(rectTransform.anchorMin.x, rectTransform.anchorMax.x, xScale), ScaleAnchor(rectTransform.anchorMin.y, rectTransform.anchorMax.y, yScale));
        return anchorMax;
    }

    /// <summary>
    /// Scale a given anchor.
    /// </summary>
    /// <param name="min">Min value of anchor.</param>
    /// <param name="max">Max value of anchor.</param>
    /// <param name="scale">Scale to multiply.</param>
    /// <returns>New max anchor value.</returns>
    private float ScaleAnchor(float min, float max, float scale) => (max - min) * scale + min;

    /// <summary>
    /// Set <seealso cref="RectTransform.anchorMax"/> of <paramref name="rectTransform"/>.
    /// </summary>
    /// <param name="rectTransform"><seealso cref="RectTransform"/> to be modified.</param>
    /// <param name="anchorMax">New anchors.</param>
    private void SetMaxAnchors(RectTransform rectTransform, Vector2 anchorMax) => rectTransform.anchorMax = anchorMax;
    /// <summary>
    /// Set <seealso cref="RectTransform.anchorMax"/> of <paramref name="rectTransform"/>.
    /// </summary>
    /// <param name="rectTransform"><seealso cref="RectTransform"/> to be modified.</param>
    /// <param name="anchorX">New x anchor.</param>
    /// <param name="anchorY">New y anchor.</param>
    private void SetMaxAnchors(RectTransform rectTransform, float anchorX, float anchorY) => SetMaxAnchors(rectTransform, new Vector2(anchorX, anchorY));
}
