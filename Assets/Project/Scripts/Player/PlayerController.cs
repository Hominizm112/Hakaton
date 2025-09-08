using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour, IInitializable
{
    private Mediator _mediator;
    private InputManager _inputManager;
    private Camera _mainCamera;
    private Action act;


    public void Initialize(Mediator mediator)
    {
        _mediator = mediator;
        _inputManager = mediator.GetService<InputManager>();
        _mainCamera = Camera.main;

        mediator.GetService<InputManager>().TryGetAction("PointerClick", out InputAction inputAction);
        inputAction.performed += OnInputAction;

    }

    private void OnInputAction(InputAction.CallbackContext ctx)
    {
        var mousePosition = _inputManager.GetVector2("Point");
        var hit = Physics2D.Raycast(_mainCamera.ScreenToWorldPoint(mousePosition), Vector2.zero, Mathf.Infinity);

        if (hit.collider == null)
        {
            return;
        }

        var interactionObject = hit.collider.gameObject.GetComponent<InteractionObject>();

        if (interactionObject == null)
        {
            return;
        }

        interactionObject.Interact();

    }

}
