using UnityEngine;
using Zenject;

public class DayInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<StallService>().FromComponentInHierarchy().AsSingle().NonLazy();
        Container.Bind<CustomerService>().FromComponentInHierarchy().AsSingle().NonLazy();
        Container.Bind<WordBook>().FromComponentInHierarchy().AsSingle().NonLazy();
        Container.Bind<StallBoxUI>().FromComponentInHierarchy().AsSingle().NonLazy();

        Container.Bind<ButtonExtended>().FromComponentsInHierarchy().AsSingle().NonLazy();
    }
}
