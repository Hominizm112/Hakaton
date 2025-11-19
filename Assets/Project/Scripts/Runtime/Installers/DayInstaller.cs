using TeaGame.Runtime.Services;
using UnityEngine.SceneManagement;
using Zenject;

public class DayInstaller : MonoInstaller
{
    [Inject] private EventBus _eventBus;
    public override void InstallBindings()
    {
        Container.Bind<ButtonExtended>().FromComponentsInHierarchy().AsSingle().NonLazy();

        Container.BindInterfacesAndSelfTo<IViewOpener>().FromComponentsInHierarchy().AsSingle().NonLazy();


        TimeService timeService = new();
        timeService.StartTrackMinutes(500);
        Container.Inject(timeService);

        _eventBus.Subscribe<SceneUnloadEvent>(_ => timeService.ForceStopTracking());
    }
}
