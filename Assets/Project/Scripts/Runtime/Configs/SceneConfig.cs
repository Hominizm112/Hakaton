using GameCore.Configs;
using UnityEngine;

namespace TeaGame.Runtime.Configs
{
    [CreateAssetMenu(fileName = "SceneConfig", menuName = "Scenes/SceneConfig")]
    public class SceneConfig : Config
    {
        public string SceneName;
    }
}