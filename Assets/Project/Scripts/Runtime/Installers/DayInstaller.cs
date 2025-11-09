using Zenject;

public class DayInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<ButtonExtended>().FromComponentsInHierarchy().AsSingle().NonLazy();

        Container.BindInterfacesAndSelfTo<IViewOpener>().FromComponentsInHierarchy().AsSingle().NonLazy();

        TimeService timeService = new();
        timeService.StartTrackMinutes(5);
        Container.Inject(timeService);
    }
}
