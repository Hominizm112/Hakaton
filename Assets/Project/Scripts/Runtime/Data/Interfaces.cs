
using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

public interface IServiceFactory
{
    T CreateService<T>() where T : Component;
}

public interface IPersistentServiceFactory
{
    T CreatePersistentService<T>() where T : Component;
}

public interface IUIFactory
{
    T CreateUIElement<T>() where T : Component;
}

public interface IInjectable
{
    bool Injected { get; }
    Action OnInjected { get; set; }
    void MarkInjected();
}

public interface IItem { }

public interface IEventListener { }

public abstract class InjectableBehaviour : MonoBehaviour, IInjectable, IDisposable
{
    public bool Injected { get; private set; }
    public Action OnInjected { get; set; }

    private void Awake()
    {
        if (!Injected)
            BindToContainer();
    }

    [Inject]
    public void Construct()
    {
        OnConstruct();
        MarkInjected();
    }

    public virtual void OnConstruct() { }

    private void BindToContainer()
    {
        var sceneContext = FindFirstObjectByType<SceneContext>();
        if (sceneContext == null) return;

        var container = sceneContext.Container;
        if (container == null) return;

        var bindAttr = GetType().GetCustomAttribute<BindAttribute>();

        if (bindAttr != null && bindAttr.BindType != null)
        {
            container.Bind(bindAttr.BindType).FromInstance(this).AsCached();
        }
        else
        {
            container.Bind(GetType()).FromInstance(this).AsCached();
            foreach (var interfaceType in GetType().GetInterfaces())
            {
                container.Bind(interfaceType).FromInstance(this).AsCached();
            }
        }

        container.Inject(this);

    }



    public void MarkInjected()
    {
        if (Injected) return;
        Injected = true;
        OnInjected?.Invoke();
        OnInjectedHandler();
    }

    protected virtual void OnInjectedHandler() { }

    public virtual void Dispose() { }
}

[AttributeUsage(AttributeTargets.Class)]
public class BindAttribute : Attribute
{
    public Type BindType { get; }
    public BindAttribute(Type bindType = null) => BindType = bindType;
}


public interface IViewOpener
{
    public abstract void OpenScreen();
}

public interface IStateListener
{
    void OnStateChanged(Game.State state);
}


