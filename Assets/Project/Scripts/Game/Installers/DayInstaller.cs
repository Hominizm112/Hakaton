using System.ComponentModel;
using UnityEngine;
using Zenject;

public class DayInstaller : MonoInstaller
{
    [SerializeField] private GameObject baseCanvas;
    public override void InstallBindings()
    {
        Container.Bind<StallService>().FromComponentInHierarchy().AsSingle().NonLazy();
        Container.Bind<CustomerService>().FromComponentInHierarchy().AsSingle().NonLazy();
        Container.Bind<StallBoxUI>().FromComponentInHierarchy().AsSingle().NonLazy();
        Container.Bind<TeaMixService>().FromComponentInHierarchy().AsSingle().NonLazy();


        Container.Bind<ButtonExtended>().FromComponentsInHierarchy().AsSingle().NonLazy();
        Container.Bind<MonoComponent>().FromComponentsInHierarchy().AsSingle().NonLazy();


        Container.Bind<InventoryView>().FromComponentsInHierarchy().AsSingle().NonLazy();
        Container.Bind<ViewOpener>().FromComponentsInHierarchy().AsSingle().NonLazy();

    }
}
