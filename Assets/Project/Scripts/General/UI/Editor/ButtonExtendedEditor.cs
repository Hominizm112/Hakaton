using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(ButtonExtended), true)]
public class ButtonExtendedEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        DrawPropertiesExcluding(serializedObject,
            new string[] { "mouseDownAnimator", "mouseUpAnimator", "mouseDownAnimation", "mouseUpAnimation", "mouseDownAnimatorName", "mouseUpAnimatorName" });

        var mouseDownAnimation = serializedObject.FindProperty("mouseDownAnimation");
        var mouseDownAnimator = serializedObject.FindProperty("mouseDownAnimator");
        var mouseUpAnimation = serializedObject.FindProperty("mouseUpAnimation");
        var mouseUpAnimator = serializedObject.FindProperty("mouseUpAnimator");
        var mouseDownAnimatorName = serializedObject.FindProperty("mouseDownAnimatorName");
        var mouseUpAnimatorName = serializedObject.FindProperty("mouseUpAnimatorName");

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Animation Settings", EditorStyles.boldLabel);

        EditorGUILayout.PropertyField(mouseDownAnimation);
        if (mouseDownAnimation.boolValue)
        {
            EditorGUILayout.PropertyField(mouseDownAnimator);
            EditorGUILayout.PropertyField(mouseDownAnimatorName);
        }
        else if (mouseDownAnimator.objectReferenceValue != null)
        {
            mouseDownAnimator.objectReferenceValue = null;
            mouseDownAnimatorName.objectReferenceValue = null;
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
            mouseUpAnimatorName.objectReferenceValue = null;

        }

        serializedObject.ApplyModifiedProperties();
    }
}
