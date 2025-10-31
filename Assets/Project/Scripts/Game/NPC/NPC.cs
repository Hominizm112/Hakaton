using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem.Interactions;

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
    public List<TeaFlavorTag> favoriteFlavors;
    public List<TeaFlavorTag> normalFlavors;
    public List<TeaFlavorTag> unlovedFlavors;


    public void BuyTea(List<TeaFlavorTag> flavors, Action<NPCBuyResult> OnComplete)
    {
        // NPCBuyResult npcBuyResult = new();

        foreach (var item in flavors)
        {
            Debug.Log(item);
        }

        if (buyReactions == null || buyReactions.Count == 0)
        {
            return;
        }

        // TeaRating teaRating = EvaluateTea(flavors);
        // Debug.Log($"Evaluated tea| rating: {teaRating.rating}, buy satisfaction: {teaRating.buySatisfaction}");

        // NPCBuySatisfaction satisfaction = GetNPCBuySatisfaction(teaRating);
        // npcBuyResult.satisfaction = satisfaction;
        // Debug.Log($"final satisfaction: {satisfaction}");

        // var buyReaction = GetBuyReaction(satisfaction);

        // if (buyReaction.HasValue && buyReaction.Value.IsValid())
        // {
        // npcBuyResult.friendPoints = buyReaction.Value.friendPointsAdded;
        // 
        // if (buyReaction.Value.dialogueLine.GetRandomItem() is string dialogue)
        // {
        // npcBuyResult.dialogueLine = dialogue;
        // }
        // }

        // OnComplete?.Invoke(npcBuyResult);


    }

    private NPCBuySatisfaction GetNPCBuySatisfaction(TeaRating teaRating)
    {
        bool isPerfectMatch = teaRating.rating >= .7f;

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

    // private TeaRating EvaluateTea(List<TeaFlavorTag> flavors)
    // {
    //     // float favoriteScore = TeaMixer.GetRating(flavors, favoriteFlavors);
    //     // float normalScore = TeaMixer.GetRating(flavors, normalFlavors);
    //     // float unlovedScore = TeaMixer.GetRating(flavors, unlovedFlavors);

    //     // float result = Mathf.Abs(favoriteScore - unlovedScore) < .5f ? normalScore : Mathf.Max(favoriteScore, unlovedScore);

    //     // if (favoriteScore == result) return new TeaRating(favoriteScore, NPCBuySatisfaction.Satisfied);
    //     // if (normalScore == result) return new TeaRating(normalScore, NPCBuySatisfaction.Neutral);
    //     // return new TeaRating(unlovedScore, NPCBuySatisfaction.Dissatisfied);
    // }

    private BuyReaction? GetBuyReaction(NPCBuySatisfaction nPCBuySatisfaction)
    {
        return buyReactions?.FirstOrDefault(r => r.buySatisfaction == nPCBuySatisfaction);
    }

    private struct TeaRating
    {
        public float rating;
        public NPCBuySatisfaction buySatisfaction;

        public TeaRating(float rating, NPCBuySatisfaction buySatisfaction)
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


