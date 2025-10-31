using UnityEngine;
using UnityEngine.InputSystem.Interactions;
using Zenject;

public class Bootstrap : MonoBehaviour
{
    [SerializeField] private string _sceneToLoad = "MainMenu";

    [Inject] private Mediator _mediator;
    [Inject] private CurrencyPresenter _playerCurrencyPresenter;
    [Inject] private SaveManager _saveService;

    [Inject]
    public void Construct()
    {
        InitializeGame();
    }

    private void InitializeGame()
    {
        _playerCurrencyPresenter.AddCurrency(1000);
        _mediator.LoadScene(_sceneToLoad, Game.State.Gameplay, false);
        _mediator.SubscribeToState(Game.State.Gameplay, (_) => _mediator.InitializeAll());

        InjectEvents();
        _ = _saveService.LoadDataAsync();
    }

    private void InjectEvents()
    {
        _mediator.GlobalEventBus.Subscribe<SceneLoadedEvent>((e) =>
        {
            if (e.SceneName == "DayScene")
            {
                _mediator.GlobalEventBus.Publish<TimeTrackStartEvent>(new(minutes: 600));
            }
        });

        _mediator.GlobalEventBus.Subscribe<SceneUnloadEvent>((e) =>
        {
            if (e.SceneName == "DayScene")
            {
                _mediator.GlobalEventBus.Publish<TimeTrackStopEvent>(new());
                // _mediator.GetService<SaveManager>().SaveData();
            }

            if (e.SceneName == "PC_TEST")
            {
                // _mediator.GetService<SaveManager>().SaveData();
            }
        });

        // _mediator.GlobalEventBus.Subscribe<TimeTrackCompletedEvent>((e) =>
        // {
        //     _mediator.LoadScene("PC_TEST", Game.State.NightScene);
        // });



    }
}
