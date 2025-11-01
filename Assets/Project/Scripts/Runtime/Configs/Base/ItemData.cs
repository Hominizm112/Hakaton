using System;
using System.ComponentModel;
using UniRx;
using UnityEngine;

[System.Serializable]
public class ItemData : ISerializationCallbackReceiver
{

    [SerializeField] private string _name;
    [SerializeField, ReadOnly(true)] private string _id;

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
}

public enum ItemRarity
{
    Common,
    Rare,
    Epic,
    Legendary
}