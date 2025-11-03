using TeaGame.Views;
using Zenject;

public class DayInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<ButtonExtended>().FromComponentsInHierarchy().AsSingle().NonLazy();
        Container.Bind<MonoComponent>().FromComponentsInHierarchy().AsSingle().NonLazy();

        Container.BindInterfacesAndSelfTo<IViewOpener>().FromComponentsInHierarchy().AsSingle().NonLazy();

    }
}
