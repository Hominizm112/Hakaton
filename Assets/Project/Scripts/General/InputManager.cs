using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

#region Events
public interface IInputEvent : IEvent { }

public class InputActionEvent : IInputEvent
{
    public InputAction.CallbackContext Context { get; }
    public string ActionName { get; }

    public InputActionEvent(InputAction.CallbackContext context, string actionName)
    {
        Context = context;
        ActionName = actionName;
    }

}

public class DragStartedEvent : IInputEvent
{
    public Vector2 ScreenPosition { get; }

    public DragStartedEvent(Vector2 screenPosition)
    {
        ScreenPosition = screenPosition;
    }
}

public class DragContinuedEvent : IInputEvent
{
    public Vector2 ScreenPosition { get; }
    public Vector2 Delta { get; }
    public Vector2 Velocity { get; }
    public Vector2 Direction { get; }

    public DragContinuedEvent(Vector2 screenPosition, Vector2 delta, Vector2 velocity, Vector2 direction)
    {
        ScreenPosition = screenPosition;
        Delta = delta;
        Velocity = velocity;
        Direction = direction;
    }
}

public class DragEndedEvent : IInputEvent
{
    public Vector2 ScreenPosition { get; }

    public DragEndedEvent(Vector2 screenPosition)
    {
        ScreenPosition = screenPosition;
    }
}

public class InputEnabledEvent : IInputEvent
{
    public bool IsEnabled { get; }

    public InputEnabledEvent(bool isEnabled)
    {
        IsEnabled = isEnabled;
    }
}


#endregion


#region InputService

public class InputManager : MonoService, IStateListener
{
    [SerializeField] private InputActionAsset _inputActions;
    private Mediator _mediator;
    private Dictionary<string, InputAction> _actionMap = new();
    private List<InputAction> _allActions = new();
    private bool _isInputEnabled = true;

    public override void Initialize(Mediator mediator)
    {
        base.Initialize();
        _mediator = mediator;

        mediator.SubscribeToState(this, Game.State.Gameplay);
        mediator.SubscribeToState(this, Game.State.Paused);
        mediator.SubscribeToState(this, Game.State.Menu);

        InitializeInputActions();
    }

    private void DebugInputActions()
    {
        if (_inputActions == null)
        {
            Debug.LogError("InputActionAsset is not assigned!");
            return;
        }

        foreach (var actionMap in _inputActions.actionMaps)
        {
            Debug.Log($"ActionMap: {actionMap.name}");
            foreach (var action in actionMap.actions)
            {
                Debug.Log($"- Action: {action.name}");
                foreach (var binding in action.bindings)
                {
                    Debug.Log($"  Binding: {binding.path}");
                }
            }
        }
    }

    private void InitializeInputActions()
    {
        if (_inputActions == null)
        {
            Debug.LogError("InputActionAsset is not assigned!");
            return;
        }

        foreach (var actionMap in _inputActions.actionMaps)
        {
            actionMap.Enable();

            foreach (var action in actionMap.actions)
            {
                _actionMap[action.name] = action;
                _allActions.Add(action);

                action.performed += ctx => OnInputActionPerformed(ctx);
                action.canceled += ctx => OnInputActionCanceled(ctx);
                action.started += ctx => OnInputActionStarted(ctx);
            }
        }
    }

    public void OnStateChanged(Game.State state)
    {
        switch (state)
        {
            case Game.State.Gameplay:
                SetInputEnabled(true);
                break;
            case Game.State.Paused:
            case Game.State.Menu:
                SetInputEnabled(false);
                break;
            case Game.State.Loading:
                SetInputEnabled(false);
                break;
        }
    }

    public void SetInputEnabled(bool enabled)
    {
        if (_isInputEnabled == enabled) return;

        _isInputEnabled = enabled;

        foreach (var actionMap in _inputActions.actionMaps)
        {
            if (enabled)
            {
                actionMap.Enable();
            }
            else
            {
                actionMap.Disable();
            }
        }

        _mediator.GlobalEventBus.Publish(new InputEnabledEvent(enabled));
    }

    public bool IsInputEnabled() => _isInputEnabled;

    public InputAction GetAction(string actionName)
    {
        if (_actionMap.TryGetValue(actionName, out var action))
        {
            return action;
        }

        Debug.LogWarning($"Input action '{actionName}' not found!");
        return null;
    }

    public bool TryGetAction(string actionName, out InputAction action)
    {
        return _actionMap.TryGetValue(actionName, out action);
    }

    public float GetAxis(string actionName)
    {
        var action = GetAction(actionName);
        return action?.ReadValue<float>() ?? 0f;
    }

    public Vector2 GetVector2(string actionName)
    {
        var action = GetAction(actionName);
        return action?.ReadValue<Vector2>() ?? Vector2.zero;
    }

    public bool GetButton(string actionName)
    {
        var action = GetAction(actionName);
        return action?.IsPressed() ?? false;
    }

    public bool GetButtonDown(string actionName)
    {
        var action = GetAction(actionName);
        return action?.WasPressedThisFrame() ?? false;
    }

