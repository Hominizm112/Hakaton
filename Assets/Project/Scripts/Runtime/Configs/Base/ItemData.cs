using System;
using TriInspector;
using UniRx;
using UnityEngine;

[System.Serializable]
public class ItemData : ISerializationCallbackReceiver
{

    [SerializeField] private string _name;
    [SerializeField] private string _id;

    public string Name
    {
        get => _name;
        set
        {
            _name = value;
            _id = _name.ToLower().Replace(" ", "_");
        }
    }
    public string Id => _id;

    [NonSerialized] public ReactiveProperty<int> Quantity = new();
    public ItemRarity Rarity;
    public int SellPrice;

    [ShowInInspector] public ReactiveProperty<ItemTag> itemTag = new(ItemTag.Item);
    [ShowInInspector, SerializeReference] public ItemConfig itemConfig;

    public void OnBeforeSerialize()
    {
        if (!string.IsNullOrEmpty(_name))
        {
            _id = _name.ToLower().Replace(" ", "_");
        }
    }

    public void OnAfterDeserialize()
    {
    }

    public T GetConfig<T>() where T : ItemConfig
    {
        return itemConfig as T;
    }

    public bool IsConfig<T>() where T : ItemConfig
    {
        return itemConfig is T;
    }
}

public enum ItemRarity
{
    Common,
    Rare,
    Epic,
    Legendary
}

public enum ItemTag
{
    Any,
    Item,
    TeaBase,
    TeaReady
}