using UnityEngine;
using System.Collections.Generic;
using MyGame.Enums;

public class TradeRequestEvent : IEvent// событие со всей инф для покупки/продажи
{
    public string Ticker { get; set; }
    public float Price { get; set; }
    public int Quantity { get; set; }
    public TradeType TradeType { get; set; }
    public TradeRequestEvent(TradeType tradeType, string ticker, float price, int quantity)
    {
        TradeType = tradeType;
        Ticker = ticker;
        Price = price;
        Quantity = quantity;
    }

}
namespace MyGame.Enums
{
    public enum TradeType
    {
        Buy,
        Sell
    }
}
