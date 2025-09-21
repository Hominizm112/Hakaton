using UnityEngine;
using MyGame.Enums;
public class OpenTradeWindowEvent : IEvent
{
    public string Ticker { get; }
    public float Price { get; }
    public TradeType TradeType { get; }

    public OpenTradeWindowEvent(string ticker, float price, TradeType tradeType)
    {
        Ticker = ticker;
        Price = price;
        TradeType = tradeType;
    }
}