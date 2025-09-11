using System.Collections.Generic;
using UnityEngine;



[CreateAssetMenu(fileName = "New Commodity", menuName = "Commodity/Base Commodity")]
public class Commodity : ScriptableObject
{
    [Header("Basic Information")]
    public string commodityName;
    public string description;
    public Sprite icon;

    [Header("Pricing")]
    [Range(1, 1000)]
    public int basePrice = 10;
    public PriceTier priceTier = PriceTier.Low;

    [Header("FlavorTags")]
    public List<TeaFlavorTag> flavorTags = new List<TeaFlavorTag>();

    [Header("Rarity & Quality")]
    [Range(1, 10)]
    public int qualityLevel = 5;
    public Rarity rarity = Rarity.Common;

    [Header("Additional Properties")]
    public Seasonality seasonality = Seasonality.YearRound;
    public RegionOfOrigin region;

    public int CurrentPrice => CalculateCurrentPrice();

    private int CalculateCurrentPrice()
    {
        float priceMultiplier = 1f;

        priceMultiplier *= GetRarityMultiplier();

        priceMultiplier *= 1f + (qualityLevel - 5) * 0.01f;

        if (seasonality != Seasonality.YearRound)
        {
            priceMultiplier *= GetSeasonalMultiplier();
        }

        return Mathf.RoundToInt(basePrice * priceMultiplier);
    }

    private float GetRarityMultiplier()
    {
        switch (rarity)
        {
            case Rarity.Common: return 1f;
            case Rarity.Uncommon: return 1.5f;
            case Rarity.Rare: return 2.5f;
            case Rarity.Epic: return 4f;
            case Rarity.Legendary: return 6f;
            default: return 1f;
        }
    }

    private float GetSeasonalMultiplier()
    {
        return 1f;
    }

    public bool HasFlavorTag(TeaFlavorTag tag)
    {
        return flavorTags.Contains(tag);
    }

    public bool HasAnyFlavorTag(params TeaFlavorTag[] tags)
    {
        foreach (var tag in tags)
        {
            if (flavorTags.Contains(tag))
            {
                return true;
            }
        }
        return false;
    }

    public bool HasAllFlavorTags(params TeaFlavorTag[] tags)
    {
        foreach (var tag in tags)
        {
            if (!flavorTags.Contains(tag))
            {
                return false;
            }
        }
        return true;
    }



}

public enum Rarity
{
    Common,
    Uncommon,
    Rare,
    Epic,
    Legendary
}

public enum PriceTier
{
    Low,
    Medium,
    High,
    Premium,
    Luxury
}

public enum Seasonality
{
    YearRound,
    Spring,
    Summer,
    Autumn,
    Winter
}

public enum RegionOfOrigin
{
    China,
    India,
    Japan,
    SriLanka,
    Taiwan,
    Kenya,
    Nepal,
    Vietnam,
    Other
}