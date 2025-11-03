using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MyGame.Enums;
using System;
using UnityEngine.Localization.Components;

public class TradingWindowView : MonoBehaviour
{
    [SerializeField] private Button _confirmButton;// кнопка подтвердить в окне торговли
    [SerializeField] private LocalizeStringEvent localizeStringEvent;
    [SerializeField] private TMP_Text _tickerText;
    //private TMP_Text _activeNameText;
    private Ticker _currentTicker;
    [SerializeField] private TMP_Text _currentPrice;
    [SerializeField] private TradeType _currentTradeType;
    public  event Action<TradeType,Ticker ,int > OnTradeConfirmed;

    public void Awake()
    {
        //_confirmButton.onClick.AddListener(HandleConfirmButtonClicked);
    }

    public void Initialize(TradeType tradeType, Ticker ticker, int quantity)
    {
        _confirmButton.onClick.AddListener(() =>
        {
            OnTradeConfirmed?.Invoke(tradeType,ticker,quantity);
        });
        
    }


    public void Show(TradeType type, Ticker ticker, int price)
    {
        _currentTradeType = type;
        _currentTicker = ticker;
        string action = (type == TradeType.Buy) ? "Покупка" : "Продажа";
        // string companyName = CompanyInfo.ActiveName.ContainsKey(ticker)
        //? CompanyInfo.ActiveName[ticker]
        // ticker.ToString();
        //_activeNameText.text = companyName;
        _tickerText.text = $"{ticker}";
        _currentPrice.text = $"Текущая цена: {price}";
        _confirmButton.gameObject.SetActive(true);
        this.gameObject.SetActive(true);
    }

    public void TradeWindowClose()
    {
        this.gameObject.SetActive(false);
        ColorfulDebug.LogGreen("Успешная операция закрытия окна торговли");


    }

}
