using System;
using GameCore.UI;
using MyGame.Enums;
using UnityEngine;

public interface IEvent { }



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

public class SceneStartLoadEvent : IEvent
{
    public string SceneName { get; }
    public GameService.State TargetState { get; }
    public SceneStartLoadEvent(string sceneName, GameService.State targetState)
    {
        SceneName = sceneName;
        TargetState = targetState;
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



#region Save/Load Events

public class LoadDataEvent : IEvent
{
    public SaveManager Sender { get; }

    public LoadDataEvent(SaveManager sender)
    {
        Sender = sender;
    }
}

public class StartSaveDataEvent : IEvent
{
    public SaveManager Sender { get; }

    public StartSaveDataEvent(SaveManager sender)
    {
        Sender = sender;
    }
}

#endregion

#region  Drag

public interface IInputEvent : IEvent { }

public class DragEndedEvent : IInputEvent
{
    public object sender;

    public DragEndedEvent(object sender)
    {
        this.sender = sender;
    }
}


public class DragStartedEvent : IInputEvent
{
    public object sender;

    public DragStartedEvent(object sender)
    {
        this.sender = sender;
    }
}

#endregion




#region Screens Events

public interface IScreenEvent : IEvent { }

public class ScreenOpenEvent : IScreenEvent
{
    public View ScreenView;

    public ScreenOpenEvent(View screenView)
    {
        ScreenView = screenView;
    }
}

public class ScreenCloseEvent : IScreenEvent
{
    public View ScreenView;

    public ScreenCloseEvent(View screenView)
    {
        ScreenView = screenView;
    }
}


#endregion

#region TransitionScreen

public class LoadingScreenDropEvent : IEvent
{

}

public class LoadingScreenLiftEvent : IEvent
{

}

#endregion