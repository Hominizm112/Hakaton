using UniRx;
using UnityEngine;

[System.Serializable]
public class ItemModel
{
    public string Id;
    public string InstanceId;
    public string Name;
    public ReactiveProperty<int> Quantity;
    public ItemRarity Rarity;
    public int SellPrice;

    [System.NonSerialized] public ItemTemplate _cachedConfig;

    public ItemModel(ItemTemplate config, int quantity = 1)
    {
        InstanceId = System.Guid.NewGuid().ToString();
        Id = config.ItemId;
        Quantity = new ReactiveProperty<int>(quantity);
        Rarity = config.Rarity;
        SellPrice = config.BaseSellPrice;
        _cachedConfig = config;
    }


}

public enum ItemRarity
{
    Common,
    Rare,
    Epic,
    Legendary
}