using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(HealthBar), true)]
public class HealthBarEditor : Editor
{
    private HealthBar healthBar;

    private float maxHealth = 100;
    private float currentHealth = 100;

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        healthBar = (HealthBar)target;
        healthBar.Setup();

        // Draw label
        EditorGUILayout.LabelField("Testing Configuration", EditorStyles.boldLabel);

        maxHealth = EditorGUILayout.FloatField("Maximum Health", maxHealth);
        currentHealth = EditorGUILayout.Slider("Current Health", currentHealth, 0, maxHealth);

        healthBar.UpdateValues(currentHealth, maxHealth);

        DrawDefaultInspector();
    }

    private void Update() => healthBar.Update();

    private void OnEnable() => EditorApplication.update += Update;
    private void OnDisable() => EditorApplication.update -= Update;
}