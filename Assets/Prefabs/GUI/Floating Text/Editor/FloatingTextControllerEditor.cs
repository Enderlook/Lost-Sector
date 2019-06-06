using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(FloatingTextController))]
public class FloatingTextcontrollerEditor : Editor
{
    public override void OnInspectorGUI()
    {       
        DrawDefaultInspector();

        // https://answers.unity.com/questions/192895/hideshow-properties-dynamically-in-inspector.html

        FloatingTextController floatingTextController = (FloatingTextController)target;

        EditorGUILayout.Space();
        // https://www.reddit.com/r/Unity3D/comments/3b43pf/unity_editor_scripting_how_can_i_draw_a_header_in/
        EditorGUILayout.LabelField("Floating Text Override Configuration", EditorStyles.boldLabel);
           
        // Draw fields
        ToggleableField(ref floatingTextController.overrideScaleMultiplier, "Scale Multiplier", ref floatingTextController.scaleMultiplier,
            () => EditorGUILayout.FloatField(
                new GUIContent("Scale Multiplier", "Multiply the scale of the canvas by this value."),
                floatingTextController.scaleMultiplier)
        );
        ToggleableField(ref floatingTextController.overrideTextColor, "Text Color", ref floatingTextController.textColor,
            () => EditorGUILayout.ColorField(
                new GUIContent("Text Color", "Text Color."),
                floatingTextController.textColor)
        );
        ToggleableField(ref floatingTextController.overrideTimeBeforeDestroy, "Time Before Destroy", ref floatingTextController.timeBeforeDestroy,
            () => EditorGUILayout.FloatField(
                new GUIContent("Time Before Destroy", "Time before self destroy in seconds. If 0, duration of the animation will be used."),
                floatingTextController.timeBeforeDestroy)
        );
        ToggleableField(ref floatingTextController.overrideRandomOffset, "Random Offset", ref floatingTextController.randomOffset,
            () => EditorGUILayout.Vector2Field(
                new GUIContent("Random Offset", "Random spawn offset."),
                floatingTextController.randomOffset)
        );
        ToggleableField(ref floatingTextController.overrideDigitPrecision, "Digit Precision", ref floatingTextController.digitPrecision,
            () => EditorGUILayout.IntField(
                new GUIContent("Digit Precision", "Digit precision (decimals) for numbers. Whenever a float is given to show, the number is rounded by a certain amount of digits."),
                floatingTextController.digitPrecision)
        );
        ToggleableField(ref floatingTextController.overrideTypeOfRounding, "Type of Rounding", ref floatingTextController.typeOfRounding,
            () => (FloatingText.TYPE_OF_ROUNDING)EditorGUILayout.EnumPopup(
                new GUIContent("Type of Rounding", "Determines how decimal digits are rounded."),
                floatingTextController.typeOfRounding)
        );        
    }

    /// <summary>
    /// Generate a toggleable button to hide or show a certain field, which is also created by this method.
    /// </summary>
    /// <typeparam name="T">Type of return value by <paramref name="func"/>.</typeparam>
    /// <param name="confirmationVariable">Boolean variable to store the toggle check.</param>
    /// <param name="confirmationLabel">Text of label.</param>
    /// <param name="field">Variable to store the changes made by the function.</param>
    /// <param name="toShowfield">Function which creates the field on Unity Inspector.</param>
    /// <seealso cref="CreateToggleButton(bool, string)"/>
    /// <seealso cref="ChangeCheck{T}(Func{T}, ref T, string)"/>
    /// <seealso cref="ChangeCheck{T, T2, T3}(Func{T2, T3, T}, ref T, T2, T3, string)"/>
    private void ToggleableField<T>(ref bool confirmationVariable, string confirmationLabel, ref T field, Func<T> toShowfield)
    {
        // https://www.reddit.com/r/Unity3D/comments/45bjwc/tooltip_on_custom_inspectorproperties/
        ChangeCheck(CreateToggleButton, ref confirmationVariable, confirmationVariable, confirmationLabel, $"Toggle {confirmationLabel}.");
        if (confirmationVariable)
        {            
            EditorGUI.indentLevel++;
            ChangeCheck(toShowfield, ref field, $"Change in {confirmationLabel}.");
            EditorGUI.indentLevel--;
        }
    }

    /// <summary>
    /// Create toggleable button.
    /// </summary>
    /// <param name="confirmationVariable">Boolean variable to store the toggle check.</param>
    /// <param name="confirmationLabel">Text of label.</param>
    /// <returns>Whenever the toggleable button is checked or not.</returns>
    private bool CreateToggleButton(bool confirmationVariable, string confirmationLabel)
    {
        return GUILayout.Toggle(confirmationVariable, new GUIContent($"Override {confirmationLabel}", $"Check to override Floating Text: {confirmationLabel} configuration."));
    }

    /// <summary>
    /// Store the change done in <paramref name="func"/> inside <paramref name="field"/>.
    /// </summary>
    /// <typeparam name="T">Type of return value by <paramref name="func"/>.</typeparam>
    /// <param name="func">Function to execute.</param>
    /// <param name="field">Variable to store the changes made by the function.</param>
    /// <param name="reason">Reason to save.</param>
    /// <seealso cref="ChangeCheck{T, T2, T3}(Func{T2, T3, T}, ref T, T2, T3, string)"/>
    private void ChangeCheck<T>(Func<T> func, ref T field, string reason)
    {
        // https://docs.unity3d.com/ScriptReference/Undo.RecordObject.html
        EditorGUI.BeginChangeCheck();
        T value = func();
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(target, reason);
            field = value;
        }
    }

    /// <summary>
    /// Store the change done in <paramref name="func"/> inside <paramref name="field"/>.<br/>
    /// The function <paramref name="func"/> is executed using the parameters <paramref name="param1"/> and <paramref name="param2"/>.
    /// </summary>
    /// <typeparam name="T">Type of return value by <paramref name="func"/>.</typeparam>
    /// <typeparam name="T2">First parameter passed to <paramref name="func"/>.</typeparam>
    /// <typeparam name="T3">Second parameter passed to <paramref name="func"/></typeparam>
    /// <param name="func">Function to execute.</param>
    /// <param name="field">Variable to store the changes made by the function.</param>
    /// <param name="reason">Reason to save.</param>
    /// <seealso cref="ChangeCheck{T}(Func{T}, ref T, string)"/>
    private void ChangeCheck<T, T2, T3>(Func<T2, T3, T> func, ref T field, T2 param1, T3 param2, string reason)
    {
        EditorGUI.BeginChangeCheck();
        T value = func(param1, param2);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(target, reason);
            field = value;
        }
    }
}
