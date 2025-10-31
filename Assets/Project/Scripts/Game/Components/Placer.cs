using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

public class Placer : MonoComponent
{
    [Header("Detection Settings")]
    [SerializeField] private string[] _targetTags = { "" };
    [SerializeField] private LayerMask _targetLayers = ~0;

    [Header("Place Settings")]
    [SerializeField] private bool smoothPlace;
    [SerializeField] private float placeDuration;
    [SerializeField] private Ease placeEase;

    private Collider2D _detectionCollider;
    private HashSet<GameObject> _objectsInArea = new HashSet<GameObject>();
    public System.Action<GameObject> OnObjectEntered;
    public System.Action<GameObject> OnObjectExited;

    private Tween _placeTween;

    private bool _placed;
    public bool Placed => _placed;

    private void Awake()
    {
        _detectionCollider = GetComponent<Collider2D>();
        SubscribeToEvent<DragEndedEvent>(HandleDragEnded);
        SubscribeToEvent<DragStartedEvent>(HandleDragStarted);

        if (_detectionCollider == null)
        {
            Debug.LogError("Placer requires a Collider component!");
            return;
        }

        _detectionCollider.isTrigger = true;
    }


    private void HandleDragStarted(DragStartedEvent e)
    {
        if (e.sender is MonoBehaviour sender)
        {
            if (sender.gameObject == gameObject)
            {
                InterruptPlace();
            }
        }
    }

    private void HandleDragEnded(DragEndedEvent e)
    {
        if (e.sender is MonoBehaviour sender)
        {
            if (sender.gameObject == gameObject)
            {
                var obj = GetObjectsInArea().FirstOrDefault(r => _targetTags.Contains(r.tag));
                if (obj != null)
                {
                    Place(obj);
                }
            }
        }
    }

    private void InterruptPlace()
    {
        _placed = false;
        _placeTween.Kill();
        _placeTween = null;
    }

    private void Place(GameObject obj)
    {
        if (smoothPlace)
        {
            _placeTween = transform.DOMove(obj.transform.position, placeDuration).SetEase(placeEase).OnComplete(() =>
            {
                _placeTween = null;
                _placed = true;
            });
        }
        else
        {
            transform.position = obj.transform.position;
            _placed = true;

        }
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
