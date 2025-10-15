using System;
using System.Collections.Generic;
using UnityEngine;

public class TeaFactory : MonoBehaviour
{
    [System.Serializable]
    public class TeaGenerationRules
    {
        public List<TeaTypeRegion> regionPreferences;
        public List<SeasonalInfluence> seasonalInfluences;
        public float mutationChance = 0.15f;
        public int maxFlavorTags = 5;
    }

    [System.Serializable]
    public class TeaTypeRegion
    {
        public TeaType teaType;
        public List<RegionOfOrigin> preferredRegions;
    }

    [System.Serializable]
    public class SeasonalInfluence
    {
        public Seasonality season;
        public List<TeaFlavorTag> favoredFlavors;
        public float priceMultiplier = 1.2f;
    }

    [Header("Generation Settings")]
    public TeaGenerationRules generationRules;
    public List<TeaCommodity> baseTeaTemplates;

    [Header("Current Season")]
    public Seasonality currentSeason = Seasonality.Spring;

    private Dictionary<TeaType, TeaCommodity> teaTypeTemplates = new Dictionary<TeaType, TeaCommodity>();

    void Awake()
    {
        foreach (var template in baseTeaTemplates)
        {
            if (!teaTypeTemplates.ContainsKey(template.teaType))
            {
                teaTypeTemplates[template.teaType] = template;
            }
        }
    }

    public TeaCommodity GenerateTeaFromBase(TeaType teaType, string customName = "")
    {
        if (!teaTypeTemplates.ContainsKey(teaType))
        {
            Debug.LogError($"No template found for tea type: {teaType}");
            return null;
        }

        TeaCommodity baseTea = teaTypeTemplates[teaType];
        TeaCommodity newTea = Instantiate(baseTea);

        ApplyRegionalVariation(newTea);

        ApplySeasonalInfluence(newTea);

        RandomizeTeaProperties(newTea);

        newTea.commodityName = string.IsNullOrEmpty(customName) ? GenerateTeaName(newTea) : customName;

        newTea.name = $"{newTea.commodityName}_{System.Guid.NewGuid().ToString().Substring(0, 8)}";

        return newTea;
    }

    public TeaCommodity GenerateRandomTea()
    {
        // Get random tea type
        System.Array values = System.Enum.GetValues(typeof(TeaType));
        TeaType randomType = (TeaType)values.GetValue(UnityEngine.Random.Range(0, values.Length));

        return GenerateTeaFromBase(randomType);
    }

    public TeaCommodity CreateTeaBlend(string blendName, params TeaBlendComponent[] components)
    {
        if (components == null || components.Length == 0)
        {
            Debug.LogError("Blend requires at least one component");
            return null;
        }

        TeaCommodity baseTea = components[0].teaComponent;
        TeaCommodity blend = Instantiate(baseTea);

        blend.commodityName = blendName;
        blend.blendComponents = new List<TeaBlendComponent>(components);
        blend.teaType = TeaType.Herbal;

        CalculateBlendedProperties(blend, components);

        blend.name = $"{blendName}_{System.Guid.NewGuid().ToString().Substring(0, 8)}";

        return blend;
    }

    private void ApplyRegionalVariation(TeaCommodity tea)
    {
        if (tea.region == RegionOfOrigin.Other || tea.region == 0)
        {
            var typeRule = generationRules.regionPreferences.Find(r => r.teaType == tea.teaType);
            if (typeRule != null && typeRule.preferredRegions.Count > 0)
            {
                tea.region = typeRule.preferredRegions[UnityEngine.Random.Range(0, typeRule.preferredRegions.Count)];
            }
            else
            {
                System.Array regions = System.Enum.GetValues(typeof(RegionOfOrigin));
                tea.region = (RegionOfOrigin)regions.GetValue(UnityEngine.Random.Range(0, regions.Length));
            }
        }

        ApplyRegionalFlavors(tea);
    }

