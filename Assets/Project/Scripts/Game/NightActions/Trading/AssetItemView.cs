using UnityEngine;
using System;
using MyGame.Enums;
using UnityEngine.UI;
using TMPro;

public class AssetItemView : MonoBehaviour//отображение одной кнопки актива
{
    [SerializeField] private Button _buyActiveButton;
    [SerializeField] private Button _sellActiveButton;
    //[SerializeField] private Button _infoActiveButton; 
    [SerializeField] private TMP_Text _tickerLabel; 
    [SerializeField] private TMP_Text _priceLabel; // Добавлено для цены
    [SerializeField] private TMP_Text _quantityLabel; 

    private Ticker _assetTicker;
    public event Action<Ticker, TradeType> OnOpenTradeRequested;
    //public event Action<Ticker> OnAssetDetailsClicked;//событие клика на кнопку актива(получение инфо)

    public void Initialize(Ticker ticker, int price, int quantity, bool isPortfolioView)
    {
        _tickerLabel.text = ticker.ToString();
        //_assetTicker = ticker;
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

        // _infoActiveButton.onClick.AddListener(() => OnAssetDetailsClicked?.Invoke(ticker));

        _buyActiveButton.gameObject.SetActive(true); 
        
    }
    //обновление одной кнопки
    public void UpdateDisplay(int price, int quantity, bool isPortfolioView)///<--int
    {
        _priceLabel.text = $"Цена:{price}";
        bool canSell = isPortfolioView && quantity > 0;
        _quantityLabel.gameObject.SetActive(canSell);
        _quantityLabel.text = $"Кол-во: {quantity}";
        _sellActiveButton.gameObject.SetActive(canSell);//управление видимостью кнопки продать: видно только в портфолио
    
    }
    private void OnDestroy()
    {
        _buyActiveButton.onClick.RemoveAllListeners();
        _sellActiveButton.onClick.RemoveAllListeners();
        // _infoActiveButton.onClick.RemoveAllListeners();
    }
          
          
}
