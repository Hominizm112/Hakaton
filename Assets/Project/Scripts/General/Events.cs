using System;
using UnityEngine;

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
public class CurrencyChangedEvent : IEvent
{
    public int NewAmount;
    public CurrencyChangedEvent(int newAmount)
    {
        NewAmount = newAmount;
    }
}


