using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ListContainer", menuName = "Utils/ListContainer")]
public class TypedListContainer : ScriptableObject
{
    [System.Serializable]
    public enum ListType
    {
        Integer,
        Float,
        String,
        Vector3,
        GameObject
    }

    [SerializeField] private ListType _currentType = ListType.Integer;

    [SerializeField] private List<int> _intList = new List<int>();
    [SerializeField] private List<float> _floatList = new List<float>();
    [SerializeField] private List<string> _stringList = new List<string>();
    [SerializeField] private List<Vector3> _vector3List = new List<Vector3>();
    [SerializeField] private List<GameObject> _gameObjectList = new List<GameObject>();

    public ListType CurrentType => _currentType;

    public object GetList()
    {
        switch (_currentType)
        {
            case ListType.Integer: return _intList;
            case ListType.Float: return _floatList;
            case ListType.String: return _stringList;
            case ListType.Vector3: return _vector3List;
            case ListType.GameObject: return _gameObjectList;
            default: return null;
        }
    }

    public List<T> GetList<T>()
    {
        if (typeof(T) == typeof(int) && _currentType == ListType.Integer)
            return _intList as List<T>;
        else if (typeof(T) == typeof(float) && _currentType == ListType.Float)
            return _floatList as List<T>;
        else if (typeof(T) == typeof(string) && _currentType == ListType.String)
            return _stringList as List<T>;
        else if (typeof(T) == typeof(Vector3) && _currentType == ListType.Vector3)
            return _vector3List as List<T>;
        else if (typeof(T) == typeof(GameObject) && _currentType == ListType.GameObject)
            return _gameObjectList as List<T>;

        Debug.LogError($"Type mismatch or unsupported type. Current type: {_currentType}, Requested type: {typeof(T)}");
        return null;
    }
}
