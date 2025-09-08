
using System;
using UnityEngine;
using UnityEngine.InputSystem.Interactions;

#region Events
public interface IEvent { }

public class LoadSceneEvent : IEvent
{
    public string SceneName;
    public Game.State TargetState;
    public LoadSceneEvent(string sceneName, Game.State targetState)
    {
        SceneName = sceneName;
        TargetState = targetState;
    }
}
public class DebugLogErrorEvent : IEvent
{
    public string Message;

    public DebugLogErrorEvent(string message)
    {
        Message = message;
    }
}

#endregion

public static class EventFactory
{
    public static T CreateEvent<T>(params object[] parameters) where T : IEvent
    {
        Type eventType = typeof(T);
        if (eventType == typeof(LoadSceneEvent))
        {
            if (parameters.Length >= 2 && parameters[0] is string sceneName && parameters[1] is Game.State targetState)
            {
                return (T)(IEvent)new LoadSceneEvent(sceneName, targetState);
            }
            else
            {
                Mediator.Instance.GlobalEventBus.Publish(new DebugLogErrorEvent("Invalid parameters for LoadSceneEvent"));
                return default(T);
            }
        }
        else if (eventType == typeof(DebugLogErrorEvent))
        {
            if (parameters.Length >= 1 && parameters[0] is string message)
            {
                return (T)(IEvent)new DebugLogErrorEvent(message);
            }
        }

        Mediator.Instance.GlobalEventBus.Publish(new DebugLogErrorEvent($"Unknown event type: {eventType}"));
        return default(T);


    }

    public static void CreateAndPublish<T>(Mediator mediator, params object[] parameters) where T : IEvent
    {
        Debug.Log($"Creating event: {typeof(T).Name}");

        T @event = CreateEvent<T>(parameters);

        if (mediator == null)
        {
            mediator = Mediator.Instance;
        }

        if (mediator == null)
        {
            Debug.LogError("Mediator instance is null! Cannot publish event.");
            return;
        }

        if (@event == null)
        {
            Debug.LogError("Failed to create event!");
            return;
        }

        Debug.Log($"Publishing event: {@event.GetType().Name}");
        mediator.GlobalEventBus.Publish(@event);
    }
    public enum EventType
    {
        None,
        LoadScene
    }
}