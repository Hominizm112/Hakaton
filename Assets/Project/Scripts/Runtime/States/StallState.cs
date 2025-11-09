using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;
using Zenject;

namespace TeaGame.States
{
    public class StallState : IDisposable
    {
        [Inject] private SaveManager _saveManager;
        [Inject] private InventoryService _inventoryService;
        [Inject] private CurrencyPresenter _currencyPresenter;

        private Dictionary<string, ItemData> boxesItemsDict = new();

        private const string STALL_BOXES_DATA_SAVE_KEY = "stallBoxesContents";

        public Action onSaveStarted;
        public Action<Dictionary<string, ItemData>> onLoad;

        private DataStates _currentState = DataStates.Unloaded;
        public DataStates CurrentState => _currentState;

        public ReactiveProperty<bool> _isCustomerAtStall = new();
        public IReadOnlyReactiveProperty<bool> IsCustomerAtStall => _isCustomerAtStall;
        public ReactiveCommand ItemSoldCommand = new();


        [Inject]
        public void Construct()
        {
            _saveManager.OnSaveStarted += HandleSaveStarted;
            _saveManager.OnSaveLoaded += HandleSaveLoaded;
        }

        private void HandleSaveStarted()
        {
            onSaveStarted?.Invoke();
        }

        private void HandleSaveLoaded()
        {
            if (CurrentState == DataStates.Unloaded)
            {
                LoadData();
            }
            onLoad?.Invoke(boxesItemsDict);

        }

        public void SaveData(Dictionary<string, ItemData> boxesItems)
        {
            RawData rawData = new();
            foreach (var kvp in boxesItems)
            {
                rawData.boxesIds.Add(kvp.Key);
                rawData.itemsIds.Add(kvp.Value.Id);
            }

            _saveManager.Save(STALL_BOXES_DATA_SAVE_KEY, rawData);

        }

        private void LoadData()
        {
            RawData rawData = _saveManager.Load<RawData>(STALL_BOXES_DATA_SAVE_KEY);
            Dictionary<string, ItemData> cleanData = new();

            for (int i = 0; i < rawData.Count; i++)
            {
                ItemData itemData = _inventoryService.GetItemDataFromId(rawData.itemsIds[i]);

                cleanData.Add(rawData.boxesIds[i], itemData);
            }

            _currentState = DataStates.Loaded;
            boxesItemsDict = cleanData;
        }

        public void SetCustomerAtStall(bool atStall)
        {
            _isCustomerAtStall.Value = atStall;
        }

        public void Sell(ItemData itemData)
        {
            ItemSoldCommand.Execute();
            _currencyPresenter.AddCurrency(itemData.SellPrice);

        }


        public void Dispose()
        {
            _saveManager.OnSaveStarted -= HandleSaveStarted;
            _saveManager.OnSaveLoaded -= HandleSaveLoaded;
        }

        private class RawData
        {
            public List<string> itemsIds = new();
            public List<string> boxesIds = new();
            public int Count => itemsIds.Count;
        }
    }
}