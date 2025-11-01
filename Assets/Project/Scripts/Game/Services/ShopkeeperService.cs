using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

public class ShopkeeperService : MonoService
{

    [Inject] private SaveManager _saveService;
    public List<CommodityEntry> _playerCommodities = new();

    public List<CommodityEntry> GetAvailableCommodities() => new(_playerCommodities);

    private void Awake()
    {

    }

    [Inject]
    public void Construct()
    {
        SubscribeToEvent<SceneUnloadEvent>(SaveData);
        SubscribeToEvent<LoadDataEvent>(_ => LoadData());
        LoadData();

        print($"Loaded playerCommodities with count: {_playerCommodities.Count}");
    }

    public void LoadData()
    {
        // var commodities = _saveService.currentSaveData.PlayerCommodities;

        // foreach (var item in commodities)
        // {
        // _playerCommodities.Add(new(ResourceService.GetCommodity(item.id), item.amount));
        // }
    }

    public void SaveData(SceneUnloadEvent @event)
    {
        if (@event.SceneName == "DayScene" || @event.SceneName == "PC_TEST")
        {
            print($"Saved _playerCommodities: {_playerCommodities.Count}");

            _saveService.currentSaveData.PlayerCommodities.Clear();

            foreach (var item in _playerCommodities)
            {
                print(item.commodity.id);
                print(item.amount);
                _saveService.currentSaveData.PlayerCommodities.Add(new(item.commodity.id, item.amount));

            }
            print($"Saved PlayerCommodities: {_saveService.currentSaveData.PlayerCommodities.Count}");
        }


    }

    public void AddCommodity(Commodity commodity, int amount = 1)
    {
        print($"Addded commodity: {commodity.commodityName}, with quantity: {amount}.");
        if (GetCommodityEntry(commodity) == null)
        {
            AddCommodityEntry(commodity);
        }

        GetCommodityEntry(commodity).amount += amount;
    }

    public bool TryReduceCommodity(Commodity commodity, int amount = 1)
    {
        if (GetCommodityEntry(commodity) == null)
        {
            _mediator.GlobalEventBus.Publish<DebugLogErrorEvent>(new($"Commodity: {commodity} not found while was trying to remove it from player"));
            return false;
        }

        if (GetCommodityEntry(commodity).amount < amount)
        {
            return false;
        }

        GetCommodityEntry(commodity).amount -= amount;
        if (GetCommodityEntry(commodity).amount == 0)
        {
            RemoveCommodityEntry(commodity);
        }
        return true;


    }

    private void AddCommodityEntry(Commodity newCommodity)
    {
        print($"Addded new commodity entry: {newCommodity.commodityName}.");
        _playerCommodities.Add(new(newCommodity, 0));
    }

    private void RemoveCommodityEntry(Commodity commodity)
    {
        _playerCommodities.Remove(GetCommodityEntry(commodity));
    }

    private CommodityEntry GetCommodityEntry(Commodity commodity)
    {
        return _playerCommodities.FirstOrDefault((r) => r.commodity == commodity);
    }

    public override void Dispose()
    {
    }
}
