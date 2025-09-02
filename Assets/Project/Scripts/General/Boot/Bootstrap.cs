using UnityEngine;
using UnityEngine.SceneManagement;

public class Bootstrap : MonoBehaviour
{
    [SerializeField] private string _sceneToLoad = "MainMenu";

    private Mediator _mediator;
    private AudioHub _audioHub;
    private SaveManager _saveManager;

    private void Awake()
    {
        _mediator = Instantiate(Resources.Load<Mediator>("Prefabs/Mediator"));
        _audioHub = Instantiate(Resources.Load<AudioHub>("Prefabs/AudioHub"));
        _saveManager = Instantiate(Resources.Load<SaveManager>("Prefabs/SaveManager"));

        DontDestroyOnLoad(_mediator);
        DontDestroyOnLoad(_audioHub);

        _mediator.RegisterService<AudioHub>(_audioHub);
        _mediator.RegisterService<SaveManager>(_saveManager);

        SceneManager.LoadScene(_sceneToLoad);

        _saveManager.LoadSaveData();
    }
}
