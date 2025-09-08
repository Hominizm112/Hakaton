using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ButtonEventHandler : MonoBehaviour
{
    [SerializeField] public EventFactory.EventType eventType;
    [SerializeField] private string _sceneName = "MainScene";
    [SerializeField] private Game.State _targetState = Game.State.Loading;

    private Button _button;
    private void Awake()
    {
        _button = gameObject.GetComponent<Button>();
        _button?.onClick.AddListener(Handle);
    }
    public void Handle()
    {
        switch (eventType)
        {
            case EventFactory.EventType.LoadScene:
                EventFactory.CreateAndPublish<LoadSceneEvent>(Mediator.Instance, _sceneName, _targetState);
                break;
            default:
                Debug.LogError($"Handler for {eventType} does not exist.");
                break;

        }
    }
    private void OnDestroy()
    {
        _button?.onClick.RemoveListener(Handle);
    }
}
