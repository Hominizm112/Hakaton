using UnityEngine;
using System.Collections.Generic;

public class AreaDetector : MonoBehaviour
{
    [Header("Detection Settings")]
    [SerializeField] private string[] _targetTags = { "Draggable", "Player", "Item" };
    [SerializeField] private LayerMask _targetLayers = ~0;

    private HashSet<GameObject> _objectsInArea = new HashSet<GameObject>();
    private Collider2D _detectionCollider;

    public System.Action<GameObject> OnObjectEntered;
    public System.Action<GameObject> OnObjectExited;

    private void Awake()
    {
        _detectionCollider = GetComponent<Collider2D>();

        if (_detectionCollider == null)
        {
            Debug.LogError("AreaDetector requires a Collider component!");
            return;
        }

        _detectionCollider.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (IsValidTarget(other.gameObject))
        {
            _objectsInArea.Add(other.gameObject);
            OnObjectEntered?.Invoke(other.gameObject);
            // Debug.Log($"{other.name} entered {gameObject.name}");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (_objectsInArea.Contains(other.gameObject))
        {
            _objectsInArea.Remove(other.gameObject);
            OnObjectExited?.Invoke(other.gameObject);
            // Debug.Log($"{other.name} exited {gameObject.name}");
        }
    }

    private bool IsValidTarget(GameObject obj)
    {
        if (((1 << obj.layer) & _targetLayers) == 0) return false;

        if (_targetTags != null && _targetTags.Length > 0)
        {
            bool hasValidTag = false;
            foreach (string tag in _targetTags)
            {
                if (obj.CompareTag(tag))
                {
                    hasValidTag = true;
                    break;
                }
            }
            if (!hasValidTag) return false;
        }

        return true;
    }

    public bool IsObjectInArea(GameObject obj)
    {
        return _objectsInArea.Contains(obj);
    }

    public GameObject[] GetObjectsInArea()
    {
        GameObject[] objects = new GameObject[_objectsInArea.Count];
        _objectsInArea.CopyTo(objects);
        return objects;
    }

    public T[] GetObjectsInArea<T>() where T : Component
    {
        List<T> components = new List<T>();
        foreach (GameObject obj in _objectsInArea)
        {
            T component = obj.GetComponent<T>();
            if (component != null)
            {
                components.Add(component);
            }
        }
        return components.ToArray();
    }

    public int GetObjectCount()
    {
        return _objectsInArea.Count;
    }

    public void ClearArea()
    {
        _objectsInArea.Clear();
    }
}