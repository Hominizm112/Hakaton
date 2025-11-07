using UnityEngine;
using Zenject;

public class PCInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<AppController>().FromComponentInHierarchy().AsSingle().NonLazy();
        Container.Bind<AppLoader>().FromComponentInHierarchy().AsSingle().NonLazy();
        Container.Bind<GraphRunnerService>().AsSingle().NonLazy();

        Container.Bind<TweenGraphRunner>().FromComponentsInHierarchy().AsSingle().NonLazy();
        Container.Bind<ButtonExtended>().FromComponentsInHierarchy().AsSingle().NonLazy();
        Container.Bind<BaseApp>().FromComponentsInHierarchy().AsSingle().NonLazy();
    }
}
