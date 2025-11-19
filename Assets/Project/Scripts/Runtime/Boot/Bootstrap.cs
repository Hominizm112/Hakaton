using System.ComponentModel;
using TeaGame.Runtime.Services;
using UniRx;
using UnityEditor.Localization.Plugins.XLIFF.V12;
using UnityEngine;
using UnityEngine.InputSystem.Interactions;
using Zenject;

public class Bootstrap : MonoBehaviour
{
    [SerializeField] private string _sceneToLoad = "DayScene";

    [Inject] private CurrencyPresenter _playerCurrencyPresenter;
    [Inject] private ScenesService _scenesService;

    [Inject]
    public void Construct()
    {
        InitializeGame();
    }

    private void InitializeGame()
    {
        _playerCurrencyPresenter.AddCurrency(1000);
        _scenesService.LoadScene(_sceneToLoad, GameService.State.Gameplay);

        // _mediator.LoadScene(_sceneToLoad, GameService.State.Gameplay, false);
        // _mediator.SubscribeToState(GameService.State.Gameplay, (_) => _mediator.InitializeAll());

        // InjectEvents();
        // _ = _saveService.LoadDataAsync();
    }

    private void InjectEvents()
    {
        // _mediator.GlobalEventBus.Subscribe<SceneLoadedEvent>((e) =>
        // {
        //     if (e.SceneName == "DayScene")
        //     {
        // _mediator.GlobalEventBus.Publish<TimeTrackStartEvent>(new(minutes: 600));
        //     }
        // });

        // _mediator.GlobalEventBus.Subscribe<SceneUnloadEvent>((e) =>
        // {
        //     if (e.SceneName == "DayScene")
        //     {
        // _mediator.GlobalEventBus.Publish<TimeTrackStopEvent>(new());
        // _mediator.GetService<SaveManager>().SaveData();
        // }

        // if (e.SceneName == "PC_TEST")
        // {
        // _mediator.GetService<SaveManager>().SaveData();
        //     }
        // });

        // _mediator.GlobalEventBus.Subscribe<TimeTrackCompletedEvent>((e) =>
        // {
        //     _mediator.LoadScene("PC_TEST", GameService.State.NightScene);
        // });



    }
}
