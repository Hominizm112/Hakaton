#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TypedListContainer))]
public class TypedListContainerEditor : Editor
{
    private SerializedProperty _currentTypeProp;
    private SerializedProperty _intListProp;
    private SerializedProperty _floatListProp;
    private SerializedProperty _stringListProp;
    private SerializedProperty _vector3ListProp;
    private SerializedProperty _gameObjectListProp;

    private void OnEnable()
    {
        _currentTypeProp = serializedObject.FindProperty("_currentType");
        _intListProp = serializedObject.FindProperty("_intList");
        _floatListProp = serializedObject.FindProperty("_floatList");
        _stringListProp = serializedObject.FindProperty("_stringList");
        _vector3ListProp = serializedObject.FindProperty("_vector3List");
        _gameObjectListProp = serializedObject.FindProperty("_gameObjectList");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // Draw the type selector
        EditorGUILayout.PropertyField(_currentTypeProp);

        // Draw the appropriate list based on the selected type
        switch ((TypedListContainer.ListType)_currentTypeProp.enumValueIndex)
        {
            case TypedListContainer.ListType.Integer:
                EditorGUILayout.PropertyField(_intListProp, true);
                break;
            case TypedListContainer.ListType.Float:
                EditorGUILayout.PropertyField(_floatListProp, true);
                break;
            case TypedListContainer.ListType.String:
                EditorGUILayout.PropertyField(_stringListProp, true);
                break;
            case TypedListContainer.ListType.Vector3:
                EditorGUILayout.PropertyField(_vector3ListProp, true);
                break;
            case TypedListContainer.ListType.GameObject:
                EditorGUILayout.PropertyField(_gameObjectListProp, true);
                break;
        }

        serializedObject.ApplyModifiedProperties();
    }
}
#endif