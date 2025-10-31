using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TeaMixerService
{
    public List<TeaFlavorTag> MixTea(TeaBase teaBase, List<WordOfPower> wordsOfPower)
    {
        List<TeaFlavorTag> teaFlavorTags = new(teaBase.baseFlavorTags);
        List<TeaFlavorTag> teaFlavorsToRemove = new();
        foreach (var wordOfPower in wordsOfPower)
        {
            foreach (var flavorInfluence in wordOfPower.wordToFlavorInfluences)
            {
                TeaFlavorTag teaFlavorTag = flavorInfluence.teaFlavorTag;
                switch (flavorInfluence.wordInfuence)
                {
                    case WordInfuence.Add:
                        if (!teaFlavorTags.Contains(teaFlavorTag))
                        {
                            teaFlavorTags.Add(teaFlavorTag);
                        }
                        break;
                    case WordInfuence.Remove:
                        if (teaFlavorTags.Contains(teaFlavorTag))
                        {
                            teaFlavorsToRemove.Add(teaFlavorTag);
                            teaFlavorTags.Remove(teaFlavorTag);
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        for (int i = teaFlavorsToRemove.Count - 1; i >= 0; i--)
        {
            if (teaFlavorTags.Contains(teaFlavorsToRemove[i]))
                teaFlavorTags.Remove(teaFlavorsToRemove[i]);
        }

        return teaFlavorTags;
    }

    public float GetRating(List<TeaFlavorTag> flavors, List<TeaFlavorTag> desiredFlavors)
    {
        float rating = 0f;

        if (flavors.Count > 0)
        {
            int flavorMatches = flavors.Count(flavor => desiredFlavors.Contains(flavor));
            rating += (float)flavorMatches / (float)flavors.Count;
        }

        Debug.Log($"calculated rating: {rating}");

        return rating;
    }


}
