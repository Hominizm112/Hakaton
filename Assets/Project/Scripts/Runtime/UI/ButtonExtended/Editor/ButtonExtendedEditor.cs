using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ButtonExtended), true)]
public class ButtonExtendedEditor : Editor
{
    private SerializedProperty spriteSwapOverride;
    private SerializedProperty spriteOverride;
    private SerializedProperty textColorSwapOverride;
    private SerializedProperty textColorOverride;

    private void OnEnable()
    {
        spriteSwapOverride = serializedObject.FindProperty("spriteSwapOverride");
        spriteOverride = serializedObject.FindProperty("spriteOverride");
        textColorSwapOverride = serializedObject.FindProperty("textColorSwapOverride");
        textColorOverride = serializedObject.FindProperty("textColorOverride");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        var buttonExtended = (ButtonExtended)target;

        DrawPropertiesExcluding(serializedObject,
            "spriteSwapOverride", "spriteOverride", "textColorSwapOverride", "textColorOverride",
            "mouseDownAnimator", "mouseUpAnimator", "mouseDownAnimation", "mouseUpAnimation",
            "mouseDownAnimatorName", "mouseUpAnimatorName");

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Override Settings", EditorStyles.boldLabel);

        EditorGUILayout.BeginVertical(GUI.skin.box);
        EditorGUILayout.LabelField("Sprite Override", EditorStyles.miniBoldLabel);

        EditorGUILayout.PropertyField(spriteSwapOverride);
        if (spriteSwapOverride.boolValue)
        {
            EditorGUILayout.PropertyField(spriteOverride);

            if (buttonExtended.settings != null && buttonExtended.settings.spriteSwap)
            {
                EditorGUILayout.HelpBox("Override is active and will take precedence over settings sprite swap.", MessageType.Info);
            }
        }
        else if (buttonExtended.settings != null && buttonExtended.settings.spriteSwap)
        {
            EditorGUILayout.HelpBox("Using sprite swap from settings.", MessageType.Info);
        }
        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginVertical(GUI.skin.box);
        EditorGUILayout.LabelField("Text Color Override", EditorStyles.miniBoldLabel);

        EditorGUILayout.PropertyField(textColorSwapOverride);
        if (textColorSwapOverride.boolValue)
        {
            EditorGUILayout.PropertyField(textColorOverride);

            if (buttonExtended.settings != null && buttonExtended.settings.textColorSwap)
            {
                EditorGUILayout.HelpBox("Override is active and will take precedence over settings text color swap.", MessageType.Info);
            }
        }
        else if (buttonExtended.settings != null && buttonExtended.settings.textColorSwap)
        {
            EditorGUILayout.HelpBox("Using text color swap from settings.", MessageType.Info);
        }
        EditorGUILayout.EndVertical();

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Animation Settings", EditorStyles.boldLabel);

        var mouseDownAnimation = serializedObject.FindProperty("mouseDownAnimation");
        var mouseDownAnimator = serializedObject.FindProperty("mouseDownAnimator");
        var mouseUpAnimation = serializedObject.FindProperty("mouseUpAnimation");
        var mouseUpAnimator = serializedObject.FindProperty("mouseUpAnimator");
        var mouseDownAnimatorName = serializedObject.FindProperty("mouseDownAnimatorName");
        var mouseUpAnimatorName = serializedObject.FindProperty("mouseUpAnimatorName");

        EditorGUILayout.PropertyField(mouseDownAnimation);
        if (mouseDownAnimation.boolValue)
        {
            EditorGUILayout.PropertyField(mouseDownAnimator);
            EditorGUILayout.PropertyField(mouseDownAnimatorName);
        }
        else if (mouseDownAnimator.objectReferenceValue != null)
        {
            mouseDownAnimator.objectReferenceValue = null;
            mouseDownAnimatorName.stringValue = string.Empty;
        }

        EditorGUILayout.PropertyField(mouseUpAnimation);
        if (mouseUpAnimation.boolValue)
        {
            EditorGUILayout.PropertyField(mouseUpAnimator);
            EditorGUILayout.PropertyField(mouseUpAnimatorName);
        }
        else if (mouseUpAnimator.objectReferenceValue != null)
        {
            mouseUpAnimator.objectReferenceValue = null;
            mouseUpAnimatorName.stringValue = string.Empty;
        }

        serializedObject.ApplyModifiedProperties();
    }
}