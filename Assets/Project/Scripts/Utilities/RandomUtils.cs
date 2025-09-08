using System;
using System.Collections.Generic;
using UnityEngine;

public static class RandomUtils
{
    public static T GetRandomItemInList<T>(List<T> list)
    {
        if (list == null || list.Count == 0)
        {
            Debug.LogWarning("List is null or empty");
            return default(T);
        }

        int randomIndex = UnityEngine.Random.Range(0, list.Count);
        return list[randomIndex];
    }

    public static bool CoinFlip()
    {
        return UnityEngine.Random.Range(0, 2) == 0;
    }

    public static int RangeInt(RangeInt range)
    {
        return UnityEngine.Random.Range(range.min, range.max);
    }

    public static char RandomLetter(bool uppercase = false, char last = (char)26)
    {
        string lastStr = last.ToString();
        lastStr.ToLower();
        last = lastStr.ToCharArray()[0];
        return uppercase ? GenerateRandomUppercaseLetter(last) : GenerateRandomLowercaseLetter(last);
    }

    private static char GenerateRandomLowercaseLetter(char last = (char)26)
    {
        last -= 'a';
        return (char)('a' + UnityEngine.Random.Range(0, last));
    }

    private static char GenerateRandomUppercaseLetter(char last = (char)26)
    {
        last -= 'a';
        return (char)('A' + UnityEngine.Random.Range(0, last));
    }


}

public struct RangeInt
{
    public int min;
    public int max;
    public RangeInt(int min, int max)
    {
        this.min = min;
        this.max = max;
    }
}