using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class RedistributeRectTransforms : EditorWindow
{
    [MenuItem("Customs/Distribute Rect Transforms")]
    private static void Init()
    {
        EditorWindow window = GetWindow(typeof(RedistributeRectTransforms));
        window.Show();
    }

    private void OnGUI()
    {
        List<string> lackOfRectTransform = new List<string>();
        foreach (GameObject gameObject in Selection.gameObjects)
        {
            if (gameObject.GetComponent<RectTransform>() == null)
                lackOfRectTransform.Add(gameObject.name);
        }

        bool error = lackOfRectTransform.Count > 0;

        if (error)
        {
            EditorGUILayout.LabelField("The selected game objects on Editor must have a Rect Transform component.");
            EditorGUILayout.LabelField("The following game objects lacks of Rect Transform component:");
            foreach (string name in lackOfRectTransform)
            {
                EditorGUILayout.LabelField($"   • {name}");
            }
        }

        if (Selection.gameObjects.Length < 3)
        {
            error = true;
            EditorGUILayout.LabelField("You must select at least 3 game objects.");
        }

        EditorGUI.BeginDisabledGroup(error);
        if (GUILayout.Button("Redistribute Vertical"))
            MakeUndoGroup(() => Redistribute(true));
        if (GUILayout.Button("Redistribute Horizontal"))
            MakeUndoGroup(() => Redistribute(false));
        if (GUILayout.Button("Redistribute Vertical And Horizontal"))
            MakeUndoGroup(() => Redistribute(true), () => Redistribute(false));

        EditorGUI.EndDisabledGroup();
    }
    private void MakeUndoGroup(params Action[] actions)
    {
        // https://forum.unity.com/threads/undo-create-group.309218/
        Undo.SetCurrentGroupName("Redistribute rect transforms anchors");
        int group = Undo.GetCurrentGroup();

        foreach(Action action in actions)
        {
            action();
        }

        Undo.CollapseUndoOperations(group);
    }

    private void Redistribute(bool doVertical)
    { 
        int amount = Selection.gameObjects.Length;
        IOrderedEnumerable<RectTransform> rectTransforms = Selection.gameObjects.Select(e => e.GetComponent<RectTransform>()).OrderBy(e => doVertical ? e.anchorMax.y : e.anchorMax.x);

        RectTransform top = rectTransforms.Last();
        RectTransform bottom = rectTransforms.First();
        float topHeight = doVertical ? top.anchorMax.y : top.anchorMax.x;
        float bottomHeight = doVertical ? bottom.anchorMax.y : bottom.anchorMax.x;
        float offset = (topHeight - bottomHeight) / (amount - 1);    

        int index = -1;
        foreach (RectTransform rectTransform in rectTransforms)
        {
            Undo.RecordObject(rectTransform, "Rect transform to redistribute");
            Vector2 min = rectTransform.anchorMax - rectTransform.anchorMin;
            rectTransform.anchorMax = new Vector2(doVertical ? rectTransform.anchorMax.x : top.anchorMax.x - top.anchorMin.x + offset * index++, doVertical ? top.anchorMax.y - top.anchorMin.y + offset * index++ : rectTransform.anchorMax.y);
            rectTransform.anchorMin = rectTransform.anchorMax - min;
        }

    }
}
