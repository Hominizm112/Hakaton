
using System;
using System.Collections.Generic;

public class EventBus
{
    private readonly Dictionary<Type, List<Subscription>> _eventSubscriptions = new();

    public class Subscription : IDisposable
    {
        public Type EventType { get; }
        public Action<IEvent> Handler { get; }
        private EventBus _eventBus;

        public Subscription(Type eventType, Action<IEvent> handler, EventBus eventBus)
        {
            EventType = eventType;
            Handler = handler;
            _eventBus = eventBus;
        }

        public void Dispose()
        {
            _eventBus.Unsubscribe(this);
        }
    }

    public int GetSubscriptionCount<T>() where T : IEvent
    {
        Type eventType = typeof(T);
        if (_eventSubscriptions.TryGetValue(eventType, out var handlers))
        {
            return handlers.Count;
        }
        return 0;
    }

    public Subscription Subscribe<T>(Action<T> handler) where T : IEvent
    {
        Type eventType = typeof(T);
        if (!_eventSubscriptions.ContainsKey(eventType))
        {
            _eventSubscriptions[eventType] = new List<Subscription>();
        }

        Action<IEvent> wrappedHandler = (e) => handler((T)e);

        var subscription = new Subscription(eventType, wrappedHandler, this);
        _eventSubscriptions[eventType].Add(subscription);

        return subscription;
    }


    private void Unsubscribe(Subscription subscription)
    {
        if (_eventSubscriptions.TryGetValue(subscription.EventType, out var subscriptions))
        {
            subscriptions.Remove(subscription);
        }
    }

    public void Unsubscribe<T>(Action<T> handler) where T : IEvent
    {
        Type eventType = typeof(T);
        if (_eventSubscriptions.TryGetValue(eventType, out var subscriptions))
        {
            subscriptions.RemoveAll(sub =>
                sub.Handler.Target == handler.Target &&
                sub.Handler.Method == handler.Method);
        }
    }

    public void Publish<T>(T eventData) where T : IEvent
    {
        Type eventType = typeof(T);

        if (_eventSubscriptions.TryGetValue(eventType, out var subscriptions))
        {
            foreach (var subscription in subscriptions.ToArray())
            {
                subscription.Handler?.Invoke(eventData);
            }
        }
    }
}
