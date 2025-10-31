using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;
using Zenject;

[System.Serializable]
public class InventoryData
{
    public List<ItemModel> Items = new();
    public int Capacity = -1;
}
public interface IInventoryService
{
    IReadOnlyReactiveCollection<ItemModel> Items { get; }
    ReactiveCommand<ItemModel> OnItemAdded { get; }
    ReactiveCommand<ItemModel> OnItemRemoved { get; }

    bool AddItem(ItemModel item);
    bool RemoveItem(string itemId);
    bool HasItem(string itemId);
    int GetItemCount(string itemId);
}
public class InventoryService : EventListener, IInventoryService, IDisposable
{
    [Inject] private readonly SaveManager _saveManager;

    private readonly ReactiveCollection<ItemModel> _items = new();
    private readonly ReactiveCommand<ItemModel> _onItemAdded = new();
    private readonly ReactiveCommand<ItemModel> _onItemRemoved = new();

    private InventoryData _inventoryData = new();

    public IReadOnlyReactiveCollection<ItemModel> Items => _items;
    public ReactiveCommand<ItemModel> OnItemAdded => _onItemAdded;
    public ReactiveCommand<ItemModel> OnItemRemoved => _onItemRemoved;

    [Inject]
    public void Construct()
    {
        SubscribeToEvent<LoadDataEvent>(_ => LoadInventory());
    }

    public override void Dispose()
    {
        base.Dispose();
        _onItemAdded?.Dispose();
        _onItemRemoved?.Dispose();
    }

    public bool AddItem(ItemModel item)
    {
        if (_items.Count >= _inventoryData.Capacity && _inventoryData.Capacity != -1)
            return false;

        var existingItem = _items.FirstOrDefault(x => x.Id == item.Id);
        if (existingItem != null)
        {
            existingItem.Quantity.Value += item.Quantity.Value;
        }
        else
        {
            _items.Add(item);
            _onItemAdded.Execute(item);
        }

        SaveInventory();
        return true;
    }

    public bool RemoveItem(string itemId)
    {
        var item = _items.FirstOrDefault(x => x.Id == itemId);
        if (item != null)
        {
            _items.Remove(item);
            _onItemRemoved.Execute(item);
            SaveInventory();
            return true;
        }
        return false;
    }


    public bool HasItem(string itemId) => _items.Any(x => x.Id == itemId);

    public int GetItemCount(string itemId) =>
        _items.Where(x => x.Id == itemId).Sum(x => x.Quantity.Value);


    private void LoadInventory()
    {
        _inventoryData = _saveManager.Load<InventoryData>("inventory") ?? new InventoryData();
        Debug.Log(_inventoryData.Items.Count);

        foreach (var item in _inventoryData.Items)
        {
            _items.Add(item);
        }
    }

    private void SaveInventory()
    {
        _inventoryData.Items = _items.ToList();
        _saveManager.Save("inventory", _inventoryData);
    }

}
