using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/* https://www.youtube.com/watch?v=RInUu1_8aGw
 * https://answers.unity.com/questions/1513969/iterate-through-object-properties.html
 * https://stackoverflow.com/questions/31860798/unityscript-how-to-loop-through-public-properties-of-a-class
 * https://docs.unity3d.com/2017.3/Documentation/ScriptReference/SerializedProperty.html
 * 
 * https://forum.unity.com/threads/enumarting-serializedproperty-throws-an-exception.511310/
 * 
 * https://docs.unity3d.com/ScriptReference/CanEditMultipleObjects.html
 */

[CustomEditor(typeof(MonoBehaviour))]
public class HeaderEditor : Editor
{
    public override void OnInspectorGUI()
    {
        /*foreach(var thisVar in target.GetType().GetFields())
        {
            Debug.Log($"a {thisVar.Name}: {thisVar.GetValue(target)}");
        }*/

        Object thisObject = CreateInstance(target.GetType());
        SerializedObject serializedObject = new SerializedObject(thisObject);
        //serializedObject.Update();

        //SerializedProperty serializedProperty = serializedObject.GetIterator();
        //serializedProperty.Next(true);

        foreach (SerializedProperty thisVar in serializedObject.GetIterator())
        {
            Debug.Log($"{thisVar.displayName}: {thisVar.type}");
        }

        Debug.Log("HI");
        base.OnInspectorGUI();
    }
}
