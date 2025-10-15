using System.Collections.Generic;
using UnityEngine;

public class TeaMixer : MonoBehaviour
{
    public static TeaBaseMixed MixTea(TeaBase teaBase, List<WordOfPower> wordsOfPower)
    {
        List<TeaFlavorTag> teaFlavorTags = teaBase.baseFlavorTags;
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

        return new(teaBase, teaFlavorTags);
    }


}

public struct TeaBaseMixed
{
    public TeaBase teaBase;
    public List<TeaFlavorTag> additionalTeaFlavorTags;

    public TeaBaseMixed(TeaBase teaBase, List<TeaFlavorTag> additionalTeaFlavorTags)
    {
        this.teaBase = teaBase;
        this.additionalTeaFlavorTags = additionalTeaFlavorTags;
    }
}
