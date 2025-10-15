using UnityEngine;
using MyGame.Enums;
using TMPro;
using UnityEngine.UI;
using System;
<<<<<<< Updated upstream
=======
using System.Collections.Generic;
>>>>>>> Stashed changes

public class TradingWindowView : MonoBehaviour
{
<<<<<<< Updated upstream
    [SerializeField] private Button _confirmButton;// кнопка подтвердить в окне торговли
    [SerializeField] private InputField _quantityInput;//ввод количества в окне торговли
    private Ticker _currentTicker;
    private TradeType _currentTradeType;
    [SerializeField] private TMP_Text _currentPrice;
    public event Action<TradeType, Ticker, int> OnTradeConfirmed;//событие подтвреждения сделки
=======
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
    //локальное событие
    public event Action<string> OnInputError;
>>>>>>> Stashed changes

    private void Awake()
    {
        _confirmButton.onClick.AddListener(() =>
        {
            int quantity;
            if (int.TryParse(_quantityInput.text, out quantity) && quantity > 0)
            {
                OnTradeConfirmed?.Invoke(_currentTradeType, _currentTicker, quantity);
            }

            else
            {
                Debug.Log("Некорректный ввод количества");
                return;
            }
        });
    }

    public void Show(TradeType type, Ticker ticker)
    {
        _currentTradeType = type;
        _currentTicker = ticker;
        string action = (type == TradeType.Buy) ? "Покупка" : "Продажа";
        this.gameObject.SetActive(true);
    }

    public void UpdateAssetPrice(float price)//<--заменить на int
    {
        _currentPrice.text = $"Текущая цена: {price}";
    }

}
public class PortfolioView : MonoBehaviour
    {
        //инфо на кнопке актива
        [SerializeField] private TMP_Text _assetNameText;
        [SerializeField] private TMP_Text _tickerText;
        [SerializeField] private TMP_Text _currentValueText;
        [SerializeField] private InputField _cashInputField;//количество cash для пополнения баланса
        [SerializeField] private TMP_Text _cashBalanceText;
        [SerializeField] private TMP_Text _totalValueText;
        [SerializeField] private TMP_Text _stocksValueText;
        [SerializeField] private TMP_Text _bondsValueText;
        [SerializeField] private TMP_Text _totalGainText;
        [SerializeField] private TMP_Text _dayGainText;
        [SerializeField] private TMP_Text _totalGainTextPercent;
        [SerializeField] private TMP_Text _dayGainTextPercent;
        [SerializeField] private TMP_Text _countStocks;
        [SerializeField] private TMP_Text _countBonds;

        [SerializeField] private Button _addCashButton;
        [SerializeField] private Button _checkOtherStocksButton;
        [SerializeField] private Button _checkOtherBondsButton;
        [SerializeField] private Button _analyticsButton;

        [SerializeField] private RectTransform _listContentParent; // Контейнер для строк
        [SerializeField] private AssetItemView _assetItemViewPrefab;

        private Ticker _selectedTicker;
        //private Mediator _mediator;

        public event Action OnActiveInfoClicked;
        public event Action<Ticker,TradeType> OnTradeActiveClicked;
        public event Action<int> OnAddCashClicked;
        public event Action OnCheckOtherStocksClicked;// посмотреть списки других активов
        public event Action OnCheckOtherBondsClicked;
        public event Action OnGetAnalyticsClicked;
        //локальное событие
        public event Action<string> OnInputError;

    private void Awake()
    {
        //_mediator = Mediator.Instance;
        //_mediator.OnInitializationCompleted += () => _mediator.GetService<PortfolioPresenter>().InitializeView(this);
       // _activeInfoButton.onClick.AddListener(() => OnActiveInfoClicked.Invoke());
        _checkOtherStocksButton.onClick.AddListener(() => OnCheckOtherStocksClicked.Invoke());
        _checkOtherBondsButton.onClick.AddListener(() => OnCheckOtherBondsClicked.Invoke());
        _analyticsButton.onClick.AddListener(() => OnGetAnalyticsClicked.Invoke());

        _addCashButton.onClick.AddListener(() =>
<<<<<<< Updated upstream
        {
            int amount;

            if (int.TryParse(_cashInputField.text, out amount))
            {
                OnAddCashClicked.Invoke(amount);
            }
            else
            {
               OnInputError?.Invoke("Некорректный ввод значения");
            }
=======
        {//заменить парсинг
>>>>>>> Stashed changes

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

<<<<<<< Updated upstream
    //метод удаления кнопок из контейнера
    public void ClearAssetListContainer()
    {
        foreach (Transform child in _listContentParent)
        {
            Destroy(child.gameObject);
=======
        //текст 



    }

    //регитсрация кнопки в контейнере всех кнопок
    public void RegisterAssetButton(Ticker ticker, AssetItemView newButtonView)
    {
        if (!_activeAssetViews.ContainsKey(ticker))
        {
            _activeAssetViews.Add(ticker, newButtonView);
>>>>>>> Stashed changes
        }
    }

    public AssetItemView CreateAssetItemView(string displayName)
    {
        return Instantiate(_assetItemViewPrefab, _listContentParent);
    }
<<<<<<< Updated upstream

=======
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
>>>>>>> Stashed changes

    //public void OnSellButtonClickHandler()
    //{
        //IActiv asset = GetSelectedAsset();
        //int quantity = int.Parse(_quantityInputField.text);
      //  OnSellActiveClicked?.Invoke(TradeType.Sell, asset, quantity);
    //}

    public Ticker GetSelectedAssetTicker()
    {
        return _selectedTicker;
    }
    //TODO вынести это в класс вьюшки элемента
    //  TMP_Text assetNameText = existingUI.GetComponentInChildren<TMP_Text>();
    //  if (assetNameText != null)
    //  {
    //       assetNameText.text = $"{Ticker} (x{Quantity})";
    //  }

    //     sellbuyButton.Ticker = ticker;
    //    sellbuyButton.Price = currentPrice;
    //    sellbuyButton.TradeType = TradeType.Sell;
    //    sellbuyButton.OnClickAction.AddListener(() =>//публикация
    //  {
    //     Mediator.Instance.GlobalEventBus.Publish(new OpenTradeWindowEvent(sellbuyButton.Ticker, sellbuyButton.Price, sellbuyButton.TradeType, 1));
    //   });

    public void UpdatePortfolioDisplay(PortfolioSummary summary, IActiv asset)//обновлению подлежат только данные о полной стоимости и балансе 
                                                                              // портфеля,цене актива
    {
        _cashBalanceText.text = summary.CashBalance.ToString();
        _totalValueText.text = summary.TotalValue.ToString();
        _bondsValueText.text = summary.BondsValue.ToString();
        _stocksValueText.text = summary.StocksValue.ToString();
        _totalGainText.text = summary.TotalGainLoss.ToString();
        _dayGainText.text = summary.DayGainLoss.ToString();
        _totalGainTextPercent.text = summary.TotalGainLossPercent.ToString();
        _dayGainTextPercent.text = summary.DayGainLossPercent.ToString();
        _countBonds.text = summary.CountBonds.ToString();
        _countStocks.text = summary.CountStocks.ToString();

        if (asset is SampleActiv sampleActiv)
        {
            //_quantityText.text = sampleActiv.Quantity.ToString();
        }
        else
        {
            //_mediator.GlobalEventBus.Publish<DebugLogErrorEvent>(new("Невозможно получить количество: объект не является активом"));
        }

        // _analyticsButton.gameObject.SetActive(true);
        // _addCashButton.gameObject.SetActive(true);

    }


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
