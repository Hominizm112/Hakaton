using UnityEngine;
using MyGame.Enums;
using TMPro;
using UnityEngine.UI;
using System;
using System.Collections.Generic;


public class PortfolioView : MonoBehaviour
{
     private TMP_Text cashBalanceText;
     private TMP_Text totalValueText;
     private TMP_Text stocksValueText;
     private TMP_Text bondsValueText;
   //[SerializeField] private TMP_Text _totalGainText;
    //[SerializeField] private TMP_Text _dayGainText;
    //[SerializeField] private TMP_Text _totalGainTextPercent;
    //[SerializeField] private TMP_Text _dayGainTextPercent;
    private TMP_Text countStocks;
    private TMP_Text countBonds;

    private Button _addCashButton;
    private Button _checkOtherStocksButton;
    private Button _checkOtherBondsButton;
    private Button _analyticsButton;


    //ссылки на кнопки
    private readonly Dictionary<Ticker, AssetItemView> _activeAssetViews = new Dictionary<Ticker, AssetItemView>();
    [SerializeField] private RectTransform _listContentParent; //Контейнер для строк
    [SerializeField] private AssetItemView _assetItemViewPrefab;

    //public event Action OnActiveInfoClicked;
    public event Action<Ticker, TradeType> OnTradeActiveClicked;
    public event Action<int> OnAddCashClicked;
    public event Action OnCheckOtherStocksClicked;// посмотреть списки других активов
    public event Action OnCheckOtherBondsClicked;
    public event Action OnGetAnalyticsClicked;
    

    private void Awake()
    {
        //_mediator = Mediator.Instance;
        //_mediator.OnInitializationCompleted += () => _mediator.GetService<PortfolioPresenter>().InitializeView(this);
       // _activeInfoButton.onClick.AddListener(() => OnActiveInfoClicked.Invoke());
        _checkOtherStocksButton.onClick.AddListener(() => OnCheckOtherStocksClicked.Invoke());
        _checkOtherBondsButton.onClick.AddListener(() => OnCheckOtherBondsClicked.Invoke());
        _analyticsButton.onClick.AddListener(() => OnGetAnalyticsClicked.Invoke());

        _addCashButton.onClick.AddListener(() =>
        {//заменить парсинг

            //int amount;
            //OnAddCashClicked.Invoke(amount);
            
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
    public void UpdatePortfolioView(PortfolioSummary summary)
    {
        UpdateCashDisplay(summary.CashBalance);
        //totalValueText.text = summary.TotalValue.ToString();
        //bondsValueText.text = summary.BondsValue.ToString();
        //stocksValueText.text = summary.StocksValue.ToString();
        //countStocks.text = summary.CountStocks.ToString();
        //countBonds.text = summary.CountBonds.ToString();

        foreach (var kvp in _activeAssetViews)
        {
            Ticker ticker = kvp.Key;
            if (summary.MyActives.TryGetValue(ticker, out IActiv activeAsset))
            {
                int newPrice = activeAsset.CurrentValue;
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
        if (cashBalanceText == null)
        {
            Debug.LogError("Не существует текстового поля для цены");
            return;
        }
        cashBalanceText.text = $"Баланс: {cash} UO";
    }
//удаление со сцены


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
