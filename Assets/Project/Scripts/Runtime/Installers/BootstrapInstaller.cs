using UnityEngine;
using Zenject;

public class BootstrapInstaller : MonoInstaller
{
    [SerializeField] private GameObject bootstrapPrefab;
    public override void InstallBindings()
    {
        Container.Bind<Bootstrap>().FromComponentInNewPrefab(bootstrapPrefab).AsSingle().NonLazy();
    }

}
