using GameCore.Configs;
using GameCore.Utils;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

public class ProjectInstaller : MonoInstaller
{
    [SerializeField] private ScreensConfig screensConfig;
    [SerializeField] private InputActionAsset inputActions;

    public override void InstallBindings()
    {
        BindCoreServices();
        BindMVVM();
        BindStates();
        BindSecondaryServices();
    }

    private void BindCoreServices()
    {
        Container.BindInterfacesAndSelfTo<EventBus>().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<InputManager>().AsSingle().WithArguments(inputActions).NonLazy();
        Container.BindInterfacesAndSelfTo<TimeService>().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<CurrencyPresenter>().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<ObjectRegistry>().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<SaveManager>().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<AudioHub>().AsSingle().NonLazy();

    }

    private void BindStates()
    {
        Container.BindInterfacesAndSelfTo<TeaGame.States.InventoryState>().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<TeaGame.States.StallState>().AsSingle().NonLazy();

    }

    private void BindMVVM()
    {
        Container.BindInterfacesAndSelfTo<GameCore.Services.ScreensService>().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<GameCore.Factories.ViewModelFactory>().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<GameCore.Factories.ViewsFactory>().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<GameCore.Factories.ScreensFactory>().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<ScreensConfig>().FromScriptableObject(screensConfig).AsSingle().NonLazy();
    }

    private void BindSecondaryServices()
    {
        Container.BindInterfacesAndSelfTo<InventoryService>().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<TeaGame.Services.TeaMixerService>().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<WordBookService>().AsSingle().NonLazy();
    }

}
