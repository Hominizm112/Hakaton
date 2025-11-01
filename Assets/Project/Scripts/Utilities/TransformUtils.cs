using System.Collections.Generic;
using UnityEngine;

public class TransformUtils : MonoBehaviour
{
    public static List<T> SearchForComponents<T>(Transform source)
    {
        var results = new List<T>();
        if (source == null) return results;

        SearchRecursively(source, results);
        return results;

    }

    private static void SearchRecursively<T>(Transform current, List<T> results)
    {
        var components = current.GetComponents<T>();
        if (components.Length > 0)
        {
            results.AddRange(components);
        }

        foreach (Transform child in current)
        {
            SearchRecursively(child, results);
        }
    }


}
