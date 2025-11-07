using System;
using System.Collections.Generic;
using Zenject;

public abstract class EventListener : IEventListener
{
    [Inject] protected EventBus _eventBus;
    private List<EventBus.Subscription> _events = new();

    public virtual void SubscribeToEvent<TEvent>(Action<TEvent> handler) where TEvent : IEvent
    {
        _events.Add(_eventBus.Subscribe(handler));

    }

    public virtual void Dispose()
    {
        foreach (var e in _events)
        {
            e.Dispose();
        }
        _events.Clear();
    }

}
