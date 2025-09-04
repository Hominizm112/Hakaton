using UnityEngine;

public class Bootstrap : MonoBehaviour
{
    [SerializeField] private string _sceneToLoad = "MainMenu";

    private Mediator _mediator;
    private AudioHub _audioHub;
    private SaveManager _saveManager;
    private InputManager _inputManager;
    private DragManager _dragManager;

    private void Awake()
    {
        _mediator = Instantiate(Resources.Load<Mediator>("Prefabs/Mediator"));
        _audioHub = Instantiate(Resources.Load<AudioHub>("Prefabs/AudioHub"));
        _saveManager = Instantiate(Resources.Load<SaveManager>("Prefabs/SaveManager"));
        _inputManager = Instantiate(Resources.Load<InputManager>("Prefabs/InputManager"));
        _dragManager = Instantiate(Resources.Load<DragManager>("Prefabs/DragManager"));

        DontDestroyOnLoad(_mediator);
        DontDestroyOnLoad(_audioHub);
        DontDestroyOnLoad(_inputManager);
        DontDestroyOnLoad(_dragManager);

        _mediator.RegisterService<AudioHub>(_audioHub);
        _mediator.RegisterService<SaveManager>(_saveManager);

        _mediator.RegisterInitializable(_inputManager);
        _mediator.RegisterInitializable(_dragManager);

        _mediator.LoadScene(_sceneToLoad, Game.State.Gameplay);

        _mediator.SubscribeToState(Game.State.Gameplay, (_) => _mediator.InitializeAll());

        _saveManager.LoadSaveData();
    }
}
