using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.UI;

public class StallService : MonoService
{
    [Header("Scene References")]
    [SerializeField] private List<ButtonExtended> stallButtons;
    [SerializeField] private Transform draggableItemHolder;
    [SerializeField] private AreaDetector itemPlaceZone;
    [Header("Prefabs")]
    [SerializeField] private GameObject draggableItemPrefab;
    private Mediator _mediator;
    private ButtonExtended _lastSelectedStallBox;
    private GameObject _draggableItem;
    private VelocityBasedRotator _velocityBasedRotator;

    #region Init
    public void Start()
    {
        _mediator = Mediator.Instance;
        _mediator.RegisterService(this);
        SetupHandlers();
        SubscribeToStallButtons();
        CreateDraggableItem();
    }



    private void SetupHandlers()
    {
        _mediator.GlobalEventBus.Subscribe<DragContinuedEvent>(StallItemUpdateDragHandler);
        _mediator.GlobalEventBus.Subscribe<DragEndedEvent>(StallItemEndDragHandler);
        _mediator.GlobalEventBus.Subscribe<InputActionEvent>(MouseClickHandler);
    }

    private void SubscribeToStallButtons()
    {
        foreach (var item in stallButtons)
        {
            item.OnMouseDownWithReference += StallBoxSelect;
        }
    }

    private void CreateDraggableItem()
    {
        _draggableItem = Instantiate(draggableItemPrefab, draggableItemHolder);
        _draggableItem.SetActive(false);
        _velocityBasedRotator = _draggableItem.GetComponent<VelocityBasedRotator>();
    }

    #endregion

    #region Handlers

    private void StallBoxSelect(ButtonExtended stallBox)
    {
        print("selected stall box");
        _lastSelectedStallBox = stallBox;


        Vector2 newPoint = _lastSelectedStallBox.transform.position;
        _draggableItem.transform.position = newPoint;
        _velocityBasedRotator.OnDragStart();

    }


    private void MouseClickHandler(InputActionEvent @event)
    {
        if (@event.ActionName != "PointerClick" || !@event.Context.performed)
        {
            return;
        }

        if (InputManager.GetObjectUnderMouse() == itemPlaceZone.gameObject)
        {
            _draggableItem.SetActive(false);
        }
    }


    private void StallItemUpdateDragHandler(DragContinuedEvent @event)
    {
        if (_lastSelectedStallBox == null)
        {
            return;
        }

        _draggableItem.SetActive(true);
        Vector2 newPoint = Camera.main.ScreenToWorldPoint(@event.ScreenPosition);
        _draggableItem.transform.position = newPoint;
        _velocityBasedRotator.OnDragContinue(@event.Velocity);
    }

    private void StallItemEndDragHandler(DragEndedEvent @event)
    {
        if (itemPlaceZone.IsObjectInArea(_draggableItem))
        {
            PlaceItem();
        }
        else
        {
            _draggableItem.SetActive(false);
        }

        _velocityBasedRotator.OnDragEnd();
        _lastSelectedStallBox = null;
    }

    private void PlaceItem()
    {
        _draggableItem.transform.DOMove(itemPlaceZone.transform.position, 0.5f).SetEase(Ease.OutBack);
    }


    #endregion

}
