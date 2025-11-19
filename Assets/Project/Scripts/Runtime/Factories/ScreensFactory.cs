using GameCore.Configs;
using GameCore.UI;
using UnityEngine;
using Zenject;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;
using UniRx;
using System.Threading;
using System;

namespace GameCore.Factories
{
    public class ScreensFactory : Factory, IDisposable
    {
        [Inject] private ViewsFactory _viewsFactory;
        [Inject] private ScreensConfig _screensConfig;
        [Inject] private EventBus _eventBus;

        private GameObject _rootUI;
        private GameObject _rootUIPersistent;
        private Dictionary<System.Type, ScreenCache> _screenCache;
        private Dictionary<string, List<System.Type>> _sceneScreenPairs;
        private Dictionary<System.Type, (UniTask<GameObject> task, CancellationTokenSource cts)> _loadingTasks;
        private readonly object _loadingTasksLock = new object();
        private CompositeDisposable disposables = new();
        private string _currentSceneName = "none";


        [Inject]
        public void Construct()
        {

            if (_screensConfig == null) throw new ArgumentNullException(nameof(_screensConfig));
            if (_screensConfig.rootCanvas == null) throw new InvalidOperationException("Root canvas is not set in ScreensConfig");
            UpdateSceneCache();

            _rootUIPersistent = UnityEngine.Object.Instantiate(_screensConfig.rootCanvas.gameObject);
            UnityEngine.Object.DontDestroyOnLoad(_rootUIPersistent.gameObject);
            _loadingTasks = new();
            _sceneScreenPairs = new();
            HandleSceneLoadEvent(_currentSceneName);
            BuildSceneScreenAssociation();
            BuildScreenCache();

            disposables.Add(_eventBus.Subscribe<SceneLoadedEvent>(@event => HandleSceneLoadEvent(@event.SceneName)));
            disposables.Add(_eventBus.Subscribe<SceneUnloadEvent>(@event => HandleSceneUnloadEvent(@event.SceneName)));

        }

        private void UpdateSceneCache()
        {
            _currentSceneName = SceneManager.GetActiveScene().name;
        }
        private void BuildSceneScreenAssociation()
        {
            foreach (var screenData in _screensConfig.Screens)
            {
                if (screenData.Persistent)
                {
                    continue;
                }

                if (!_sceneScreenPairs.ContainsKey(screenData.Scene.SceneName))
                {
                    _sceneScreenPairs.Add(screenData.Scene.SceneName, new());
                }

                if (_sceneScreenPairs[screenData.Scene.SceneName] == null)
                {
                    _sceneScreenPairs[screenData.Scene.SceneName] = new();
                }
                _sceneScreenPairs[screenData.Scene.SceneName].Add(screenData.Type);
            }
        }

        private void BuildScreenCache()
        {
            _screenCache = new();
            foreach (var screenData in _screensConfig.Screens)
            {
                if (screenData.Type != null && !_screenCache.ContainsKey(screenData.Type))
                {
                    ScreenCache screenCache = new(screenData.Asset, screenData.Persistent, screenData.InitializeOnStart);
                    _screenCache[screenData.Type] = screenCache;

                    if (screenData.InitializeOnStart)
                    {
                        if (screenData.Persistent || screenData.Scene.SceneName == _currentSceneName)
                        {
                            CreateAsync(screenData.Type).Forget();
                        }
                    }
                }
            }

        }

        private void HandleSceneLoadEvent(string sceneName)
        {
            _rootUI = UnityEngine.Object.Instantiate(_screensConfig.rootCanvas.gameObject);
            _currentSceneName = sceneName;
            if (_sceneScreenPairs.TryGetValue(sceneName, out var types))
            {
                foreach (var type in types)
                {
                    if (_screenCache.TryGetValue(type, out var screenCache))
                    {
                        if (screenCache.InitializeOnStart)
                        {
                            CreateAsync(type).Forget();
                        }
                    }
                }
            }
        }

