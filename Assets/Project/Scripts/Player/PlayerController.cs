using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using Zenject;

public class PlayerController : MonoService
{
    [Inject] private InputManager _inputManager;


    public void Initialize(Mediator mediator)
    {
        _mediator = mediator;

        _inputManager.TryGetAction("PointerClick", out InputAction inputAction);
        inputAction.performed += OnInputAction;

    }

    private void OnInputAction(InputAction.CallbackContext ctx)
    {
        if (IsPointerOverUI())
        {
            return;
        }


        var mousePosition = _inputManager.GetVector2("Point");
        var hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(mousePosition), Vector2.zero, Mathf.Infinity);

        if (hit.collider == null)
        {
            TryMoveInDialogue();
            return;
        }

        var interactionObject = hit.collider.gameObject.GetComponent<InteractionObject>();

        Debug.Log($"Interacted with {hit.collider.gameObject.name}");

        if (interactionObject == null)
        {
            return;
        }

        interactionObject.Interact();

    }


    private bool IsPointerOverUI()
    {
        if (Mouse.current != null)
        {
            var mousePosition = Mouse.current.position.ReadValue();
            return IsScreenPointOverUI(mousePosition);
        }

        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.isInProgress)
        {
            var touchPosition = Touchscreen.current.primaryTouch.position.ReadValue();
            return IsScreenPointOverUI(touchPosition);
        }

        return false;
    }

    private bool IsScreenPointOverUI(Vector2 screenPosition)
    {
        var eventData = new PointerEventData(EventSystem.current)
        {
            position = screenPosition
        };

        var results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        return results.Count > 0;
    }
    private void TryMoveInDialogue()
    {
        _mediator.TryGetService<NPCService>(out var service);
        service?.HandleMoveInDialogue();
    }

}
