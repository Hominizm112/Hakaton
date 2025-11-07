using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using GameCore.Configs;
using UniRx;
using UnityEngine;
using Zenject;

public class Placer : MonoComponent
{
    [Header("Detection Settings")]
    [SerializeField] private string[] _targetTags = { "" };
    [SerializeField] private string trashcanTag = "";
    [SerializeField] private LayerMask _targetLayers = ~0;

    [Header("Place Settings")]
    [SerializeField] private bool smoothPlace;
    [SerializeField] private float placeDuration;
    [SerializeField] private Ease placeEase;


    private ItemData _containingItem;
    public ItemData ContainingItem => _containingItem;
    private Collider2D _detectionCollider;
    private HashSet<GameObject> _objectsInArea = new HashSet<GameObject>();
    public Action<GameObject> OnObjectEntered;
    public Action<GameObject> OnObjectExited;
    public PlacerConfig placerConfig;
    public bool IsActive => gameObject.activeSelf;


    public bool Placed => _placed;

    public bool Placing
    {
        get
        {
            return _placeTween != null && _placeTween.active;

        }
    }

    private Tween _placeTween;
    private bool _placed;
    private GameObject _lastPlacedAreaDetector;
    private Emitter _emitter;


    public Action<ItemData> onPlace;


    public override void OnConstruct()
    {
        _detectionCollider = GetComponent<Collider2D>();
        SubscribeToEvent<DragEndedEvent>(HandleDragEnded);
        SubscribeToEvent<DragStartedEvent>(HandleDragStarted);

        if (_detectionCollider == null)
        {
            throw new ArgumentNullException("Placer requires a Collider component!");
        }

        _detectionCollider.isTrigger = true;

    }

    public void SetContainingItem(ItemData itemData)
    {
        _containingItem = itemData;
    }


    private void HandleDragStarted(DragStartedEvent e)
    {
        if (e.sender is MonoBehaviour sender)
        {
            if (sender.gameObject == gameObject)
            {
                if (_lastPlacedAreaDetector != null)
                {
                    _lastPlacedAreaDetector.GetComponent<AreaDetector>().TakeItem();
                }
                InterruptPlace();
            }
        }
    }

    private void HandleDragEnded(DragEndedEvent e)
    {
        if (_containingItem == null) return;

        if (e.sender is MonoBehaviour sender)
        {
            if (sender.gameObject == gameObject)
            {
                if (GetObjectsInArea().FirstOrDefault(r => r.tag == trashcanTag))
                {
                    Hide();
                    return;
                }

                var obj = GetObjectsInArea().FirstOrDefault(r => _targetTags.Contains(r.tag));

                PlacerAction placerAction = PlacerAction.PlaceInEmpty;
                if (obj != null && obj.GetComponent<AreaDetector>().AllowedItemTags.Contains(_containingItem.itemTag.Value))
                {
                    placerAction = PlacerAction.PlaceInArea;
                }

                var behaviour = placerConfig.GetAction(placerAction, _containingItem);

                switch (behaviour)
                {
                    case PlacerBehaviourType.Place:
                        Place(obj);
                        break;


                    case PlacerBehaviourType.Hide:
                        Hide();
                        break;

                    case PlacerBehaviourType.Return:
                        Return();
                        break;



                }

            }
        }
    }

    private void Hide()
    {

        if (gameObject.activeSelf)
        {
            if (_emitter == null)
            {
                _emitter = GetComponent<Emitter>();
            }
            _emitter?.Emit();
        }

        gameObject.SetActive(false);
        SetContainingItem(null);
        onPlace?.Invoke(null);
    }

    private void Return()
    {
        if (_lastPlacedAreaDetector != null)
        {
            Place(_lastPlacedAreaDetector);
        }
        else
        {
            gameObject.SetActive(false);
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
                HandlePlaceEnd(obj);

            });
        }
        else
        {
            HandlePlaceEnd(obj);
        }

    }

    private void HandlePlaceEnd(GameObject obj)
    {
        transform.position = obj.transform.position;
        _placed = true;
        onPlace?.Invoke(_containingItem);
        _lastPlacedAreaDetector = obj;
        obj.GetComponent<AreaDetector>().PlaceItem(ContainingItem);

    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (IsValidTarget(other.gameObject))
        {
            _objectsInArea.Add(other.gameObject);
            OnObjectEntered?.Invoke(other.gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (_objectsInArea.Contains(other.gameObject))
        {
            _objectsInArea.Remove(other.gameObject);
            OnObjectExited?.Invoke(other.gameObject);
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

    public List<GameObject> GetObjectsInArea()
    {
        GameObject[] objects = new GameObject[_objectsInArea.Count];
        _objectsInArea.CopyTo(objects);
        return objects.ToList();
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
