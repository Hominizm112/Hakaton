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