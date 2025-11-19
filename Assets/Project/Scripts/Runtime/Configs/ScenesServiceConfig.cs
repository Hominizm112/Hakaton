using GameCore.Configs;
using UnityEngine;

namespace TeaGame.Runtime.Configs
{
    [CreateAssetMenu(fileName = "ScenesServiceConfig", menuName = "Services/ScenesServiceConfig")]
    public class ScenesServiceConfig : Config
    {
        public string bootstrapSceneName = "";
    }
}