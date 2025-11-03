

using System;
using System.Collections.Generic;

namespace GameCore.Configs
{
    [Serializable]
    public class PlacerConfig
    {
        public List<PlacerBehaviour> placerBehaviours;

        public PlacerBehaviourType GetAction(PlacerAction action, ItemData itemData = null)
        {
            var item = placerBehaviours.Find(r => r.placerAction == action);
            PlacerBehaviour itemWithTag = null;
            if (itemData != null)
            {
                itemWithTag = placerBehaviours.Find(r => r.placerAction == action && r.itemTag == itemData.itemTag.Value);
            }

            if (itemWithTag != null)
            {
                return itemWithTag.placeBehaviourType;
            }

            if (item != null)
            {
                return item.placeBehaviourType;
            }

            return PlacerBehaviourType.Hide;
        }
    }

    [Serializable]
    public class PlacerBehaviour
    {
        public PlacerBehaviourType placeBehaviourType;
        public PlacerAction placerAction;
        public ItemTag itemTag;
    }

    public enum PlacerBehaviourType
    {
        Hide,
        Return,
        Place
    }

    public enum PlacerAction
    {
        Take,
        PlaceInArea,
        PlaceInEmpty
    }
}