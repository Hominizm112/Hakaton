using System.Collections.Generic;
using GameCore.Handlers;
using GameCore.Utils;
using UnityEngine;

namespace GameCore.Configs
{
    [CreateAssetMenu(fileName = "HandlersConfig", menuName = "Configs/HandlersConfig")]
    public class HandlersConfig : Config
    {
        [SerializeField] private List<AddressablePrefabByType<IHandlerable>> _handlers;

        public IReadOnlyList<AddressablePrefabByType<IHandlerable>> Handlers => _handlers;
    }
}