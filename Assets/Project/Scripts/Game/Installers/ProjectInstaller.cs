using UnityEngine;
using Zenject;

public class ProjectInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        CreateAndBindAllServices();

        Container.BindInterfacesAndSelfTo<ServiceFactory>().AsSingle();


        Container.Bind<EventBus>().AsSingle();

    }

    private void CreateAndBindAllServices()
    {
        BindPersistentService<Mediator>();
        BindPersistentService<CurrencyPresenter>();
        BindPersistentService<SaveManager>();
        BindPersistentService<AudioHub>();
        BindPersistentService<InputManager>();
        BindPersistentService<DragManager>();
        BindPersistentService<TransitionScreen>();
        BindPersistentService<PlayerController>();
        BindPersistentService<ShopkeeperService>();
        BindPersistentService<ConsoleService>();
        BindPersistentService<TimeService>();
        BindPersistentService<PortfollioService>();

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
