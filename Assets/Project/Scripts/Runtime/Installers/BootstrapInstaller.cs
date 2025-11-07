using UnityEngine;
using Zenject;

public class BootstrapInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<Bootstrap>().FromComponentInNewPrefabResource("Prefabs/Bootstrap").AsSingle().NonLazy();
    }

}
