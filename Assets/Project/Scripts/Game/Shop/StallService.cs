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
    [SerializeField] private Transform itemPlacePosition;


    [Header("Item Preview Settings")]
    [SerializeField] private GameObject draggableItemPrefab;
    [SerializeField] private Vector2 itemDragOffset;
    [SerializeField] private ParticleSystem itemDeselectedParticleEmitter;

    [Header("Tea Selection For Stall Box")]
    [SerializeField] private StallBoxUI teaSelectionScreen;


    public override List<Type> requiredServices { get; protected set; } = new() { typeof(CustomerService) };
    private ButtonExtended _lastSelectedStallBox;
    private ButtonExtended _lastSelectedStallBoxStatic;
    private GameObject _draggableItem;
    private VelocityBasedRotator _velocityBasedRotator;
    private bool _teaSelectionScreenOpen;

    private TeaBase _selectedCommodity;
    public TeaBase SelectedCommodity => _selectedCommodity;

    private CustomerService _customerService;

    #region Init

    private void Start()
    {
        Mediator.Instance.RegisterService(this);
    }

    public override void Initialize(Mediator mediator)
    {
        base.Initialize(mediator);
        _customerService = mediator.GetService<CustomerService>();

        SetupHandlers();
        SubscribeToStallButtons();
        CreateDraggableItem();

        RequestCustomer();
    }




    private void SetupHandlers()
    {
        AddEvent(_mediator.GlobalEventBus.Subscribe<DragContinuedEvent>(StallItemUpdateDragHandler));
        AddEvent(_mediator.GlobalEventBus.Subscribe<DragEndedEvent>(StallItemEndDragHandler));
        AddEvent(_mediator.GlobalEventBus.Subscribe<InputActionEvent>(MouseClickHandler));
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
        _lastSelectedStallBox = stallBox;
        _lastSelectedStallBoxStatic = stallBox;

        if (_teaSelectionScreenOpen)
        {
            ShowTeaSelectionForStallBox();
            return;
        }


        Vector2 newPoint = _lastSelectedStallBox.transform.position;
        _draggableItem.transform.position = newPoint + itemDragOffset;
        _velocityBasedRotator.OnDragStart();

    }


    private void MouseClickHandler(InputActionEvent @event)
    {
        if (@event.ActionName != "PointerClick" || !@event.Context.performed)
        {
            return;
        }


    }


    private void StallItemUpdateDragHandler(DragContinuedEvent @event)
    {
        if (_lastSelectedStallBox == null || _teaSelectionScreenOpen)
        {
            return;
        }


        _draggableItem.SetActive(true);
        Vector2 newPoint = Camera.main.ScreenToWorldPoint(@event.ScreenPosition);
        _draggableItem.transform.position = newPoint + itemDragOffset;
        _velocityBasedRotator.OnDragContinue(@event.Velocity);
    }

    private void StallItemEndDragHandler(DragEndedEvent @event)
    {
        if (itemPlaceZone.IsObjectInArea(_draggableItem) && _lastSelectedStallBox?.GetComponent<StallBox>().commodity is TeaBase)
        {
            PlaceItem();
        }
        else
        {
            HideItem();
        }

        _velocityBasedRotator.OnDragEnd();
        _lastSelectedStallBox = null;
    }

    private void PlaceItem()
    {
        _selectedCommodity = _lastSelectedStallBox.GetComponent<StallBox>().commodity as TeaBase;
        _draggableItem.transform.DOMove(itemPlacePosition.transform.position, 0.5f).SetEase(Ease.OutBack);
    }

    private void HideItem()
    {
        if (InputManager.GetObjectUnderMouse() != _draggableItem)
        {
            return;
        }
        if (!_draggableItem.activeSelf)
        {
            return;
        }
        _draggableItem.SetActive(false);

        itemDeselectedParticleEmitter.transform.position = _draggableItem.transform.position;
        itemDeselectedParticleEmitter.Play();

        if (_selectedCommodity != null)
        {
            _mediator.GlobalEventBus.Publish<TeaRemovedFromSelectionEvent>(new());
        }
        _selectedCommodity = null;

    }


    #endregion

    #region Tea Selection

    public void SwitchTeaSelectionScreen()
    {
        teaSelectionScreen.gameObject.SetActive(!teaSelectionScreen.gameObject.activeSelf);
        _teaSelectionScreenOpen = teaSelectionScreen.gameObject.activeSelf;

    }

    public void ShowTeaSelectionForStallBox()
    {
        Commodity commodity = _lastSelectedStallBox.GetComponent<StallBox>().commodity;
        if (commodity != null)
        {
            teaSelectionScreen.SetCommodity(commodity);
        }
        else
        {
            teaSelectionScreen.UnsetCommodity();
        }
    }

    public void SetSelectedCommodityToStallBox()
    {
        print(teaSelectionScreen.selectedCommodity is TeaBase);
        if (teaSelectionScreen.selectedCommodity is TeaBase teaBase)
        {
            _lastSelectedStallBoxStatic.GetComponent<StallBox>().commodity = teaBase;
        }
    }


    #endregion

    #region Customer 

    public void RequestCustomer()
    {
        _customerService.RequestCustomer();
    }

    public void CustomerCompletedHandler()
    {

    }


    #endregion

    public override void Dispose()
    {
        _mediator.UnregisterService(this);
    }


}
