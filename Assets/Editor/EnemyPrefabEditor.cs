using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(EnemyPrefab))]
public class EnemyPrefabEditor : PropertyDrawer
{
    private const int spriteHeight = 50;
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // https://www.raywenderlich.com/939-extend-the-unity3d-editor
        EditorGUI.PropertyField(position, property, label, true);
        if (property.isExpanded)
        {
            GameObject enemyPrefab = (GameObject)property.FindPropertyRelative("prefab").objectReferenceValue;
            if (enemyPrefab != null)
            {
                SpriteRenderer enemySprite = enemyPrefab.transform.Find("Body").Find("Image").GetComponent<SpriteRenderer>();

                int previousIndentLevel = EditorGUI.indentLevel;
                EditorGUI.indentLevel = 2;

                Rect indentedRect = EditorGUI.IndentedRect(position);
                float fieldHeight = base.GetPropertyHeight(property, label) + EditorGUIUtility.singleLineHeight / 1.5f;
                Vector3 enemySize = enemySprite.bounds.size;
                Rect texturePosition = new Rect(indentedRect.x, indentedRect.y + fieldHeight * 4, enemySize.x / enemySize.y * spriteHeight, spriteHeight);
                EditorGUI.DropShadowLabel(texturePosition, new GUIContent(enemySprite.sprite.texture));

                EditorGUI.indentLevel = previousIndentLevel;
            }
        }
    }
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        GameObject enemyPrefab = (GameObject)property.FindPropertyRelative("prefab").objectReferenceValue;
        if (property.isExpanded && enemyPrefab != null)
        {
            return EditorGUI.GetPropertyHeight(property) + spriteHeight;
        }
        else
        {
            return EditorGUI.GetPropertyHeight(property);
        }
    }
}
