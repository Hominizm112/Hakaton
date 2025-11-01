using System;
using System.Collections.Generic;
using System.Reflection;
using Cysharp.Threading.Tasks;
using GameCore.Factories;
using GameCore.UI;
using GameCore.UI.Loading;
using UnityEngine.AddressableAssets;
using Zenject;

namespace GameCore.Services
{
    public class ScreensService : Service, ISceneChangable
    {
        [Inject] private readonly ScreensFactory _screensFactory;

        private readonly Dictionary<Type, View> _screensByType = new();
        private readonly Stack<View> _screensStack = new();

        public async UniTask<TScreen> OpenAsync<TScreen>() where TScreen : View
        {
            if (_screensByType.TryGetValue(typeof(TScreen), out var screen))
            {
                if (screen)
                {
                    screen.Open();
                    _screensStack.Push(screen);
                    return (TScreen)screen;
                }
            }

            var newScreen = await _screensFactory.CreateAsync<TScreen>();

            newScreen.Open();
            _screensByType[typeof(TScreen)] = newScreen;
            _screensStack.Push(newScreen);

            return newScreen;
        }

        public async UniTask OpenAsync(Type viewType)
        {
            MethodInfo method = typeof(ScreensService).GetMethod("OpenAsync");
            MethodInfo genericMethod = method.MakeGenericMethod(viewType);
            await (UniTask)genericMethod.Invoke(this, null);
        }

        public void Close()
        {
            if (_screensStack.TryPop(out var screen))
            {
                screen.Close();
                screen.gameObject.SetActive(false);
            }
        }

        public void DestroyScreens()
        {
            foreach (var screenPair in _screensByType)
            {
                var screen = screenPair.Value;
                if (screen == null) continue;

                screen.Close();

                if (screen is IDisposable disposable)
                {
                    disposable.Dispose();
                }

                if (screen.gameObject != null)
                {
                    Addressables.ReleaseInstance(screen.gameObject);
                }
            }

            _screensByType.Clear();
            _screensStack.Clear();
        }

        public void SceneChanged()
        {
            DestroyScreens();
        }
    }
}
