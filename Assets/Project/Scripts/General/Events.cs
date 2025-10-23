using System;
using MyGame.Enums;
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



#region Trade Events
public class TradeRequestEvent : IEvent// событие со всей инф для покупки/продажи
{
    public Ticker Ticker { get; set; }
    public float Price { get; set; }
    public int Quantity { get; set; }
    public TradeType TradeType { get; set; }
    public TradeRequestEvent(TradeType tradeType, Ticker ticker, float price, int quantity)
    {
        TradeType = tradeType;
        Ticker = ticker;
        Price = price;
        Quantity = quantity;
    }

}


public class OpenTradeWindowEvent : IEvent//событие открытия окна торговли
{
    public Ticker Ticker { get; }
    public float Price { get; }
    public TradeType TradeType { get; }
    public int Quantity { get; }

}

public class AssetListChangedEvent : IEvent
{
    public readonly Ticker NewAssetTicker;
    public AssetListChangedEvent(Ticker newAssetTicker)
    {
        NewAssetTicker = newAssetTicker;
    }
}

#endregion

#region Scene Management Related Events

public class SceneLoadedEvent : IEvent
{
    public readonly string SceneName;
    public SceneLoadedEvent(string sceneName)
    {
        SceneName = sceneName;
    }
}

public class SceneUnloadEvent : IEvent
{
    public readonly string SceneName;

    public SceneUnloadEvent(string sceneName)
    {
        SceneName = sceneName;
    }
}

#endregion

#region In-game Time Events

public interface TimeEvent : IEvent { }

public class TimeTrackStartEvent : TimeEvent
{
    public readonly int Minutes;
    public readonly int Seconds;

    public TimeTrackStartEvent(int minutes = 0, int seconds = 0)
    {
        Minutes = minutes;
        Seconds = seconds;
    }


}

public class TimeTrackStopEvent : TimeEvent { }

public class TimeTrackCompletedEvent : TimeEvent { }

#endregion


#region Stall Related Events

public class CustomerAtStallEvent : IEvent { }

#endregion

#region Save/Load Events

public class LoadDataEvent : IEvent { }

public class TeaRemovedFromSelectionEvent : IEvent { }

#endregion