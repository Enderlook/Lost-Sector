using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LivingObject), true), CanEditMultipleObjects]
public class LivingObjectEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        LivingObject livingObject = (LivingObject)target;
        livingObject.startingHealth = EditorGUILayout.Slider("Starting Health", livingObject.startingHealth, -1, livingObject.startingMaxHealth);

        // https://docs.unity3d.com/ScriptReference/EditorGUI.ProgressBar.html
        float percent = livingObject.startingHealth != -1 ? livingObject.startingHealth / livingObject.startingMaxHealth : 1;
        ProgressBar(percent, $"Health ({Mathf.Round(percent * 100)}%)");

        DrawDefaultInspector();
        serializedObject.ApplyModifiedProperties();
    }

    void ProgressBar(float value, string label)
    {
        // https://docs.unity3d.com/ScriptReference/Editor.html
        // Get a rect for the progress bar using the same margins as a textfield:
        Rect rect = GUILayoutUtility.GetRect(18, 18, "TextField");
        EditorGUI.ProgressBar(rect, value, label);
        EditorGUILayout.Space();
    }
}
