using System.Collections.Generic;
using UnityEngine;

public static class RandomUtils
{
    public static T GetRandom<T>(List<T> list)
    {
        if (list == null || list.Count == 0)
        {
            Debug.LogWarning("List is null or empty");
            return default(T);
        }

        int randomIndex = Random.Range(0, list.Count);
        return list[randomIndex];
    }
}
