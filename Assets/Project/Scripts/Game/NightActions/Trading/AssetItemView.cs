using UnityEngine;
using System;
using MyGame.Enums;
using UnityEngine.UI;
using TMPro;

public class AssetItemView : MonoBehaviour//отображение одной кнопки актива
{
    [SerializeField] private Button _buyActiveButton;
    [SerializeField] private Button _sellActiveButton;
    [SerializeField] private Button _infoActiveButton; 
    [SerializeField] private TMP_Text _tickerLabel; 

    private Ticker _assetTicker;
    public event Action<Ticker, TradeType> OnOpenTradeRequested;
    public event Action<Ticker> OnAssetDetailsClicked;//событие клика на кнопку актива(получение инфо)

    public void Initialize(Ticker ticker, string displayName)
    {
        _assetTicker = ticker;
        _tickerLabel.text = displayName;
        _buyActiveButton.onClick.AddListener(() =>
       {
           //if (ticker == Ticker.None)
           //{
           //_mediator.GlobalEventBus.Publish<DebugLogErrorEvent>(new("Не существует актива с данным тикером"));//убрать из view в presenter
           //}

           OnOpenTradeRequested?.Invoke(ticker, TradeType.Buy);

       });

        _sellActiveButton.onClick.AddListener(() =>
       {
           //if (ticker == Ticker.None)
           //{
           //_mediator.GlobalEventBus.Publish<DebugLogErrorEvent>(new("Не существует актива с данным тикером"));//убрать из view в presenter
           //}

           OnOpenTradeRequested?.Invoke(ticker, TradeType.Sell);

       });

       _infoActiveButton.onClick.AddListener(() => OnAssetDetailsClicked?.Invoke(ticker));

    }

    private void OnDestroy()
    {
        _buyActiveButton.onClick.RemoveAllListeners();
        _sellActiveButton.onClick.RemoveAllListeners();
        _infoActiveButton.onClick.RemoveAllListeners();
    }
          
          
}
