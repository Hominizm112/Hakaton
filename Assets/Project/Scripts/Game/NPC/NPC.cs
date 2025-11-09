using System;
using System.Collections.Generic;
using UniRx;
using Unity.VisualScripting;
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
    public RangeUtils.Bounds<int> friendLevelCap;
    private int _friendPoints;
    public List<FriendLevel> friendLevelPoints = new();
    public List<TeaFlavorTag> favoriteFlavors;
    public List<TeaFlavorTag> normalFlavors;
    public List<TeaFlavorTag> unlovedFlavors;

    private ReactiveProperty<FriendLevel> _currentFriendLevel;
    private IReadOnlyReactiveProperty<FriendLevel> CurrentFriendLevel => _currentFriendLevel;


    public void BuyItem(ItemData itemData)
    {
        if (itemData.IsConfig<TeaConfig>())
        {
            BuyTea(itemData);
        }

    }

    public FriendLevel GetFriendLevel()
    {
        return friendLevelPoints.Find(r => r.pointsToReach.InRange(_friendPoints));
    }


    private void BuyTea(ItemData itemData)
    {
        var flavors = itemData.GetConfig<TeaConfig>().teaFlavorTags;
        var buySatisfaction = EvaluateFlavors(flavors);

        switch (buySatisfaction)
        {
            case NPCBuySatisfaction.Best:
                _friendPoints += 100;
                break;
            case NPCBuySatisfaction.Normal:
                _friendPoints += 25;
                break;
            case NPCBuySatisfaction.Poor:
                _friendPoints -= 50;
                break;
        }

        _friendPoints = Mathf.Max(0, _friendPoints);

        FriendLevel friendLevel = GetFriendLevel();

        if (friendLevel != _currentFriendLevel.Value)
        {
            _currentFriendLevel.Value = friendLevel;
        }
    }


    private NPCBuySatisfaction EvaluateFlavors(List<TeaFlavorTag> flavors)
    {
        int favoriteMatches = 0;
        int normalMatches = 0;
        int unlovedMatches = 0;


        foreach (var flavor in flavors)
        {
            if (favoriteFlavors.Contains(flavor))
            {
                favoriteMatches++;
            }

            if (normalFlavors.Contains(flavor))
            {
                normalMatches++;
            }

            if (unlovedFlavors.Contains(flavor))
            {
                unlovedMatches++;
            }
        }

        int flavorsCount = flavors.Count;

        float favoriteRating = favoriteMatches / flavorsCount;
        float normalRating = normalMatches / flavorsCount;
        float unlovedRating = unlovedMatches / flavorsCount;

        float maxRating = Mathf.Max(favoriteRating, normalRating, unlovedRating);

        RangeFloat rangeFavorite = new(favoriteRating - 0.1f, favoriteRating + 0.1f);

        if (rangeFavorite.InRange(unlovedRating))
        {
            return NPCBuySatisfaction.Normal;
        }

        if (maxRating == favoriteRating)
        {
            return NPCBuySatisfaction.Best;
        }
        else if (maxRating == normalRating)
        {
            return NPCBuySatisfaction.Normal;
        }
        else if (maxRating == unlovedRating)
        {
            return NPCBuySatisfaction.Poor;
        }

        return NPCBuySatisfaction.Poor;


    }


}


[Serializable]
public class FriendLevel
{
    public int level;
    public RangeInt pointsToReach;
}

public enum NPCBuySatisfaction
{
    Best,
    Normal,
    Poor
}