        private void HandleSceneUnloadEvent(string sceneName)
        {
            var typesToCancel = new List<System.Type>();

            foreach (var sceneScreenPair in _sceneScreenPairs)
            {
                var types = sceneScreenPair.Value;
                foreach (var type in types)
                {
                    if (sceneScreenPair.Key == sceneName)
                    {
                        if (_loadingTasks.TryGetValue(type, out var _))
                        {
                            typesToCancel.Add(type);
                            continue;
                        }
                        _screenCache[type].Asset.ReleaseAsset();
                    }
                }
            }

            foreach (var type in typesToCancel)
            {
                if (_loadingTasks.TryGetValue(type, out var loadingData))
                {
                    loadingData.cts?.Cancel();
                    loadingData.cts?.Dispose();
                    _loadingTasks.Remove(type);

                    ReleaseAddressableAsset(type).Forget();
                }
            }

        }

        public async UniTask<View> CreateAsync(System.Type viewType)
        {
            if (!_screenCache.TryGetValue(viewType, out var data))
            {
                throw new System.Exception($"No screen data found for type: {viewType}");
            }

            lock (_loadingTasksLock)
            {
                if (_loadingTasks.TryGetValue(viewType, out var existingLoadingData))
                {
                    existingLoadingData.cts?.Cancel();
                    existingLoadingData.cts?.Dispose();
                    _loadingTasks.Remove(viewType);
                }
            }

            var cts = new CancellationTokenSource();
            var loadingTask = LoadPrefabAsync(data.Asset, cts.Token);

            _loadingTasks[viewType] = (loadingTask, cts);

            try
            {
                GameObject prefabObj = await loadingTask;
                _loadingTasks.Remove(viewType);

                return CreateScreenFromPrefab(prefabObj, viewType, data);
            }
            catch (OperationCanceledException)
            {
                _loadingTasks.Remove(viewType);
                throw;
            }
        }

        private View CreateScreenFromPrefab(GameObject prefabObj, System.Type viewType, ScreenCache screenCache)
        {
            var prefab = prefabObj.GetComponent(viewType) as View;
            if (prefab == null)
            {
                throw new InvalidOperationException($"Prefab for {viewType} doesn't have the required View component");
            }
            var rootUI = screenCache.Persistent ? _rootUIPersistent : _rootUI;
            var screen = _viewsFactory.Create(prefab, rootUI.transform);
            screen.transform.localScale = Vector3.one;
            return screen;
        }


        private async UniTask<GameObject> LoadPrefabAsync(AssetReferenceGameObject assetRef, CancellationToken cancellationToken = default)
        {
            var handle = assetRef.LoadAssetAsync<GameObject>();

            try
            {
                await handle.WithCancellation(cancellationToken);
                return handle.Result;
            }
            catch (OperationCanceledException)
            {
                if (handle.IsValid())
                {
                    Addressables.Release(handle);
                }
                throw;
            }
        }

        private async UniTask ReleaseAddressableAsset(System.Type viewType)
        {
            if (_screenCache.TryGetValue(viewType, out var screenData))
            {
                try
                {
                    screenData.Asset.ReleaseAsset();
                }
                catch (Exception ex)
                {
                    Debug.LogWarning($"Failed to release addressable asset for {viewType}: {ex.Message}");
                }
            }
        }

        public void CancelAllLoadingTasks()
        {
            foreach (var loadingTask in _loadingTasks.Values)
            {
                loadingTask.cts?.Cancel();
                loadingTask.cts?.Dispose();
            }

            _loadingTasks.Clear();
        }

        public void Dispose()
        {
            CancelAllLoadingTasks();
            disposables?.Dispose();
        }

        public struct ScreenCache
        {
            public AssetReferenceGameObject Asset;
            public bool Persistent;
            public bool InitializeOnStart;
            public ScreenCache(AssetReferenceGameObject asset, bool persistent, bool initializeOnStart)
            {
                Asset = asset;
                Persistent = persistent;
                InitializeOnStart = initializeOnStart;
            }
        }

    }

}