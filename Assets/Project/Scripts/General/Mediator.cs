using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Rendering;
using UnityEngine;

public interface IInitializable
{
    void Initialize(Mediator mediator);
}



#region MonoService
public interface IService { }

public abstract class MonoService : MonoBehaviour, IService, IInitializable
{
    public virtual List<Type> requiredServices { get; protected set; } = new List<Type>();
    protected Dictionary<Type, IService> services = new Dictionary<Type, IService>();
    public Dictionary<Type, IService> Services => services;
    public bool AllServicesReady => requiredServices.Count == 0;

    private string LogPrefix => GetType().ToString() + "// ";
    [NonSerialized] public bool initialized;


    public virtual void Initialize(Mediator mediator = null)
    {
        if (initialized) return;
        ColorfulDebug.LogGreen($"//: Initialized service {this}");
        initialized = true;
    }

    public void HandleServiceRegistration(ServiceRegisterEvent @event)
    {
        Debug.Log($"{LogPrefix}Handling service registration: {@event.Service?.GetType().Name}");
        if (@event.Service == null) return;

        Type serviceType = @event.Service.GetType();
        bool foundMatch = false;

        foreach (var requiredType in requiredServices.ToArray())
        {
            if (requiredType.IsAssignableFrom(serviceType))
            {
                services[requiredType] = @event.Service;
                requiredServices.Remove(requiredType);
                Debug.Log($"{LogPrefix}Registered {requiredType.Name} service");
                foundMatch = true;
                break;
            }
        }
        if (foundMatch)
        {
            CheckAllServicesReady();
        }
    }

    private void CheckAllServicesReady()
    {
        Debug.Log($"=== {GetType()} Service Status ===");
        Debug.Log($"AllServicesReady: {AllServicesReady}");
        Debug.Log($"Required services remaining: {requiredServices.Count}");
        foreach (var service in services)
        {
            Debug.Log($"{LogPrefix}Has service: {service.Key.Name}");
        }
        if (requiredServices.Count == 0)
        {

            OnAllServicesReady();
            Initialize();
        }
    }

    protected virtual void OnAllServicesReady()
    {
        // Override in derived classes
    }

    protected T GetService<T>() where T : IService
    {
        if (services.TryGetValue(typeof(T), out IService service))
        {
            return (T)service;
        }
        return default;
    }
}

#endregion


public interface IStateListener
{
    void OnStateChanged(Game.State state);
}

#region EventBus

public class EventBus
{
    private readonly Dictionary<Type, List<Action<IEvent>>> _eventSubscriptions = new();


    public int GetSubscriptionCount<T>() where T : IEvent
    {
        Type eventType = typeof(T);
        if (_eventSubscriptions.TryGetValue(eventType, out var handlers))
        {
            return handlers.Count;
        }
        return 0;
    }

    public void LogSubscriptions()
    {
        Debug.Log("=== EventBus Subscriptions ===");
        foreach (var kvp in _eventSubscriptions)
        {
            Debug.Log($"{kvp.Key.Name}: {kvp.Value.Count} handlers");
        }
    }
    public void Subscribe<T>(Action<T> handler) where T : IEvent
    {
        Type eventType = typeof(T);
        if (!_eventSubscriptions.ContainsKey(eventType))
        {
            _eventSubscriptions[eventType] = new List<Action<IEvent>>();
        }

        _eventSubscriptions[eventType].Add((e) => handler((T)e));
        Debug.Log($"Subscribed to {eventType.Name}");
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
        // Debug.Log($"Publishing {eventType.Name}");

        if (_eventSubscriptions.TryGetValue(eventType, out var handlers))
        {
            // Debug.Log($"Found {handlers.Count} handlers for {eventType.Name}");
            foreach (var handler in handlers.ToArray())
            {
                handler?.Invoke(eventData);
            }
        }
        else
        {
            // Debug.LogWarning($"No handlers found for {eventType.Name}");
            // LogSubscriptions();
        }
    }
}

#endregion


