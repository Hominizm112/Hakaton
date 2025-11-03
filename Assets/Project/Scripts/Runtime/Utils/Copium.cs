

using UnityEngine;

namespace GameCore.Runtime.Utils
{
    public static class Copium
    {
        public static T CreateDeepCopy<T>(T source) where T : class
        {
            return source != null ?
                JsonUtility.FromJson(JsonUtility.ToJson(source), source.GetType()) as T : null;
        }
    }
}