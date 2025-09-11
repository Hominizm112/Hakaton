using System.Collections.Generic;
using UnityEngine;



[System.Serializable]
public class TeaAromaProfile
{
    [Range(0, 10)] public int strength;
    [Range(0, 10)] public int complexity;
    public List<TeaFlavorTag> primaryAromas = new();
    public List<TeaFlavorTag> secondaryAromas = new();
}

[CreateAssetMenu(fileName = "New Tea", menuName = "Commodity/Tea")]
public class TeaCommodity : Commodity
{
    [Header("Tea-Specific Properties")]
    public TeaType teaType;
    public TeaGrade teaGrade;
    public ProcessingLevel processingLevel;
    public int harvestYear = 2024;
    public float caffeineContent = 0.03f;

    [Header("Brewing Information")]
    public int idealBrewTemperature = 80;
    public int brewTimeSeconds = 180;
    public string brewingNotes;

    [Header("Appearance")]
    public Color dryLeafColor = Color.green;
    public Color liquorColor = Color.cyan;

    [Header("Aroma Profile")]
    public TeaAromaProfile aromaProfile;

    [Header("Blend Components")]
    public List<TeaBlendComponent> blendComponents;

    public string GetFullDescription()
    {
        return $"{commodityName} - {teaGrade} {teaType} from {region}. " +
               $"Harvested in {harvestYear}. " +
               $"Primary flavors: {string.Join(", ", aromaProfile.primaryAromas)}. " +
               $"Best brewed at {idealBrewTemperature}°C for {brewTimeSeconds} seconds.";
    }

    public float CalculateQualityScore()
    {
        float score = qualityLevel;

        // Grade affects quality
        score *= GetGradeMultiplier();

        // Complexity of aroma adds to quality
        score += aromaProfile.complexity * 0.5f;

        // Recent harvest is generally better
        int yearsAgo = System.DateTime.Now.Year - harvestYear;
        if (teaType == TeaType.PuErh)
        {
            // PuErh improves with age (to a point)
            score += Mathf.Clamp(yearsAgo * 0.2f, 0, 5);
        }
        else
        {
            // Other teas are best fresh
            score -= Mathf.Clamp(yearsAgo * 0.3f, 0, 3);
        }

        return Mathf.Clamp(score, 1, 10);
    }

    private float GetGradeMultiplier()
    {
        switch (teaGrade)
        {
            case TeaGrade.Superfine: return 1.5f;
            case TeaGrade.Fine: return 1.2f;
            case TeaGrade.Good: return 1.0f;
            case TeaGrade.Medium: return 0.8f;
            case TeaGrade.Low: return 0.6f;
            default: return 1.0f;
        }
    }
}

public enum TeaType
{
    Green,
    Black,
    Oolong,
    White,
    PuErh,
    Yellow,
    Herbal,
    Rooibos,
    Matcha,
    Chai
}

public enum ProcessingLevel
{
    Raw,
    Light,
    Medium,
    Heavy,
    Aged
}

[System.Serializable]
public class TeaBlendComponent
{
    public TeaCommodity teaComponent;
    [Range(0.1f, 0.9f)] public float proportion;
}

public enum TeaGrade
{
    Superfine,
    Fine,
    Good,
    Medium,
    Low
}

public enum TeaFlavorTag
{
    // Базовые
    Floral,
    Fruity,
    Earthy,
    Grassy,
    Sweet,
    Bitter,
    Smoky,
    Spicy,
    Herbal,
    Woody,

    // Специфичные

    Astringent,
    Malty,
    Creamy,
    Nutty,
    Citrus,
    Berry,
    StoneFruit,
    Tropical,
    Mineral,
    Honey,

    // Дополнительные
    Umami,
    Buttery,
    Vanilla,
    Caramel,
    Chocolate,
    Malt,
    Toast,
    Nutmeg,
    Cinnamon,
    Ginger,

    // Редкие и уникальные
    Orchid,
    Jasmine,
    Rose,
    Lavender,
    Mint,
    Lemon,
    Orange,
    Peach,
    Apricot,
    Melon,
    Mushroom,
    ForestFloor,
    Leather,
    Tobacco,
    Wine,
    Unique
}