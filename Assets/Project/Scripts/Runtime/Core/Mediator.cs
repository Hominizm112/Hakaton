using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;



#region Mediator
public class Mediator : MonoBehaviour
{

    [Inject] public DiContainer _container;
    [Inject] public readonly List<IInitializable> _initializables;
    [Inject] public EventBus GlobalEventBus { get; private set; }
    [Inject] private TransitionScreen _transitionScreen;




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

    private readonly Dictionary<Game.State, List<Action<Game.State>>> _stateChangeCallbacks = new();
    private readonly Dictionary<Type, object> _services = new Dictionary<Type, object>();


    public event Action<Game.State> OnStateChanged;
    public static event Action<float> OnLoadProgress;
    public static event Action<string> OnSceneLoadStarted;
    public static event Action<string> OnSceneLoadComplete;


    public event Action OnInitializationCompleted;

    private Game.State _currentState;
    public Game.State CurrentState => _currentState;
    public GameSettings Settings { get; private set; } = new();

    #region Initialize

    [Inject]
    public void Construct()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        Instance = this;
        SubscribeToEvents();
        InitializeStateDictionary();
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
            initializable.Initialize();
            return;
        }
        if (!_initializables.Contains(initializable))
        {
            _initializables.Add(initializable);
        }
    }

    public void InitializeAll()
    {
        foreach (var initializable in _initializables.ToArray())
        {
            initializable.Initialize();
        }

        _initializables.Clear();
        OnInitializationCompleted?.Invoke();
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

    public void LogAllServices()
    {
        foreach (var service in _services)
        {
            Debug.Log(service.Value);
        }
    }

    #endregion

    #region Scene Loading //TODO переделать в отдельный класс

    public void LoadScene(string sceneName, Game.State targetState, bool useTransitionScreen = true)
    {
        print(SceneManager.GetActiveScene().name);
        print(sceneName);
        if (SceneManager.GetActiveScene().name == sceneName)
        {
            return;
        }
        SetState("Loading");
        if (useTransitionScreen)
        {
            // _transitionScreen.StartTransition(() => StartCoroutine(LoadSceneAsync(sceneName, targetState)));
        }
        else
        {
            StartCoroutine(LoadSceneAsync(sceneName, targetState));
        }
    }

    private IEnumerator LoadSceneAsync(string sceneName, Game.State targetState)
    {
        ColorfulDebug.LogBlue($"Started loading scene: {sceneName}");
        OnSceneLoadStarted?.Invoke(sceneName);

        GlobalEventBus.Publish<SceneUnloadEvent>(new(SceneManager.GetActiveScene().name));

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
        GlobalEventBus.Publish<SceneLoadedEvent>(new(sceneName));

        if (_transitionScreen != null)
        {
            // _transitionScreen.EndTransition();
        }

        ColorfulDebug.LogBlue($"Finished loading scene: {sceneName}");

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