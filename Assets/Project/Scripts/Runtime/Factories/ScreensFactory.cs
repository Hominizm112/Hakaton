using GameCore.Configs;
using GameCore.Services;
using GameCore.UI;
using System.Linq;
using GameCore.Utils;
using UnityEngine;
using Zenject;
using Cysharp.Threading.Tasks;

namespace GameCore.Factories
{
    public class ScreensFactory : Factory
    {
        [Inject] private ViewsFactory _viewsFactory;
        [Inject] private ScreensConfig _screensConfig;

        private GameObject _rootUI;

        [Inject]
        public void Construct()
        {
            _rootUI = Object.Instantiate(_screensConfig.rootCanvas.gameObject);
        }

        public async UniTask<TView> CreateAsync<TView>() where TView : View
        {
            var data = _screensConfig.Screens.
                FirstOrDefault(d => d.Type == typeof(TView));
            var handle = await data.Asset.LoadAssetAsync<GameObject>();
            var prefab = handle.GetComponent<TView>();
            var screen = _viewsFactory.Create(prefab, _rootUI.transform);
            screen.gameObject.SetActive(false);
            return screen;
        }

        public TView CreateSync<TView>() where TView : View
        {
            var data = _screensConfig.Screens.
                FirstOrDefault(d => d.Type == typeof(TView));
            var handle = data.Asset.LoadAssetAsync<GameObject>();
            var obj = handle.WaitForCompletion();
            var prefab = obj.GetComponent<TView>();
            var screen = _viewsFactory.Create(prefab, _rootUI.transform);
            screen.gameObject.SetActive(false);
            return screen;
        }


    }
}