#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ButtonEventHandler))]
public class ButtonEventHandlerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        ButtonEventHandler handler = (ButtonEventHandler)target;

        SerializedProperty eventTypeProp = serializedObject.FindProperty("eventType");
        EditorGUILayout.PropertyField(eventTypeProp);

        switch (handler.eventType)
        {
            case EventFactory.EventType.LoadScene:
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_sceneName"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_targetState"));
                break;
        }

        serializedObject.ApplyModifiedProperties();
    }
}

#endif