#region Mediator
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
    public static event Action<string> OnSceneLoadStarted;
    public static event Action<string> OnSceneLoadComplete;


    public event Action OnInitializationCompleted;

    private Game.State _currentState;
    public Game.State CurrentState => _currentState;
    public EventBus GlobalEventBus { get; private set; }
    public GameSettings Settings { get; private set; } = new();

    #region Initialize

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        Instance = this;

        GlobalEventBus = new EventBus();
        SubscribeToEvents();
        InitializeStateDictionary();

        OnSceneLoadStarted += _ => ServiceCleanup();
    }

    private void SubscribeToEvents()
    {
        GlobalEventBus.Subscribe<DebugLogErrorEvent>(DebugLogErrorEventHandler);
        GlobalEventBus.Subscribe<LoadSceneEvent>(@LoadSceneEventHandler);
    }

    private void DebugLogErrorEventHandler(DebugLogErrorEvent @event)
    {
        ColorfulDebug.LogError(@event.Message);
    }

    private void LoadSceneEventHandler(LoadSceneEvent @event)
    {
        LoadScene(@event.SceneName, @event.TargetState);
    }

    private void InitializeStateDictionary()
    {
        foreach (Game.State state in Enum.GetValues(typeof(Game.State)))
        {
            _stateChangeCallbacks[state] = new List<Action<Game.State>>();
        }
    }

    public void RegisterInitializable(IInitializable initializable, bool immediate = false)
    {
        if (immediate)
        {
            initializable.Initialize(this);
            return;
        }
        if (!_initializables.Contains(initializable))
        {
            _initializables.Add(initializable);
        }
    }

    public void InitializeAll()
    {
        Debug.Log($"Initializing {_initializables.Count} services");
        foreach (var initializable in _initializables.ToArray()) // Copy to avoid modification
        {
            initializable.Initialize(this);
        }

        foreach (var initializable in _initializables)
        {
            if (initializable is MonoService monoService)
            {
                GlobalEventBus.Subscribe<ServiceRegisterEvent>(monoService.HandleServiceRegistration);

                CheckPreRegisteredServices(monoService);
            }
        }

        _initializables.Clear();
        OnInitializationCompleted?.Invoke();
    }


    private void CheckPreRegisteredServices(MonoService monoService)
    {
        foreach (var requiredType in monoService.requiredServices.ToArray())
        {
            if (_services.Values.Any(service => requiredType.IsAssignableFrom(service.GetType())))
            {
                var serviceInstance = _services.Values.First(s => requiredType.IsAssignableFrom(s.GetType()));
                monoService.HandleServiceRegistration(new ServiceRegisterEvent(serviceInstance as MonoService));
            }
        }
    }

    #endregion

    #region State

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

    #endregion

    #region Service

    public void RegisterService<T>(T service) where T : class
    {
        if (_services.ContainsKey(typeof(T)))
        {
            ColorfulDebug.LogWarning($"Service {typeof(T)} already registered!");
            return;
        }

        ColorfulDebug.LogGreen($"Registered service: {service}");
        _services[typeof(T)] = service;

        if (service is MonoService monoService && !monoService.initialized)
        {
            if (monoService.AllServicesReady)
            {
                monoService.Initialize(this);
            }
            else
            {
                GlobalEventBus.Subscribe<ServiceRegisterEvent>(monoService.HandleServiceRegistration);
                CheckPreRegisteredServices(monoService);

            }
        }
        GlobalEventBus.Publish(new ServiceRegisterEvent(service as MonoService));
    }

    public void UnregisterService<T>(T service) where T : class
    {
        ColorfulDebug.LogGreen($"Unregistered service: {service}");

        Type serviceType = typeof(T);
        _services.Remove(serviceType);
    }

    public T GetService<T>() where T : class
    {
        if (_services.TryGetValue(typeof(T), out object service))
        {
            return service as T;
        }

        ColorfulDebug.LogError($"Service of type {typeof(T)} not registered!");
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

    public void CheckActiveServices()
    {
        foreach (var keyValuePair in _services)
        {
            print(keyValuePair.Key);
        }
    }

    private void ServiceCleanup()
    {
        foreach (var keyValuePair in _services)
        {
            if (keyValuePair.Value == null)
            {
                _services.Remove(keyValuePair.Key);
            }
        }
    }

    public void RegisterPersistent<T>(T obj) where T : UnityEngine.Object
    {
        DontDestroyOnLoad(obj);
    }

    #endregion

    #region Scene Loading

    public void LoadScene(string sceneName, Game.State targetState, bool useTransitionScreen = true)
    {
        Debug.Log($"loading scene: {sceneName}");
        SetState("Loading");
        if (useTransitionScreen)
        {
            GetService<TransitionScreen>().StartTransition(() => StartCoroutine(LoadSceneAsync(sceneName, targetState)));
        }
        else
        {
            StartCoroutine(LoadSceneAsync(sceneName, targetState));
        }
    }

    private IEnumerator LoadSceneAsync(string sceneName, Game.State targetState)
    {
        OnSceneLoadStarted?.Invoke(sceneName);
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

        OnSceneLoadComplete?.Invoke(sceneName);

        if (GetService<TransitionScreen>() != null)
        {
            GetService<TransitionScreen>().EndTransition();
        }

        SetState(targetState);
    }

    #endregion
    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }

        OnStateChanged = null;
        OnLoadProgress = null;
        OnSceneLoadStarted = null;
        OnSceneLoadComplete = null;
        OnInitializationCompleted = null;


        foreach (var callbackList in _stateChangeCallbacks.Values)
        {
            callbackList.Clear();
        }
        _stateChangeCallbacks.Clear();
        _initializables.Clear();
        _services.Clear();
        GlobalEventBus = null;
    }
}

#endregion