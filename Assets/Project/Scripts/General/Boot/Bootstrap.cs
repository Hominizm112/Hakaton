using UnityEngine;

public class Bootstrap : MonoBehaviour
{
    [SerializeField] private string _sceneToLoad = "MainMenu";

    private Mediator _mediator;
    private AudioHub _audioHub;
    private SaveManager _saveManager;
    private InputManager _inputManager;
    private DragManager _dragManager;
    private TransitionScreen _transitionScreen;
    private PlayerController _playerController;
    private CurrencyPresenter _playerCurrencyPresenter;
    private NPCService _npcService;
    private ShopkeeperService _shopkeeperService;
    private ConsoleService _consoleService;
    private TimeService _timeService;

    private void Awake()
    {
        _mediator = Instantiate(Resources.Load<Mediator>("Prefabs/Mediator"));
        _audioHub = Instantiate(Resources.Load<AudioHub>("Prefabs/AudioHub"));
        _saveManager = Instantiate(Resources.Load<SaveManager>("Prefabs/SaveManager"));
        _inputManager = Instantiate(Resources.Load<InputManager>("Prefabs/InputManager"));
        _dragManager = Instantiate(Resources.Load<DragManager>("Prefabs/DragManager"));
        _transitionScreen = Instantiate(Resources.Load<TransitionScreen>("Prefabs/TransitionScreen"));

        _playerController = Instantiate(Resources.Load<PlayerController>("Prefabs/PlayerController"));
        _playerCurrencyPresenter = Instantiate(Resources.Load<CurrencyPresenter>("Prefabs/CurrencyPresenter"));

        _npcService = Instantiate(Resources.Load<NPCService>("Prefabs/NPCService"));

        _shopkeeperService = Instantiate(Resources.Load<ShopkeeperService>("Prefabs/ShopkeeperService"));

        _consoleService = Instantiate(Resources.Load<ConsoleService>("Prefabs/ConsoleService"));

        _timeService = Instantiate(Resources.Load<TimeService>("Prefabs/TimeService"));

        RegisterPersistent(_mediator);
        RegisterPersistent(_audioHub);
        RegisterPersistent(_inputManager);
        RegisterPersistent(_dragManager);
        RegisterPersistent(_transitionScreen);
        RegisterPersistent(_playerController);
        RegisterPersistent(_playerCurrencyPresenter);
        RegisterPersistent(_npcService);
        RegisterPersistent(_shopkeeperService);
        RegisterPersistent(_consoleService);
        RegisterPersistent(_timeService);

        _mediator.RegisterService(_audioHub);
        _mediator.RegisterService(_inputManager);
        _mediator.RegisterService(_saveManager);
        _mediator.RegisterService(_playerCurrencyPresenter);
        _mediator.RegisterService(_shopkeeperService);
        _mediator.RegisterService(_consoleService);
        _mediator.RegisterService(_timeService);

        _mediator.RegisterInitializable(_dragManager);
        _mediator.RegisterInitializable(_transitionScreen);
        _mediator.RegisterInitializable(_playerController);
        _mediator.RegisterInitializable(_playerCurrencyPresenter, true);


        _playerCurrencyPresenter.AddCurrency(1000);

        _mediator.RegisterInitializable(_npcService);



        _mediator.LoadScene(_sceneToLoad, Game.State.Gameplay, false);

        _mediator.SubscribeToState(Game.State.Gameplay, (_) => _mediator.InitializeAll());




        _saveManager.LoadSaveData();
    }

    public void RegisterPersistent<T>(T obj) where T : Object
    {
        DontDestroyOnLoad(obj);
    }
}