    public bool GetButtonUp(string actionName)
    {
        var action = GetAction(actionName);
        return action?.WasReleasedThisFrame() ?? false;
    }

    private void OnInputActionPerformed(InputAction.CallbackContext context)
    {
        if (!_isInputEnabled) return;

        _mediator.GlobalEventBus.Publish(new InputActionEvent(context, context.action.name));
    }

    private void OnInputActionStarted(InputAction.CallbackContext context)
    {
        if (!_isInputEnabled) return;

        _mediator.GlobalEventBus.Publish(new InputActionEvent(context, context.action.name));
    }

    private void OnInputActionCanceled(InputAction.CallbackContext context)
    {
        if (!_isInputEnabled) return;

        _mediator.GlobalEventBus.Publish(new InputActionEvent(context, context.action.name));
    }

    public void RebindAction(string actionName, int bindingIndex, InputBinding newBinding, Action<bool> callback = null)
    {
        var action = GetAction(actionName);
        if (action == null)
        {
            callback?.Invoke(false);
            return;
        }

        try
        {
            action.ApplyBindingOverride(bindingIndex, newBinding);
            callback?.Invoke(true);
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to rebind action '{actionName}': {e.Message}");
            callback?.Invoke(false);
        }
    }

    private void OnDestroy()
    {
        foreach (var action in _allActions)
        {
            action.performed -= OnInputActionPerformed;
            action.canceled -= OnInputActionCanceled;
            action.started -= OnInputActionStarted;
        }

        _allActions.Clear();

        if (_inputActions != null)
        {
            _inputActions.Disable();
        }

        if (_mediator != null)
        {
            _mediator.UnsubscribeFromState(this, Game.State.Gameplay);
            _mediator.UnsubscribeFromState(this, Game.State.Paused);
            _mediator.UnsubscribeFromState(this, Game.State.Menu);
        }
    }

    #region Utils


    /// <summary>
    /// Gets the GameObject under the mouse pointer (3D and 2D objects)
    /// </summary>
    /// <param name="layerMask">Layer mask to filter objects</param>
    /// <param name="maxDistance">Maximum raycast distance</param>
    /// <returns>The GameObject under mouse, or null if none</returns>
    public static GameObject GetObjectUnderMouse(LayerMask layerMask = default, float maxDistance = Mathf.Infinity)
    {
        if (layerMask.value == 0)
        {
            layerMask = -1;
        }

        if (Camera.main == null)
        {
            return null;
        }

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit3D, maxDistance, layerMask))
        {
            return hit3D.collider.gameObject;
        }

        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mousePos2D = new Vector2(mouseWorldPos.x, mouseWorldPos.y);


        RaycastHit2D hit2D = Physics2D.Raycast(mousePos2D, Vector2.zero, maxDistance, layerMask);

        if (hit2D.collider != null)
        {
            return hit2D.collider.gameObject;
        }

        return null;
    }




    /// <summary>
    /// Gets the GameObject under the mouse pointer (UI elements)
    /// </summary>
    /// <returns>The UI GameObject under mouse, or null if none</returns>
    public static GameObject GetUIUnderMouse()
    {
        if (EventSystem.current == null || !EventSystem.current.IsActive())
        {
            Debug.LogWarning("EventSystem is not available or active");
            return null;
        }

        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition
        };

        var results = new System.Collections.Generic.List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);

        if (results.Count > 0)
        {
            return results[0].gameObject;
        }

        return null;
    }



    /// <summary>
    /// Gets both UI and objects under mouse in one call
    /// </summary>
    /// <param name="layerMask">Layer mask for objects</param>
    /// <returns>Tuple with (uiObject, worldObject)</returns>
    public static (GameObject uiObject, GameObject worldObject) GetEverythingUnderMouse(LayerMask layerMask = default)
    {
        GameObject uiObject = GetUIUnderMouse();
        GameObject worldObject = GetObjectUnderMouse(layerMask);

        return (uiObject, worldObject);
    }


    /// <summary>
    /// Gets a specific component from the object under mouse
    /// </summary>
    /// <typeparam name="T">Component type to look for</typeparam>
    /// <param name="layerMask">Layer mask for objects</param>
    /// <returns>The component, or null if not found</returns>
    public static T GetComponentUnderMouse<T>(LayerMask layerMask = default) where T : Component
    {
        GameObject obj = GetObjectUnderMouse(layerMask);
        if (obj != null)
        {
            return obj.GetComponent<T>();
        }
        return null;
    }

    /// <summary>
    /// Gets a specific component from UI under mouse
    /// </summary>
    /// <typeparam name="T">Component type to look for</typeparam>
    /// <returns>The component, or null if not found</returns>
    public static T GetUIComponentUnderMouse<T>() where T : Component
    {
        GameObject uiObject = GetUIUnderMouse();
        if (uiObject != null)
        {
            return uiObject.GetComponent<T>();
        }
        return null;
    }


    #endregion


}

#endregion