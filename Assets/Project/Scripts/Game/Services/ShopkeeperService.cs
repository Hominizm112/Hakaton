using System.Collections.Generic;
using UnityEngine;

public class ShopkeeperService : MonoService
{
    private Dictionary<Commodity, int> _playerCommodities = new();
    private Mediator _mediator;

    public override void Initialize(Mediator mediator)
    {
        _mediator = mediator;
    }

    public void AddCommodity(Commodity commodity, int amount = 1)
    {
        print($"Addded commodity: {commodity.commodityName}, with quantity: {amount}.");
        if (!_playerCommodities.ContainsKey(commodity))
        {
            AddCommodityEntry(commodity);
        }

        _playerCommodities[commodity] += amount;
    }

    public bool TryReduceCommodity(Commodity commodity, int amount = 1)
    {
        if (!_playerCommodities.ContainsKey(commodity))
        {
            _mediator.GlobalEventBus.Publish<DebugLogErrorEvent>(new($"Commodity: {commodity} to found while was trying to remove it from player"));
            return false;
        }

        if (_playerCommodities[commodity] < amount)
        {
            return false;
        }

        _playerCommodities[commodity] -= amount;
        if (_playerCommodities[commodity] == 0)
        {
            RemoveCommodityEntry(commodity);
        }
        return true;


    }

    private void AddCommodityEntry(Commodity newCommodity)
    {
        print($"Addded new commodity entry: {newCommodity.commodityName}.");
        _playerCommodities.Add(newCommodity, 0);
    }

    private void RemoveCommodityEntry(Commodity commodity)
    {
        _playerCommodities.Remove(commodity);
    }
}
