using UnityEngine;
using UnityEngine.SceneManagement;

public abstract class UnitySingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;
    private static readonly object _lock = new object();
    private static bool _isPersistent = false;

    public static T Instance
    {
        get
        {
            lock (_lock)
            {
                if (_instance == null)
                {
                    _instance = FindFirstObjectByType<T>();

                    if (_instance == null)
                    {
                        // Only create new instance if not quitting
                        if (!Application.isPlaying)
                            return null;

                        var singletonObject = new GameObject();
                        _instance = singletonObject.AddComponent<T>();
                        singletonObject.name = $"{typeof(T)} (Singleton)";

                        Debug.Log($"[Singleton] Created new {typeof(T)} instance");
                    }
                }
                return _instance;
            }
        }
    }

    protected virtual void Awake()
    {
        if (_instance == null)
        {
            _instance = this as T;
            if (_isPersistent && Application.isPlaying)
            {
                DontDestroyOnLoad(gameObject);
            }
        }
        else if (_instance != this)
        {
            Debug.LogWarning($"[Singleton] Destroying duplicate {typeof(T)} instance");
            Destroy(gameObject);
        }
    }

    public void SetPersistent(bool persistent)
    {
        _isPersistent = persistent;
        if (persistent && Application.isPlaying)
        {
            DontDestroyOnLoad(gameObject);
        }
    }

    protected virtual void OnDestroy()
    {
        // Only null the instance if this was the active instance
        if (_instance == this)
        {
            _instance = null;
        }
    }
}