    private void ApplyRegionalFlavors(TeaCommodity tea)
    {
        switch (tea.region)
        {
            case RegionOfOrigin.China:
                TryAddFlavorTag(tea, TeaFlavorTag.Earthy);
                TryAddFlavorTag(tea, TeaFlavorTag.Smoky);
                break;
            case RegionOfOrigin.India:
                TryAddFlavorTag(tea, TeaFlavorTag.Malty);
                TryAddFlavorTag(tea, TeaFlavorTag.Spicy);
                break;
            case RegionOfOrigin.Japan:
                TryAddFlavorTag(tea, TeaFlavorTag.Grassy);
                TryAddFlavorTag(tea, TeaFlavorTag.Umami);
                break;
            case RegionOfOrigin.Taiwan:
                TryAddFlavorTag(tea, TeaFlavorTag.Floral);
                TryAddFlavorTag(tea, TeaFlavorTag.Creamy);
                break;
        }
    }

    private void ApplySeasonalInfluence(TeaCommodity tea)
    {
        var seasonInfluence = generationRules.seasonalInfluences.Find(s => s.season == currentSeason);
        if (seasonInfluence != null)
        {
            foreach (var flavor in seasonInfluence.favoredFlavors)
            {
                if (UnityEngine.Random.value > 0.7f)
                {
                    TryAddFlavorTag(tea, flavor);
                }
            }

            tea.basePrice = Mathf.RoundToInt(tea.basePrice * seasonInfluence.priceMultiplier);
        }
    }

    private void RandomizeTeaProperties(TeaCommodity tea)
    {
        tea.qualityLevel = Mathf.RoundToInt(GaussianRandom(5, 2, 1, 10));

        tea.teaGrade = DetermineGradeFromQuality(tea.qualityLevel);

        tea.harvestYear = UnityEngine.Random.Range(System.DateTime.Now.Year - 5, System.DateTime.Now.Year + 1);

        if (tea.aromaProfile == null)
        {
            tea.aromaProfile = new TeaAromaProfile();
        }

        tea.aromaProfile.strength = UnityEngine.Random.Range(5, 10);
        tea.aromaProfile.complexity = Mathf.Clamp(tea.qualityLevel - 2, 1, 8);

        EnsureMinimumFlavorTags(tea, 2);

        if (UnityEngine.Random.value < generationRules.mutationChance)
        {
            ApplyFlavorMutation(tea);
        }
    }

    private void EnsureMinimumFlavorTags(TeaCommodity tea, int minTags)
    {
        if (tea.aromaProfile.primaryAromas.Count < minTags)
        {
            List<TeaFlavorTag> allFlavors = new List<TeaFlavorTag>(
                (TeaFlavorTag[])System.Enum.GetValues(typeof(TeaFlavorTag)));

            allFlavors.RemoveAll(f => tea.aromaProfile.primaryAromas.Contains(f));

            while (tea.aromaProfile.primaryAromas.Count < minTags && allFlavors.Count > 0)
            {
                int randomIndex = UnityEngine.Random.Range(0, allFlavors.Count);
                tea.aromaProfile.primaryAromas.Add(allFlavors[randomIndex]);
                allFlavors.RemoveAt(randomIndex);
            }
        }
    }

    private void ApplyFlavorMutation(TeaCommodity tea)
    {
        if (UnityEngine.Random.value < 0.3f && tea.aromaProfile.primaryAromas.Count < generationRules.maxFlavorTags)
        {
            TryAddFlavorTag(tea, GetRareFlavorForType(tea.teaType));
        }

        if (UnityEngine.Random.value < 0.2f)
        {
            tea.qualityLevel = Mathf.Min(10, tea.qualityLevel + 1);
            tea.teaGrade = DetermineGradeFromQuality(tea.qualityLevel);
        }
    }

    private TeaFlavorTag GetRareFlavorForType(TeaType type)
    {
        switch (type)
        {
            case TeaType.Green:
                return UnityEngine.Random.value > 0.5f ? TeaFlavorTag.Umami : TeaFlavorTag.Mineral;
            case TeaType.Black:
                return UnityEngine.Random.value > 0.5f ? TeaFlavorTag.Honey : TeaFlavorTag.StoneFruit;
            case TeaType.Oolong:
                return UnityEngine.Random.value > 0.5f ? TeaFlavorTag.Orchid : TeaFlavorTag.Buttery;
            case TeaType.White:
                return UnityEngine.Random.value > 0.5f ? TeaFlavorTag.Melon : TeaFlavorTag.Apricot;
            case TeaType.PuErh:
                return UnityEngine.Random.value > 0.5f ? TeaFlavorTag.ForestFloor : TeaFlavorTag.Mushroom;
            default:
                return TeaFlavorTag.Floral;
        }
    }

