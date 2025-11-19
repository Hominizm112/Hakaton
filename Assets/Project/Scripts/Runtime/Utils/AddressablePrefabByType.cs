using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using TeaGame.Runtime.Configs;
using TriInspector;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace GameCore.Utils
{
    [Serializable]
    public class AddressablePrefabByType<TFilter> where TFilter : class
    {
        [Group("Type")][HideLabel][SerializeField][Dropdown(nameof(GetTypes))] private string type;

        [Group("Asset")][HideLabel][SerializeField] private AssetReferenceGameObject asset;

        public Type Type => TypeExtensions.GetType(type);
        public AssetReferenceGameObject Asset => asset;
        public bool InitializeOnStart;
        public bool Persistent;
        public SceneConfig Scene;

        [UsedImplicitly]
        private IEnumerable<string> GetTypes() => TypeExtensions.FilterTypes<TFilter>();
    }
}