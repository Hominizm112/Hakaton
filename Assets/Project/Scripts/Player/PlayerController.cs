using UnityEngine;

public class PlayerController : MonoBehaviour, IInitializable
{
    private Mediator _mediator;
    private InputManager _inputManager;

    public void Initialize(Mediator mediator)
    {
        _mediator = mediator;
        _inputManager = mediator.GetService<InputManager>();

        mediator.GlobalEventBus.Subscribe<InputActionEvent>(OnInputAction);
    }

    private void OnInputAction(InputActionEvent inputEvent)
    {

    }
}
