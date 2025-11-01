using System;
using System.Collections.Generic;
using DG.Tweening;
using JetBrains.Annotations;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class StallService : MonoService
{
    [Header("Scene References")]
    [SerializeField] private List<ButtonExtended> stallButtons;
    [SerializeField] private Transform draggableItemHolder;
    [SerializeField] private AreaDetector itemPlaceZone;
    [SerializeField] private Transform itemPlacePosition;
    [SerializeField] private Transform finalTeaEndPos;

    [Header("Item Preview Settings")]
    [SerializeField] private GameObject draggableItemPrefab;
    [SerializeField] private Vector2 itemDragOffset;
    [SerializeField] private ParticleSystem itemDeselectedParticleEmitter;

    [Header("Tea Selection For Stall Box")]
    // [SerializeField] private StallBoxUI teaSelectionScreen;

    [Inject] private CustomerService _customerService;
    [Inject] private ShopkeeperService _shopkeeperService;
    // [Inject] private WordBook _wordBook;

    private ButtonExtended _lastSelectedStallBox;
    private ButtonExtended _lastSelectedStallBoxStatic;
    private GameObject _draggableItem;
    private VelocityBasedRotator _velocityBasedRotator;
    private bool _teaSelectionScreenOpen;

    private TeaBase _selectedCommodity;
    public TeaBase SelectedCommodity => _selectedCommodity;

    private bool _canSelectStallBox = true;
    private bool _canSpawnNewCustomer = true;

    private List<TeaFlavorTag> _currentflavors = new();

    #region Init

    [Inject]
    public void Construct()
    {
        SetupHandlers();
        SubscribeToStallButtons();
        CreateDraggableItem();

        RequestCustomer();
    }




    private void SetupHandlers()
    {
        SubscribeToEvent<DragContinuedEvent>(StallItemUpdateDragHandler);
        SubscribeToEvent<DragEndedEvent>(StallItemEndDragHandler);
        SubscribeToEvent<InputActionEvent>(MouseClickHandler);
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
        if (!_canSelectStallBox)
        {
            return;
        }

        _lastSelectedStallBox = stallBox;
        _lastSelectedStallBoxStatic = stallBox;

        if (_teaSelectionScreenOpen)
        {
            // ShowTeaSelectionForStallBox();
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

    public void HideItem(bool bypassInput = false)
    {
        if (InputManager.GetObjectUnderMouse() != _draggableItem && !bypassInput)
        {
            return;
        }
        if (!_draggableItem || !_draggableItem.activeSelf)
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

    public void SetTeaReady(List<TeaFlavorTag> teaFlavors, float quality)
    {
        _canSelectStallBox = false;
        _draggableItem.transform.DOMove(finalTeaEndPos.position, 1f).SetEase(Ease.OutBack);
        _currentflavors = teaFlavors;
    }


    public void SellItem()
    {
        if (_currentflavors.Count != 0)
        {
            _customerService.CustomerAtStall.npc.BuyTea(_currentflavors, CustomerCompletedHandler);
            HideItem(true);
            _currentflavors.Clear();
            _canSelectStallBox = true;
            _shopkeeperService.TryReduceCommodity(_lastSelectedStallBoxStatic.GetComponent<StallBox>().commodity);
            // _wordBook.ResetSelectedWords();
        }
    }

    #endregion


    #region Customer 

    public void RequestCustomer()
    {
        _customerService.RequestCustomer();
    }

    public void CustomerCompletedHandler(NPCBuyResult npcBuyResult)
    {
        _customerService.DespawnCustomer();
        if (_canSpawnNewCustomer)
        {
            _customerService.RequestCustomer();
        }
    }

    #endregion




    #region Tea Selection

    // public void SwitchTeaSelectionScreen()
    // {
    //     teaSelectionScreen.gameObject.SetActive(!teaSelectionScreen.gameObject.activeSelf);
    //     _teaSelectionScreenOpen = teaSelectionScreen.gameObject.activeSelf;

    // }

    // public void ShowTeaSelectionForStallBox()
    // {
    //     Commodity commodity = _lastSelectedStallBox.GetComponent<StallBox>().commodity;
    //     if (commodity != null)
    //     {
    //         teaSelectionScreen.SetCommodity(commodity);
    //     }
    //     else
    //     {
    //         teaSelectionScreen.UnsetCommodity();
    //     }
    // }

    // public void SetSelectedCommodityToStallBox()
    // {
    //     print(teaSelectionScreen.selectedCommodity is TeaBase);
    //     if (teaSelectionScreen.selectedCommodity is TeaBase teaBase)
    //     {
    //         _lastSelectedStallBoxStatic.GetComponent<StallBox>().commodity = teaBase;
    //     }
    // }


    #endregion

    public override void Dispose()
    {
    }


}
