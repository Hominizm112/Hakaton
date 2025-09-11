using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "new NPC", menuName = "NPC/NPC")]
public class NPC : ScriptableObject
{
    public string npcName;
    public GameObject npcPrefab;
    public TypedListContainer smallTalkLines;
    public string SmallTalkLine => smallTalkLines.GetRandomItem() as string;
    public TypedListContainer requestLines;
    public string RequestLine => requestLines.GetRandomItem() as string;
    public List<BuyReaction> buyReactions;
    public RangeUtils.Bounds<int> friendLevelCap;
    public List<FriendLevel> friendLevelPoints = new();
    public TeaComposite favoriteTea;
    public TeaComposite normalTea;
    public TeaComposite unlovedTea;


    public void BuyTea(TeaCommodity tea, Action<NPCBuyResult> OnComplete)
    {
        NPCBuyResult npcBuyResult = new();

        if (tea == null || buyReactions == null || buyReactions.Count == 0)
        {
            return;
        }

        TeaRating teaRating = EvaluateTea(tea);
        Debug.Log($"Evaluated tea| rating: {teaRating.rating}, buy satisfaction: {teaRating.buySatisfaction}");

        NPCBuySatisfaction satisfaction = GetNPCBuySatisfaction(teaRating);
        npcBuyResult.satisfaction = satisfaction;
        Debug.Log($"final satisfaction: {satisfaction}");

        var buyReaction = GetBuyReaction(satisfaction);

        if (buyReaction.HasValue && buyReaction.Value.IsValid())
        {
            npcBuyResult.friendPoints = buyReaction.Value.friendPointsAdded;

            if (buyReaction.Value.dialogueLine.GetRandomItem() is string dialogue)
            {
                npcBuyResult.dialogueLine = dialogue;
            }
        }

        OnComplete?.Invoke(npcBuyResult);


    }

    private NPCBuySatisfaction GetNPCBuySatisfaction(TeaRating teaRating)
    {
        bool isPerfectMatch = teaRating.rating >= 95;

        if (teaRating.buySatisfaction == NPCBuySatisfaction.Satisfied)
        {
            return isPerfectMatch ? NPCBuySatisfaction.VerySatisfied : NPCBuySatisfaction.Satisfied;
        }
        if (teaRating.buySatisfaction == NPCBuySatisfaction.Dissatisfied)
        {
            return isPerfectMatch ? NPCBuySatisfaction.VeryDissatisfied : NPCBuySatisfaction.Dissatisfied;
        }
        return NPCBuySatisfaction.Neutral;
    }

    private TeaRating EvaluateTea(TeaCommodity tea)
    {
        int favoriteScore = favoriteTea.GetRating(tea);
        int normalScore = normalTea.GetRating(tea);
        int unlovedScore = unlovedTea.GetRating(tea);

        int result = Mathf.Abs(favoriteScore - unlovedScore) < 50 ? normalScore : Mathf.Max(favoriteScore, unlovedScore);

        if (favoriteScore == result) return new TeaRating(favoriteScore, NPCBuySatisfaction.Satisfied);
        if (normalScore == result) return new TeaRating(normalScore, NPCBuySatisfaction.Neutral);
        return new TeaRating(unlovedScore, NPCBuySatisfaction.Dissatisfied);
    }

    private BuyReaction? GetBuyReaction(NPCBuySatisfaction nPCBuySatisfaction)
    {
        return buyReactions?.FirstOrDefault(r => r.buySatisfaction == nPCBuySatisfaction);
    }

    private struct TeaRating
    {
        public int rating;
        public NPCBuySatisfaction buySatisfaction;

        public TeaRating(int rating, NPCBuySatisfaction buySatisfaction)
        {
            this.rating = rating;
            this.buySatisfaction = buySatisfaction;
        }
    }


}

public struct NPCBuyResult
{
    public int friendPoints;
    public string dialogueLine;
    public NPCBuySatisfaction satisfaction;

}

[Serializable]
public struct TeaComposite
{
    public List<TeaType> teaTypes;
    public List<TeaFlavorTag> teaFlavorTags;
    public List<ProcessingLevel> processingLevels;
    public List<TeaGrade> teaGrades;

    [Range(0, 100)]
    public int teaRatingThreshold;

    public int GetRating(TeaCommodity tea)
    {
        float rating = 0f;

        // 20% weight
        if (teaTypes.Contains(tea.teaType))
            rating += 20f;

        // 40% weight
        if (teaFlavorTags.Count > 0)
        {
            int flavorMatches = teaFlavorTags.Count(flavor => tea.flavorTags.Contains(flavor));
            float flavorMatchPercent = (float)flavorMatches / teaFlavorTags.Count;
            rating += flavorMatchPercent * 40f;
        }
        else
        {
            rating += 40f;
        }

        // 15% weight
        if (processingLevels.Contains(tea.processingLevel))
            rating += 15f;

        // 25% weight
        if (teaGrades.Contains(tea.teaGrade))
            rating += 25f;

        Debug.Log($"calculated rating: {rating}");

        return rating > teaRatingThreshold ? Mathf.RoundToInt(rating) : 0;


    }
}

[Serializable]
public struct BuyReaction
{
    public NPCBuySatisfaction buySatisfaction;
    public float costMultiplier;
    public int friendPointsAdded;
    public TypedListContainer dialogueLine;

    public bool IsValid()
    {
        return dialogueLine != null;
    }
}

[Serializable]
public struct FriendLevel
{
    public int level;
    public int pointsToReach;
}


public enum NPCBuySatisfaction
{
    VeryDissatisfied,
    Dissatisfied,
    Neutral,
    Satisfied,
    VerySatisfied
}


