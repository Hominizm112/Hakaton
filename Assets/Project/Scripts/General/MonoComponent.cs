using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class MonoComponent : InjectableBehaviour, IDisposable
{
    [Inject] protected EventBus _eventBus;
    [Inject] protected Mediator _mediator;
    private List<EventBus.Subscription> _events = new();

    public virtual void SubscribeToEvent<TEvent>(Action<TEvent> handler) where TEvent : IEvent
    {
        _events.Add(_eventBus.Subscribe(handler));

    }

    public override void OnConstruct()
    {
        base.OnConstruct();
    }

    private void OnDestroy()
    {
        foreach (var e in _events)
        {
            e.Dispose();
        }
        _events.Clear();
        Dispose();
    }
    public virtual void Dispose() { }
}
