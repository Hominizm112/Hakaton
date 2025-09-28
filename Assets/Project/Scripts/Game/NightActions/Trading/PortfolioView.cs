using UnityEngine;
using MyGame.Enums;
using TMPro;
using UnityEngine.UI;
using System;
public class PortfolioView : MonoBehaviour
{
    //инфо на кнопке актива
    [SerializeField] private TMP_Text _assetNameText;
    [SerializeField] private TMP_Text _tickerText;
    [SerializeField] private TMP_Text _quantityText;
    [SerializeField] private TMP_Text _currentValueText;

    [SerializeField] private InputField _cashInputField;
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

    [SerializeField] private Button _activeInfoButton;
    [SerializeField] private Button _buyActiveButton;
    [SerializeField] private Button _sellActiveButton;
    [SerializeField] private Button _addCashButton;
    [SerializeField] private Button _checkOtherStocksButton;
    [SerializeField] private Button _checkOtherBondsButton;
    [SerializeField] private Button _analyticsButton;


    //public Ticker Ticker;
    //public float Price;
    //public int Quantity;
    //public TradeType TradeType;
    private Mediator _mediator;

    public event Action OnActiveInfoClicked;
    public event Action OnBuyActiveClicked;
    public event Action OnSellActiveClicked;
    public event Action<int> OnAddCashClicked;
    public event Action OnCheckOtherStocksClicked;// посмотреть списки других активов
    public event Action OnCheckOtherBondsClicked;
    public event Action OnGetAnalyticsClicked;

    private void Awake()
    {
        _mediator = Mediator.Instance;
        _mediator.OnInitializationCompleted += () => _mediator.GetService<PortfolioPresenter>().InitializeView(this);

        _activeInfoButton.onClick.AddListener(() => OnActiveInfoClicked.Invoke());
        _buyActiveButton.onClick.AddListener(() => OnBuyActiveClicked.Invoke());
        _sellActiveButton.onClick.AddListener(() => OnSellActiveClicked.Invoke());
        _checkOtherStocksButton.onClick.AddListener(() => OnCheckOtherStocksClicked.Invoke());
        _checkOtherBondsButton.onClick.AddListener(() => OnCheckOtherBondsClicked.Invoke());
        _analyticsButton.onClick.AddListener(() => OnGetAnalyticsClicked.Invoke());
        _addCashButton.onClick.AddListener(() =>
        {

            int amount;

            if (int.TryParse(_cashInputField.text, out amount))
            {
                OnAddCashClicked.Invoke(amount);
            }
            else
            {
                _mediator.GlobalEventBus.Publish<DebugLogErrorEvent>(new("Некорректное считывание введенного значения"));
            }

        });
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
    // портфеля,цене актива, количества активов в портфеле
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
            _quantityText.text = sampleActiv.Quantity.ToString();
        }
        else
        {
            _mediator.GlobalEventBus.Publish<DebugLogErrorEvent>(new("Невозможно получить количество: объект не является активом"));
        }

        // _analyticsButton.gameObject.SetActive(true);
        // _addCashButton.gameObject.SetActive(true);

    }

    public void OpenTradeWindow()
    {
    
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
    //public void OnClick()
    //{
    //  Mediator.Instance.GlobalEventBus.Publish(new OpenTradeWindowEvent(Ticker, Price, TradeType, Quantity));
    //}

    private void OnDestroy()
    {
        _activeInfoButton.onClick.RemoveAllListeners();
        _buyActiveButton.onClick.RemoveAllListeners();
        _sellActiveButton.onClick.RemoveAllListeners();
        _addCashButton.onClick.RemoveAllListeners();
        _checkOtherStocksButton.onClick.RemoveAllListeners();
        _checkOtherBondsButton.onClick.RemoveAllListeners();
        _analyticsButton.onClick.RemoveAllListeners();

    }
}
