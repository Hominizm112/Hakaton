using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using GameCore.Runtime.Utils;
using TeaGame.States;
using UniRx;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Zenject;

[System.Serializable]
public class InventoryData
{
    public ReactiveCollection<ItemData> Items = new(new());
    public int Capacity = -1;
}
public interface IInventoryService
{
    bool AddItem(ItemData item, int quantity = -1);
    bool RemoveItem(string itemId, int quantity = 1);
    bool HasItem(string itemId);
    int GetItemCount(string itemId);
}
public class InventoryService : EventListener, IInventoryService, IDisposable
{
    [Inject] private readonly InventoryState _inventoryState;
    [Inject] private SaveManager _saveManager;


    private InventoryData _inventoryData = new();
    public InventoryData InventoryData => _inventoryData;


    private CompositeDisposable _disposables = new();
    private List<Action> _onDispose = new();

    private List<ItemData> _itemDatas = new();

    private const string ITEMS_ADDRESSABLE_LABEL = "Item";


    [Inject]
    public async void Construct()
    {

        _itemDatas = await LoadItemsAsync();

        _saveManager.OnSaveLoaded += LoadItems;
        _saveManager.OnSaveStarted += SaveItems;

        // _onDispose.Add(() => _saveManager.OnSaveLoaded -= LoadItems);
        // _onDispose.Add(() => _saveManager.OnSaveStarted -= SaveItems);
    }

    public override void Dispose()
    {
        _disposables.Dispose();
        foreach (var @event in _onDispose)
        {
            @event?.Invoke();
        }
    }

    public void LoadItems()
    {
        var rawInventory = _inventoryState.LoadRawInventory();
        _inventoryData.Items.Clear();
        _inventoryData.Capacity = rawInventory.Capacity;
        foreach (var rawItem in rawInventory.Items)
        {
            AddItem(rawItem.Id, rawItem.Quantity);
        }

    }

    public void SaveItems()
    {
        List<InventoryState.ItemSaveData> itemIds = new();
        foreach (var item in _inventoryData.Items)
        {
            itemIds.Add(new(item.Id, item.Quantity.Value));
        }

        _inventoryState.SaveRawInventory(itemIds, _inventoryData.Capacity);
    }


    public async UniTask<List<ItemData>> LoadItemsAsync()
    {
        var handle = Addressables.LoadAssetsAsync<ItemTemplate>(ITEMS_ADDRESSABLE_LABEL, null);
        await handle.Task;

        if (handle.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
        {
            List<ItemTemplate> rawItems = handle.Result.ToList();
            List<ItemData> items = new();

            foreach (var item in rawItems)
            {
                items.Add(item.itemData);
            }
            return items;

        }

        return null;

    }

    public ItemData GetItemDataFromId(string itemId)
    {
        return _itemDatas.Find(r => r.Id == itemId);
    }

    public bool AddItem(string itemId, int quantity = 1)
    {
        ItemData item = Copium.CreateDeepCopy(_itemDatas.Find(r => r.Id == itemId));

        if (item != null)
        {
            return AddItem(item, quantity);
        }

        return false;
    }

    public bool AddItem(ItemData item, int quantity = 1)
    {
        if (_inventoryData.Items.Count >= _inventoryData.Capacity && _inventoryData.Capacity != -1)
            return false;

        var existingItem = _inventoryData.Items.FirstOrDefault(x => x.Id == item.Id);
        if (existingItem != null)
        {
            existingItem.Quantity.Value += quantity;
        }
        else
        {
            item.Quantity.Value = quantity;
            _inventoryData.Items.Add(item);
        }

        SaveItems();
        return true;
    }


    public bool RemoveItem(string itemId, int quantity = 1)
    {
        var item = _inventoryData.Items.FirstOrDefault(x => x.Id == itemId);
        if (item != null)
        {
            if (item.Quantity.Value - quantity < 0)
            {
                return false;
            }

            item.Quantity.Value -= quantity;
            if (item.Quantity.Value == 0)
            {
                _inventoryData.Items.Remove(item);
            }
            SaveItems();
            return true;
        }
        return false;
    }


    public bool HasItem(string itemId) => _inventoryData.Items.Any(x => x.Id == itemId);

    public int GetItemCount(string itemId) =>
        _inventoryData.Items.Where(x => x.Id == itemId).Sum(x => x.Quantity.Value);


    public void CreateFetchingLink(Action<ItemData> callback)
    {

    }




    [Serializable]
    public struct CollectionActionData
    {
        public ItemData ItemData;
        public CollectionAction CollectionAction;

        public CollectionActionData(ItemData itemData, CollectionAction collectionAction)
        {
            ItemData = itemData;
            CollectionAction = collectionAction;
        }
    }

    public enum CollectionAction
    {
        Add,
        Remove
    }

}
