using UnityEngine;
using MyGame.Enums;
using TMPro;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using Unity.Collections;


public class PortfolioView : MonoBehaviour
{
    [SerializeField] private InputField _addCashInputField;//количество cash для пополнения баланса
    [SerializeField] private TMP_Text _cashBalanceText;
    [SerializeField] private TMP_Text _totalValueText;
    [SerializeField] private TMP_Text _stocksValueText;
    [SerializeField] private TMP_Text _bondsValueText;
   //[SerializeField] private TMP_Text _totalGainText;
    //[SerializeField] private TMP_Text _dayGainText;
    //[SerializeField] private TMP_Text _totalGainTextPercent;
    //[SerializeField] private TMP_Text _dayGainTextPercent;
    [SerializeField] private TMP_Text _countStocks;
    [SerializeField] private TMP_Text _countBonds;

    [SerializeField] private Button _addCashButton;
    [SerializeField] private Button _checkOtherStocksButton;
    [SerializeField] private Button _checkOtherBondsButton;
    [SerializeField] private Button _analyticsButton;


    //ссылки на кнопки
    private readonly Dictionary<Ticker, AssetItemView> _activeAssetViews = new Dictionary<Ticker, AssetItemView>();
    [SerializeField] private RectTransform _listContentParent; //Контейнер для строк
    [SerializeField] private AssetItemView _assetItemViewPrefab;
    private Ticker _selectedTicker;
    //public event Action OnActiveInfoClicked;
    public event Action<Ticker, TradeType> OnTradeActiveClicked;
    public event Action<int> OnAddCashClicked;
    public event Action OnCheckOtherStocksClicked;// посмотреть списки других активов
    public event Action OnCheckOtherBondsClicked;
    public event Action OnGetAnalyticsClicked;
    //локальное событие
    public event Action<string> OnInputError;

    private void Awake()
    {
        _checkOtherStocksButton.onClick.AddListener(() => OnCheckOtherStocksClicked.Invoke());
        _checkOtherBondsButton.onClick.AddListener(() => OnCheckOtherBondsClicked.Invoke());
        _analyticsButton.onClick.AddListener(() => OnGetAnalyticsClicked.Invoke());
        _addCashButton.onClick.AddListener(() =>
        {
            int amount;

            if (int.TryParse(_addCashInputField.text, out amount))
            {
                OnAddCashClicked.Invoke(amount);
            }
            else
            {
                OnInputError?.Invoke("Некорректный ввод значения");
            }

        });
    }


//регитсрация кнопки в контейнере всех кнопок
    public void RegisterAssetButton(Ticker ticker, AssetItemView newButtonView)
    {
        _activeAssetViews[ticker] = newButtonView;
    }

    public AssetItemView CreateAssetItemView()//string displayName??
    {
        AssetItemView newItem = Instantiate(_assetItemViewPrefab, _listContentParent);
        return newItem;
    }
//удаление кнопки со сцены    
    public void DeactivateAssetButton(Ticker ticker)
    {
        if (_activeAssetViews.TryGetValue(ticker, out AssetItemView viewToRemove))
        {
            Destroy(viewToRemove.gameObject);
            _activeAssetViews.Remove(ticker);
        }
        else
        {
            Debug.LogWarning($"Попытка удалить неактивную UI-кнопку для тикера {ticker}.");
            return;
        }
    }
    public void UpdatePortfolioView(PortfolioSummary summary)//<--заменить на int,обновление информации на UI
    {
        UpdateCashDisplay(summary.CashBalance);
        _totalValueText.text = summary.TotalValue.ToString();
        _bondsValueText.text = summary.BondsValue.ToString();
        _stocksValueText.text = summary.StocksValue.ToString();
        _countStocks.text = summary.CountStocks.ToString();
        _countBonds.text = summary.CountBonds.ToString();

        foreach (var kvp in _activeAssetViews)
        {
            Ticker ticker = kvp.Key;
            if (summary.MyActives.TryGetValue(ticker, out SampleActiv activeAsset))
            {
                int newPrice = activeAsset.CurrentValue; // <--int
                int newQuantity = activeAsset.Quantity;
                UpdateAssetButton(ticker, newPrice, newQuantity);
            }
            else
            {

            }
        }

    }
    //обновление одной кнопки актива
    public void UpdateAssetButton(Ticker newTicker,int newPrice, int newQuantity)
    {
        if (_activeAssetViews.TryGetValue(newTicker, out AssetItemView viewToUpdate))
            {
                viewToUpdate.UpdateDisplay(
                    price: newPrice,
                    quantity: newQuantity,
                    true
                );
            }

    }

    public void UpdateCashDisplay(int cash)
    {
        if (_cashBalanceText == null)
        {
            Debug.LogError("Не существует текстового поля для цены");
            return;
        }
        _cashBalanceText.text = $"Баланс: {cash} UO";
    }
//удаление со сцены

    //public void OnSellButtonClickHandler()
    //{
    //IActiv asset = GetSelectedAsset();
    //int quantity = int.Parse(_quantityInputField.text);
    //  OnSellActiveClicked?.Invoke(TradeType.Sell, asset, quantity);
    //}

    public void SnowPortfolioReport()
    {
        ColorfulDebug.LogGreen("PortfolioReport is generate:");
    }


    public void OpenBondScreenInfo()
    {

        //просто инфа, отсюда нельзя ничего купить

    }


    public void OpenStockScreenInfo()
    {
        //просто инфа, отсюда нельзя ничего купить
    }

    public void OpenOtherStockScreen()
    {
        ColorfulDebug.LogYellow("This is stocks");

    }


    public void OpenOtherBondScreen()
    {

       ColorfulDebug.LogYellow("This is bonds");

    }

    private void OnDestroy()
    {
        _addCashButton.onClick.RemoveAllListeners();
        _checkOtherStocksButton.onClick.RemoveAllListeners();
        _checkOtherBondsButton.onClick.RemoveAllListeners();
        _analyticsButton.onClick.RemoveAllListeners();

    }
}
