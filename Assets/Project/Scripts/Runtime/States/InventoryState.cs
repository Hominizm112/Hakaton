using System;
using System.Collections.Generic;
using Zenject;

namespace TeaGame.States
{
    public class InventoryState : IDisposable
    {
        private DataStates _currentState = DataStates.Unloaded;
        public DataStates CurrentState => _currentState;

        private InventorySaveData _inventorySaveData;

        private const string INVENTORY_DATA_SAVE_KEY = "inventoryContents";
        [Inject] private SaveManager _saveManager;

        [Inject]
        public void Construct()
        {
            _saveManager.OnSaveStarted += SaveRawInventoryToFile;
        }
        public InventorySaveData LoadRawInventory()
        {
            if (CurrentState == DataStates.Unloaded)
            {
                _inventorySaveData = _saveManager.Load<InventorySaveData>(INVENTORY_DATA_SAVE_KEY);
                _currentState = DataStates.Loaded;
            }
            return _inventorySaveData;
        }

        public void SaveRawInventory(List<ItemSaveData> inventoryData, int capacity = -1)
        {
            if (_inventorySaveData == null)
            {
                _inventorySaveData = new InventorySaveData(inventoryData, capacity);
            }
            else
            {
                _inventorySaveData.Items = inventoryData;
                _inventorySaveData.Capacity = capacity;
            }
        }

        private void SaveRawInventoryToFile()
        {
            _saveManager.Save(INVENTORY_DATA_SAVE_KEY, _inventorySaveData);

        }

        public void Dispose()
        {
            _saveManager.OnSaveStarted -= SaveRawInventoryToFile;
        }




        public struct ItemSaveData
        {
            public string Id;
            public int Quantity;

            public ItemSaveData(string id, int quantity)
            {
                Id = id;
                Quantity = quantity;
            }
        }

        public class InventorySaveData
        {
            public List<ItemSaveData> Items;
            public int Capacity;

            public InventorySaveData(List<ItemSaveData> items, int capacity)
            {
                Items = items;
                Capacity = capacity;
            }
        }
    }

    public enum DataStates
    {
        Unloaded,
        Loaded
    }
}