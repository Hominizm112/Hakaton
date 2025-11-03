using GameCore.Configs;
using GameCore.UI;
using UnityEngine;
using Zenject;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;

namespace GameCore.Factories
{
    public class ScreensFactory : Factory
    {
        [Inject] private ViewsFactory _viewsFactory;
        [Inject] private ScreensConfig _screensConfig;

        private GameObject _rootUI;
        private Dictionary<System.Type, AssetReferenceGameObject> _screenCache;
        private Dictionary<System.Type, UniTask<GameObject>> _loadingTasks;

        [Inject]
        public void Construct()
        {
            _rootUI = Object.Instantiate(_screensConfig.rootCanvas.gameObject);
            _loadingTasks = new();
            BuildScreenCache();
        }

        private void BuildScreenCache()
        {
            _screenCache = new();
            foreach (var screenData in _screensConfig.Screens)
            {
                if (screenData.Type != null && !_screenCache.ContainsKey(screenData.Type))
                {
                    _screenCache[screenData.Type] = screenData.Asset;

                    if (screenData.InitializeOnStart)
                    {
                        CreateAsync(screenData.Type).Forget();
                    }
                }
            }
        }

        public async UniTask<View> CreateAsync(System.Type viewType)
        {
            if (!_screenCache.TryGetValue(viewType, out var data))
            {
                throw new System.Exception($"No screen data found for type: {viewType}");
            }

            if (!_loadingTasks.TryGetValue(viewType, out var loadingTask))
            {
                loadingTask = LoadPrefabAsync(data);
                _loadingTasks[viewType] = loadingTask;
            }

            GameObject prefabObj = await loadingTask;
            _loadingTasks.Remove(viewType);

            return CreateScreenFromPrefab(prefabObj, viewType);
        }

        private View CreateScreenFromPrefab(GameObject prefabObj, System.Type viewType)
        {
            var prefab = prefabObj.GetComponent(viewType) as View;
            var screen = _viewsFactory.Create(prefab, _rootUI.transform);
            screen.transform.localScale = Vector3.one;
            return screen;
        }


        private async UniTask<GameObject> LoadPrefabAsync(AssetReferenceGameObject data)
        {
            var handle = await data.LoadAssetAsync<GameObject>();
            return handle;
        }


    }
}