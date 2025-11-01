using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace GameCore.Utils
{
    [Serializable]
    public class SerializedType<TFilter> where TFilter : class
    {
        [SerializeField] private string type;

        public Type Type => Type.GetType(type);

        [UsedImplicitly]
        private IEnumerable<string> GetTypes() => TypeExtensions.FilterTypes<TFilter>();
    }
}