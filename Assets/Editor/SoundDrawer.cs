using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

// https://docs.unity3d.com/Manual/editor-PropertyDrawers.html

[CustomPropertyDrawer(typeof(Sound))]
public class SoundDrawer : PropertyDrawer
{
    bool foldout;
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        SerializedProperty audioClip = property.FindPropertyRelative("audioClip");
        SerializedProperty minVolume = property.FindPropertyRelative("minVolume");
        SerializedProperty maxVolume = property.FindPropertyRelative("maxVolume");

        //EditorGUI.PropertyField(position, property, label, true);
        // https://answers.unity.com/questions/389986/expandable-property-with-propertydrawer.html
        foldout = EditorGUI.Foldout(position, foldout, label, true);

        if (foldout) //property.isExpanded
        {
            EditorGUI.BeginProperty(position, label, property);

            // Draw label
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            // Don't make child fields be indented
            int indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            position.xMin += EditorGUIUtility.labelWidth;
            position.height = EditorGUIUtility.singleLineHeight;
            //new Rect(position.x, position.y, position.width, position.height)
            EditorGUI.PropertyField(position, audioClip);

            position.y += EditorGUIUtility.singleLineHeight;

            //EditorGUILayout.BeginHorizontal("box");
            //EditorGUI.LabelField(position, "Volume");
            EditorGUI.PropertyField(position, minVolume, GUIContent.none);
            position.y += EditorGUIUtility.singleLineHeight;
            EditorGUI.PropertyField(position, maxVolume, GUIContent.none);
            //EditorGUILayout.EndHorizontal();

            EditorGUI.indentLevel = indent;

            EditorGUI.EndProperty();
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if (foldout)
            return base.GetPropertyHeight(property, label);
        else
            return base.GetPropertyHeight(property, label);
        /*SerializedProperty ite = property.serializedObject.GetIterator();

        float totalHeight = EditorGUI.GetPropertyHeight(property, label, true) + EditorGUIUtility.standardVerticalSpacing;

        while (ite.NextVisible(true))
        {
            totalHeight += EditorGUI.GetPropertyHeight(ite, label, true) + EditorGUIUtility.standardVerticalSpacing;
        }

        return totalHeight;


        /*SerializedObject childObj = new SerializedObject(property.objectReferenceValue);
        SerializedProperty ite = childObj.GetIterator();

        float totalHeight = EditorGUI.GetPropertyHeight(property, label, true) + EditorGUIUtility.standardVerticalSpacing;

        while (ite.NextVisible(true))
        {
            totalHeight += EditorGUI.GetPropertyHeight(ite, label, true) + EditorGUIUtility.standardVerticalSpacing;
        }

        return totalHeight;*/
    }
}
