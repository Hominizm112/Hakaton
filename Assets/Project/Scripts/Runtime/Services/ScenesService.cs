using System;
using Cysharp.Threading.Tasks;
using TeaGame.Runtime.Configs;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace TeaGame.Runtime.Services
{
    public class ScenesService : Service
    {

        private bool _bootstrapBooted = false;
        private ScenesServiceConfig _config;
        private bool _sceneLoading = false;

        public bool LoadingScreenLiftRequested;


        [Inject]
        public void Construct(ScenesServiceConfig config)
        {
            SERVICE_NAME = GetType().ToString();
            _config = config;

            disposables.Add(GlobalEventBus.Subscribe<LoadingScreenLiftEvent>(_ => LoadingScreenLiftRequested = false));

            if (!_bootstrapBooted)
            {
                // LoadBootstrap();
            }
        }

        private void LoadBootstrap()
        {
            _bootstrapBooted = true;
            if (!_config || string.IsNullOrEmpty(_config.bootstrapSceneName))
            {
                ThrowNullArgument($"Config is null or name of bootstrap scene was not set!");
                return;
            }

            LoadScene(_config.bootstrapSceneName, GameService.State.Boot);
        }


        public void LoadScene(string sceneName, GameService.State targetState, bool useTransitionScreen = true)
        {
            if (SceneManager.GetActiveScene().name == sceneName || _sceneLoading)
            {
                return;
            }
            else
            {
                LoadSceneAsync(sceneName, targetState).Forget();
            }
        }

        private async UniTask LoadSceneAsync(string sceneName, GameService.State targetState)
        {
            GlobalEventBus.Publish<SceneUnloadEvent>(new(SceneManager.GetActiveScene().name));
            GlobalEventBus.Publish<SceneStartLoadEvent>(new(sceneName, targetState));


            _sceneLoading = true;
            AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
            operation.allowSceneActivation = false;

            await WaitForSceneLoad(operation);

            GlobalEventBus.Publish<SceneLoadedEvent>(new(sceneName));
            LoadingScreenLiftRequested = true;

        }

        private async UniTask WaitForSceneLoad(AsyncOperation operation)
        {
            while (!operation.isDone)
            {
                if (operation.progress >= 0.9f)
                {
                    operation.allowSceneActivation = true;
                }

                await UniTask.Yield();
            }
            _sceneLoading = false;
        }


    }
}