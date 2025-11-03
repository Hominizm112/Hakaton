using GameCore.Configs;
using GameCore.Utils;
using UnityEngine;
using Zenject;

public class ProjectInstaller : MonoInstaller
{
    [SerializeField] private ScreensConfig screensConfig;
    public override void InstallBindings()
    {
        Container.Bind<ObjectRegistry>().AsSingle().NonLazy();

        Container.BindInterfacesAndSelfTo<SaveManager>().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<CurrencyPresenter>().AsSingle().NonLazy();

        CreateAndBindAllServices();


        Container.BindInterfacesAndSelfTo<ServiceFactory>().AsSingle().NonLazy();



        Container.Bind<EventBus>().AsSingle();

        BindMVVM();
        BindInventory();
        BindStates();
        BindServices();
    }

    private void CreateAndBindAllServices()
    {
        BindPersistentService<Mediator>();
        BindPersistentService<AudioHub>();
        BindPersistentService<InputManager>();
        // BindPersistentService<DragManager>();
        BindPersistentService<TransitionScreen>();
        BindPersistentService<PlayerController>();
        // BindPersistentService<ShopkeeperService>();
        BindPersistentService<ConsoleService>();
        BindPersistentService<TimeService>();
        BindPersistentService<PortfollioService>();

    }

    private void BindInventory()
    {
        Container.BindInterfacesAndSelfTo<TeaGame.States.InventoryState>().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<InventoryService>().AsSingle().NonLazy();
        // Container.Bind<InventoryViewModel>().AsSingle();
    }

    private void BindStates()
    {
        Container.BindInterfacesAndSelfTo<TeaGame.States.StallState>().AsSingle().NonLazy();
    }

    private void BindServices()
    {
        Container.BindInterfacesAndSelfTo<TeaGame.Services.TeaMixerService>().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<WordBookService>().AsSingle().NonLazy();
    }

    private void BindMVVM()
    {
        Container.BindInterfacesAndSelfTo<GameCore.Services.ScreensService>().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<GameCore.Factories.ViewModelFactory>().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<GameCore.Factories.ViewsFactory>().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<GameCore.Factories.ScreensFactory>().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<ScreensConfig>().FromScriptableObject(screensConfig).AsSingle().NonLazy();
    }












    private void BindPersistentService<T>() where T : Component
    {
        BindService<T>();
    }

    private T CreatePersistentService<T>(InjectContext context) where T : Component
    {
        var factory = context.Container.Resolve<IPersistentServiceFactory>();
        return factory.CreatePersistentService<T>();
    }

    private void BindService<T>() where T : Component
    {
        Container.Bind<T>().FromMethod(CreateService<T>).AsSingle().NonLazy();
    }

    private T CreateService<T>(InjectContext context) where T : Component
    {
        var factory = context.Container.Resolve<IServiceFactory>();
        return factory.CreateService<T>();
    }
}
