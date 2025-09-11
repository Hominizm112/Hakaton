using System;
using UnityEngine;

public class RangeUtils : MonoBehaviour
{


    [Serializable]
    public class Bounds<T> where T : IComparable<T>
    {
        public T min;
        public T max;
        public bool InBounds(T value, bool minInclusive = true, bool maxInclusive = true)
        {
            bool minCheck = minInclusive ? value.CompareTo(min) >= 0 : value.CompareTo(min) > 0;
            bool maxCheck = maxInclusive ? value.CompareTo(max) <= 0 : value.CompareTo(max) < 0;

            return minCheck && maxCheck;
        }

        public bool InBounds(T value)
        {
            return InBounds(value, false, false);
        }
    }
}
