using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using MyGame.Enums;
public class QuickTradeButton : MonoBehaviour
{
    public string Ticker;
    public float Price;
    public TradeType TradeType;
    public UnityEvent OnClickAction;

    public void OnClick()
    {
        Mediator.Instance.GlobalEventBus.Publish(new OpenTradeWindowEvent(Ticker, Price, TradeType));
    }
}