using UnityEditor;
using UnityEngine;

//[CustomEditor(typeof(GameOverMenu))]
public class GameOverMenuEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        GameOverMenu gameOverMenu = (GameOverMenu)target;

        gameOverMenu.HasWon = EditorGUILayout.Toggle(new GUIContent("Has Won", "On true, it will show the win menu. On false, it will show the loose menu."), gameOverMenu.HasWon);
        //gameOverMenu.SetConfiguration(gameOverMenu.hasWon, 1234, 1234);
    }
}