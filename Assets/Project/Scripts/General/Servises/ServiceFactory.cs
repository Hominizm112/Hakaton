using UnityEngine;
using Zenject;

public class ServiceFactory : IServiceFactory, IPersistentServiceFactory, IUIFactory
{
    private readonly DiContainer _container;

    public ServiceFactory(DiContainer container)
    {
        _container = container;
    }
    public T CreateService<T>() where T : Component
    {
        string prefabPath = $"Prefabs/{typeof(T).Name}";
        return CreateService<T>(prefabPath);
    }

    public T CreateService<T>(string prefabPath) where T : Component
    {
        var prefab = Resources.Load<GameObject>(prefabPath);
        if (prefab == null)
        {
            Debug.LogError($"Prefab not found at path: {prefabPath}");
            return null;
        }

        return _container.InstantiatePrefabForComponent<T>(prefab);
    }

    public GameObject CreateGameObject(string prefabPath)
    {
        var prefab = Resources.Load<GameObject>(prefabPath);
        if (prefab == null)
        {
            Debug.LogError($"Prefab not found at path: {prefabPath}");
            return null;
        }

        return _container.InstantiatePrefab(prefab);
    }

    public T CreatePersistentService<T>() where T : Component
    {
        T service = CreateService<T>();
        if (service != null)
        {
            UnityEngine.Object.DontDestroyOnLoad(service.gameObject);
        }
        return service;
    }

    public T CreateUIElement<T>() where T : Component
    {
        return CreateService<T>($"UI/{typeof(T).Name}");
    }
}
