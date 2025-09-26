using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using MyGame.Enums;
public class QuickTradeButton : MonoBehaviour//управление кнопкой быстрой торговли
{
    public Ticker Ticker;
    public float Price;
    public int Quantity;
    public TradeType TradeType;
    public UnityEvent OnClickAction;
    public void OnClick()
    {
        Mediator.Instance.GlobalEventBus.Publish(new OpenTradeWindowEvent(Ticker, Price, TradeType, Quantity));
    }
}