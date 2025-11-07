using System;
using Zenject;
public interface IService { }

public class Service : IService, IInitializable, IDisposable
{
    [Inject] protected EventBus GlobalEventBus;


    public virtual void Initialize()
    {
    }


    public virtual void SubscribeToEvent<TEvent>(Action<TEvent> handler) where TEvent : IEvent
    {
        GlobalEventBus.Subscribe(handler);
    }

    public void OnDestroy()
    {
        Dispose();
    }

    public virtual void Dispose()
    {
    }
}