    private void TryAddFlavorTag(TeaCommodity tea, TeaFlavorTag tag)
    {
        if (!tea.aromaProfile.primaryAromas.Contains(tag) &&
            tea.aromaProfile.primaryAromas.Count < generationRules.maxFlavorTags)
        {
            tea.aromaProfile.primaryAromas.Add(tag);
        }
    }

    private TeaGrade DetermineGradeFromQuality(int quality)
    {
        if (quality >= 9) return TeaGrade.Superfine;
        if (quality >= 7) return TeaGrade.Fine;
        if (quality >= 5) return TeaGrade.Good;
        if (quality >= 3) return TeaGrade.Medium;
        return TeaGrade.Low;
    }

    private void CalculateBlendedProperties(TeaCommodity blend, TeaBlendComponent[] components)
    {
        blend.basePrice = 0;
        blend.qualityLevel = 0;
        blend.aromaProfile = new TeaAromaProfile();
        blend.flavorTags = new List<TeaFlavorTag>();

        float totalProportion = 0f;
        float totalStrength = 0f;
        float totalComplexity = 0f;

        foreach (var component in components)
        {
            totalProportion += component.proportion;

            blend.basePrice += Mathf.RoundToInt(component.teaComponent.basePrice * component.proportion);
            blend.qualityLevel += Mathf.RoundToInt(component.teaComponent.qualityLevel * component.proportion);

            totalStrength += component.teaComponent.aromaProfile.strength * component.proportion;
            totalComplexity += component.teaComponent.aromaProfile.complexity * component.proportion;

            foreach (var flavor in component.teaComponent.flavorTags)
            {
                if (!blend.flavorTags.Contains(flavor) && UnityEngine.Random.value < component.proportion * 0.7f)
                {
                    blend.flavorTags.Add(flavor);
                }
            }
        }

        if (totalProportion != 1f)
        {
            blend.basePrice = Mathf.RoundToInt(blend.basePrice / totalProportion);
            blend.qualityLevel = Mathf.RoundToInt(blend.qualityLevel / totalProportion);
            totalStrength /= totalProportion;
            totalComplexity /= totalProportion;
        }

        blend.aromaProfile.strength = Mathf.RoundToInt(totalStrength);
        blend.aromaProfile.complexity = Mathf.RoundToInt(totalComplexity);

        blend.aromaProfile.primaryAromas = new List<TeaFlavorTag>();
        for (int i = 0; i < Mathf.Min(3, blend.flavorTags.Count); i++)
        {
            blend.aromaProfile.primaryAromas.Add(blend.flavorTags[i]);
        }
    }

    private string GenerateTeaName(TeaCommodity tea)
    {
        List<string> prefixes = new List<string>
        {
            "Imperial", "Royal", "Golden", "Silver", "Jade", "Dragon", "Phoenix", "Lucky"
        };

        List<string> suffixes = new List<string>
        {
            "Pearl", "Cloud", "Mist", "Dew", "Rain", "Moon", "Sun", "Blossom"
        };

        string regionName = tea.region.ToString();
        if (tea.region == RegionOfOrigin.SriLanka) regionName = "Ceylon";

        string prefix = prefixes[UnityEngine.Random.Range(0, prefixes.Count)];
        string suffix = suffixes[UnityEngine.Random.Range(0, suffixes.Count)];

        return $"{prefix} {regionName} {tea.teaType} {suffix}";
    }

    private float GaussianRandom(float mean, float stdDev, float min, float max)
    {
        float u1 = 1.0f - UnityEngine.Random.value;
        float u2 = 1.0f - UnityEngine.Random.value;
        float randStdNormal = Mathf.Sqrt(-2.0f * Mathf.Log(u1)) * Mathf.Sin(2.0f * Mathf.PI * u2);
        float randNormal = mean + stdDev * randStdNormal;

        return Mathf.Clamp(randNormal, min, max);
    }

    private float CalculateAverage(System.Collections.IEnumerable components, System.Func<TeaBlendComponent, float> selector)
    {
        float sum = 0f;
        int count = 0;

        foreach (TeaBlendComponent component in components)
        {
            sum += selector(component);
            count++;
        }

        return count > 0 ? sum / count : 0f;
    }
}
