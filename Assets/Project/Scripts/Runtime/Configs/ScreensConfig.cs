using System.Collections.Generic;
using GameCore.UI;
using GameCore.Utils;
using UnityEngine;

namespace GameCore.Configs
{
    [CreateAssetMenu(fileName = "ScreensConfig", menuName = "Configs/ScreensConfig")]
    public class ScreensConfig : Config
    {
        [SerializeField] private List<AddressablePrefabByType<View>> _screens;

        public IReadOnlyList<AddressablePrefabByType<View>> Screens => _screens;
        public Canvas rootCanvas;
    }
}