using UnityEngine;
using MyGame.Enums;
using TMPro;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using UnityEngine.Localization.Components;


public class PortfolioView : BaseApp
{
    //[SerializeField] private LocalizeStringEvent localizeStringEvent;
    [SerializeField] private TMP_Text cashBalanceText;
    [SerializeField] private TMP_Text totalValueText;
    [SerializeField] private TMP_Text stocksValueText;
    [SerializeField] private TMP_Text bondsValueText;
    //[SerializeField] private TMP_Text _totalGainText;
    //[SerializeField] private TMP_Text _dayGainText;
    //[SerializeField] private TMP_Text _totalGainTextPercent;
    //[SerializeField] private TMP_Text _dayGainTextPercent;
    [SerializeField] private TMP_Text countStocks;
    [SerializeField] private TMP_Text countBonds;
    [SerializeField] private Button _addCashButton;
    [SerializeField] private Button _checkOtherStocksButton;
    [SerializeField] private Button _checkOtherBondsButton;
    [SerializeField] private Button _analyticsButton;

    //ссылки на кнопки
    private readonly Dictionary<Ticker, AssetItemView> _activeAssetViews = new Dictionary<Ticker, AssetItemView>();
    private RectTransform _listContentParent; //Контейнер для строк
    [SerializeField] private AssetItemView _assetItemViewPrefab;
    //public event Action OnActiveInfoClicked;
    public event Action<Ticker, TradeType> OnTradeActiveClicked;
    public event Action<int> OnAddCashClicked;
    public event Action OnCheckOtherStocksClicked;// посмотреть списки других активов
    public event Action OnCheckOtherBondsClicked;
    public event Action OnGetAnalyticsClicked;
    private const string CASH_BALANCE_SAVE_KEY = "CASH_BALANCE";


    protected void Awake()
    {
        // _activeInfoButton.onClick.AddListener(() => OnActiveInfoClicked.Invoke());
        // _mediator.OnInitializationCompleted += () => _mediator.GetService<PortfolioPresenter>().InitializeView(this);
        _checkOtherStocksButton.onClick.AddListener(() => OnCheckOtherStocksClicked.Invoke());
        _checkOtherBondsButton.onClick.AddListener(() => OnCheckOtherBondsClicked.Invoke());
        _analyticsButton.onClick.AddListener(() => OnGetAnalyticsClicked.Invoke());

        _addCashButton.onClick.AddListener(() =>
        {
            // int keypadInput = _appController.GetApp<KeypadApp>().KeypadInput;
            // if (keypadInput == 0)
            // {
            // return;
            // }
            // OnAddCashClicked.Invoke(keypadInput);

        });
    }

    public void CreatePortfolioView()
    {
        _addCashButton.gameObject.SetActive(true);
        _analyticsButton.gameObject.SetActive(true);
        _checkOtherStocksButton.gameObject.SetActive(true);
        _checkOtherBondsButton.gameObject.SetActive(true);
        //текст

    }

    //регитсрация кнопки в контейнере всех кнопок
    public void RegisterAssetButton(Ticker ticker, AssetItemView newButtonView)
    {
        if (!_activeAssetViews.ContainsKey(ticker))
        {
            _activeAssetViews.Add(ticker, newButtonView);
        }
    }

    public AssetItemView CreateAssetItemView()
    {
        return Instantiate(_assetItemViewPrefab, _listContentParent);
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
    #region Update
    public void UpdatePortfolioView(IPortfolioDisplayData data)
    {
        //UpdateCashDisplay(summary.CashBalance);
        totalValueText.text = data.TotalValue.ToString();
        bondsValueText.text = data.BondsValue.ToString();
        stocksValueText.text = data.StocksValue.ToString();
        countStocks.text = data.CountStocks.ToString();
        countBonds.text = data.CountBonds.ToString();
    }
    //обновление одной кнопки актива
    public void UpdateQuantityActiv(Ticker ticker, int newQuantity)
    {
        if (_activeAssetViews.TryGetValue(ticker, out AssetItemView viewToUpdate))
        {
            viewToUpdate.quantityLabel.text = newQuantity.ToString();

        }

    }
    public void UpdateAssetButton(Ticker newTicker, int newPrice, int newQuantity)
    {
        if (_activeAssetViews.TryGetValue(newTicker, out AssetItemView viewToUpdate))
        {
            //viewToUpdate.UpdateDisplay(
            // price: newPrice,

            // true
            //);
        }
    }

    public void UpdateCashDisplay(int cash)
    {
        if (cashBalanceText == null)
        {
            Debug.LogError("Не существует текстового поля для цены");
            return;
        }
        cashBalanceText.text = $"Баланс: {cash} UO";
    }
    //удаление со сцены


    #endregion
    public void OpenAddCashWindow()
    {

    }


    public void SnowPortfolioReport()
    {

    }


    public void OpenBondScreenInfo()
    {

    }


    public void OpenStockScreenInfo()
    {

    }

    public void OpenOtherStockScreen()
    {

    }


    public void OpenOtherBondScreen()
    {

    }

    private void OnDestroy()
    {
        _addCashButton.onClick.RemoveAllListeners();
        _checkOtherStocksButton.onClick.RemoveAllListeners();
        _checkOtherBondsButton.onClick.RemoveAllListeners();
        _analyticsButton.onClick.RemoveAllListeners();

    }
}
