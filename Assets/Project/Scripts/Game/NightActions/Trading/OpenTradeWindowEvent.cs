using UnityEngine;
using MyGame.Enums;
public class OpenTradeWindowEvent : IEvent//событие открытия окна торговли
{
    public string Ticker { get; }
    public float Price { get; }
    public TradeType TradeType { get; }
    public int Quantity{ get; }

    public OpenTradeWindowEvent(string ticker, float price, TradeType tradeType, int quantity)
    {
        Ticker = ticker;
        Price = price;
        TradeType = tradeType;
        Quantity = quantity;
    }
}