using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Draggable2D : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] public DropZone[] dropZones;
    [SerializeField] public string[] states;

    private string _currentState;
    public string CurrentState => _currentState;

    public void SetState(string newState)
    {
        _currentState = newState;
    }

    public void PlaceInDropZone()
    {
        foreach (var item in dropZones)
        {
            if (item.areaDetector.IsObjectInArea(gameObject))
            {
                transform.DOMove(item.placePosition.transform.position, 0.5f).SetEase(Ease.OutBack);
            }
        }
    }
}


[Serializable]
public struct DropZone
{
    public AreaDetector areaDetector;
    public Transform placePosition;
    public string draggableState;
}

