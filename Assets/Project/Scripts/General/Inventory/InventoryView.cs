using System.Collections.Generic;
using UniRx;
using UnityEngine;
using Zenject;

public class InventoryView : MonoBehaviour
{
    [SerializeField] private Transform _itemsContainer;
    [SerializeField] private GameObject _itemSlotPrefab;

    [Inject] private InventoryViewModel _viewModel;
    private CompositeDisposable _disposables = new();
    private Dictionary<ItemModel, ItemView> _itemSlots = new();

    [Inject]
    public void Construct()
    {
        SetupBindings();
    }

    private void SetupBindings()
    {
        _viewModel.Items.ObserveAdd()
            .Subscribe(OnItemAdded)
            .AddTo(_disposables);

        _viewModel.Items.ObserveRemove()
            .Subscribe(OnItemRemoved)
            .AddTo(_disposables);

        _viewModel.Items.ObserveReset()
            .Subscribe(_ => ClearItems())
            .AddTo(_disposables);
    }

    private void OnItemAdded(CollectionAddEvent<ItemModel> evt)
    {
        var itemSlot = Instantiate(_itemSlotPrefab, _itemsContainer).GetComponent<ItemView>();
        itemSlot.Initialize(evt.Value, _viewModel);
        _itemSlots[evt.Value] = itemSlot;
    }

    private void OnItemRemoved(CollectionRemoveEvent<ItemModel> evt)
    {
        if (_itemSlots.TryGetValue(evt.Value, out var slot))
        {
            Destroy(slot.gameObject);
            _itemSlots.Remove(evt.Value);
        }
    }

    private void ClearItems()
    {
        foreach (var slot in _itemSlots.Values)
        {
            Destroy(slot.gameObject);
        }
        _itemSlots.Clear();
    }

    private void OnDestroy()
    {
        _disposables?.Dispose();
    }
}
