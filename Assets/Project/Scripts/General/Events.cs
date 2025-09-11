using System;
using UnityEngine;

public interface IEvent { }

public class LoadSceneEvent : IEvent
{
    public string SceneName { get; }
    public Game.State TargetState { get; }
    public LoadSceneEvent(string sceneName, Game.State targetState)
    {
        SceneName = sceneName;
        TargetState = targetState;
    }
}
public class DebugLogErrorEvent : IEvent
{
    public string Message { get; }

    public DebugLogErrorEvent(string message)
    {
        Message = message;
    }
}
public class CurrencyChangedEvent : IEvent
{
    public int NewAmount { get; }
    public CurrencyChangedEvent(int newAmount)
    {
        NewAmount = newAmount;
    }
}


public class ServiceRegisterEvent : IEvent
{
    public IService Service { get; }

    public ServiceRegisterEvent(IService service)
    {
        Service = service;
    }
}
