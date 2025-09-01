using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInitializable
{
    void Initialize(Mediator mediator);
}

public interface IStateListener
{
    void OnStateChanged(Game.State state);
}

public interface IEvent { }

public class EventBus
{
    private readonly Dictionary<Type, List<Action<IEvent>>> _eventSubscriptions = new();

    public void Subscribe<T>(Action<T> handler) where T : IEvent
    {
        Type eventType = typeof(T);
        if (!_eventSubscriptions.ContainsKey(eventType))
        {
            _eventSubscriptions[eventType] = new List<Action<IEvent>>();
        }

        _eventSubscriptions[eventType].Add((e) => handler((T)e));
    }

    public void Unsubscribe<T>(Action<T> handler) where T : IEvent
    {
        Type eventType = typeof(T);
        if (_eventSubscriptions.TryGetValue(eventType, out var handlers))
        {
            handlers.RemoveAll(h => h.Target == handler.Target && h.Method == handler.Method);
        }
    }

    public void Publish<T>(T eventData) where T : IEvent
    {
        Type eventType = typeof(T);
        if (_eventSubscriptions.TryGetValue(eventType, out var handlers))
        {
            foreach (var handler in handlers.ToArray())
            {
                handler?.Invoke(eventData);
            }
        }
    }
}


public class Mediator : MonoBehaviour
{
    [System.Serializable]
    public class GameSettings
    {
        public float masterVolume = 1f;
        public float musicVolume = 1f;
        public float sfxVolume = 1f;
        public bool fullscreen = true;
        public int resolutionIndex = 0;
        public int qualityLevel = 2;
    }

    public static Mediator Instance { get; private set; }

    private readonly List<IInitializable> _initializables = new();
    private readonly Dictionary<Game.State, List<Action<Game.State>>> _stateChangeCallbacks = new();
    private readonly Dictionary<Type, object> _services = new Dictionary<Type, object>();


    public event Action<Game.State> OnStateChanged;
    public static event Action<float> OnLoadProgress;

    private Game.State _currentState;
    public Game.State CurrentState => _currentState;
    public EventBus GlobalEventBus { get; private set; }
    public GameSettings Settings { get; private set; } = new();


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        Instance = this;

        GlobalEventBus = new EventBus();
        InitializeStateDictionary();
    }

    private void InitializeStateDictionary()
    {
        foreach (Game.State state in Enum.GetValues(typeof(Game.State)))
        {
            _stateChangeCallbacks[state] = new List<Action<Game.State>>();
        }
    }

    public void RegisterInitializable(IInitializable initializable)
    {
        if (!_initializables.Contains(initializable))
        {
            _initializables.Add(initializable);
        }
    }

    public void InitializeAll()
    {
        foreach (var initializable in _initializables)
        {
            initializable.Initialize(this);
        }

        _initializables.Clear();
    }

    public void SetState(Game.State newState)
    {
        if (_currentState == newState)
        {
            return;
        }

        Game.State previousState = _currentState;
        _currentState = newState;

        Debug.Log($"Game State changed from {previousState} to {_currentState}");

        OnStateChanged?.Invoke(_currentState);

        InvokeStateCallback(_currentState);
    }

    public void SetState(string newStateName)
    {
        if (Enum.TryParse(newStateName, out Game.State newState))
        {
            SetState(newState);
        }
        else
        {
            Debug.LogError($"Attempted to set invalid state: {newStateName}");
        }
    }

    public void SubscribeToState(Game.State state, Action<Game.State> callback)
    {
        if (_stateChangeCallbacks.TryGetValue(state, out var callbackList))
        {
            if (!callbackList.Contains(callback))
            {
                callbackList.Add(callback);
            }
        }
    }

    public void SubscribeToState(IStateListener listener, Game.State state)
    {
        SubscribeToState(state, listener.OnStateChanged);
    }

    public void UnsubscribeFromState(Game.State state, Action<Game.State> callback)
    {
        if (_stateChangeCallbacks.TryGetValue(state, out var callbackList))
        {
            callbackList.Remove(callback);
        }
    }

    public void UnsubscribeFromState(IStateListener listener, Game.State state)
    {
        UnsubscribeFromState(state, listener.OnStateChanged);
    }

    public bool IsCurrentState(params Game.State[] states)
    {
        foreach (Game.State state in states)
        {
            if (_currentState == state) return true;
        }
        return false;
    }

    private void InvokeStateCallback(Game.State state)
    {
        if (_stateChangeCallbacks.TryGetValue(state, out var callbacks))
        {
            for (int i = callbacks.Count - 1; i >= 0; i--)
            {
                callbacks[i]?.Invoke(state);
            }
        }
    }

    public void RegisterService<T>(T service) where T : class
    {
        _services[typeof(T)] = service;
    }

    public T GetService<T>() where T : class
    {
        if (_services.TryGetValue(typeof(T), out object service))
        {
            return service as T;
        }

        Debug.LogError($"Service of type {typeof(T)} not registered!");
        return null;
    }

    public bool TryGetService<T>(out T service) where T : class
    {
        if (_services.TryGetValue(typeof(T), out object obj))
        {
            service = obj as T;
            return true;
        }

        service = null;
        return false;
    }

    public void LoadScene(string sceneName, Game.State targetState)
    {
        SetState("Loading");
        StartCoroutine(LoadSceneAsync(sceneName, targetState));
    }

    private IEnumerator LoadSceneAsync(string sceneName, Game.State targetState)
    {
        AsyncOperation operation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName);
        operation.allowSceneActivation = false;

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            OnLoadProgress?.Invoke(progress);

            if (operation.progress >= 0.9f)
            {
                operation.allowSceneActivation = true;
            }

            yield return null;
        }

        SetState(targetState);
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }

        OnStateChanged = null;
        foreach (var callbackList in _stateChangeCallbacks.Values)
        {
            callbackList.Clear();
        }
        _stateChangeCallbacks.Clear();
        _initializables.Clear();
    }
}
