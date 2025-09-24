using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using MyGame.Enums;
public class PortfolioApp : MonoBehaviour
{
    [SerializeField] private Transform _assetListContainer; //для динамического добавления активов
    [SerializeField] private GameObject _assetUIPrefab;
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

    private Dictionary<string, GameObject> _activeUIElements = new Dictionary<string, GameObject>();
    private PortfollioService _portfolioService;
    Mediator _mediator;
    private void Awake()
    {
        _portfolioService = Mediator.Instance.GetService<PortfollioService>();
        if (_portfolioService != null)
        {
            ///
        }
        else
        {
            _mediator.GlobalEventBus.Publish<DebugLogErrorEvent>(new("PortfollioService  не найден"));
        }
    }

    private void OnDestroy()
    {

    }
    private void UpdateUI(PortfolioSummary summary)
    {
        _cashBalanceText.text = summary.CashBalance.ToString("C");
        _totalValueText.text = summary.TotalValue.ToString("C");

        HashSet<string> updatedTickers = new HashSet<string>();

        _bondsValueText.text = summary.BondsValue.ToString("C");
        _stocksValueText.text = summary.StocksValue.ToString("C");
        _totalGainText.text = summary.TotalGainLoss.ToString("C");
        _dayGainText.text = summary.DayGainLoss.ToString("C");
        _totalGainTextPercent.text = summary.TotalGainLossPercent.ToString("C");
        _dayGainTextPercent.text = summary.DayGainLossPercent.ToString("C");
        _countBonds.text = summary.CountBonds.ToString("C");
        _countStocks.text = summary.CountStocks.ToString("C");

        _analyticsButton.gameObject.SetActive(true);
        _addCashButton.gameObject.SetActive(true);

        foreach (var entry in summary.MyStocks)
        {
            string ticker = entry.Key;
            int quantity = entry.Value;

            if (quantity > 0)
            {
                if (_activeUIElements.ContainsKey(ticker))
                {
                    GameObject existingUI = _activeUIElements[ticker];
                    TMP_Text assetNameText = existingUI.GetComponentInChildren<TMP_Text>();
                    if (assetNameText != null)
                    {
                        assetNameText.text = $"{ticker} (x{quantity})";
                    }
                }
                else
                {
                    CreateAssetUI(ticker, quantity);
                }
                updatedTickers.Add(ticker);
            }

        }  
    List<string> tickersToRemove = new List<string>();
    foreach (var existingTicker in _activeUIElements.Keys)
    {
        if (!updatedTickers.Contains(existingTicker))
        {
            tickersToRemove.Add(existingTicker);
        }
    }

    foreach (var ticker in tickersToRemove)
    {
        Destroy(_activeUIElements[ticker]);
        _activeUIElements.Remove(ticker);
    }
    }
    private void CreateAssetUI(string ticker, int quantity)
    {
        GameObject assetUI = Instantiate(_assetUIPrefab, _assetListContainer);
        _activeUIElements[ticker] = assetUI;
        TMP_Text assetNameText = assetUI.GetComponentInChildren<TMP_Text>();

        if (assetNameText != null)
        {
            assetNameText.text = $"{ticker} (x{quantity})";
        }
        float currentPrice = _portfolioService.GetAssetPrice(ticker);
        //кнопка инфо об активе
        Button assetButton = assetUI.GetComponent<Button>();
        if (assetButton != null)
        {
            assetButton.onClick.AddListener(() =>
            {
                object assetData = _portfolioService.GetAssetByTicker(ticker);
                Mediator.Instance.GlobalEventBus.Publish(new OpenAssetPageEvent(assetData));
            });
        }
        //действие кнопки sell(buy)
        QuickTradeButton sellbuyButton = assetUI.GetComponentInChildren<QuickTradeButton>();
        if (sellbuyButton!= null)
        {
            sellbuyButton.Ticker = ticker;
            sellbuyButton.Price = currentPrice;
            sellbuyButton.TradeType = TradeType.Sell;
            sellbuyButton.OnClickAction.AddListener(() =>//публикация
            {
                Mediator.Instance.GlobalEventBus.Publish(new OpenTradeWindowEvent(sellbuyButton.Ticker, sellbuyButton.Price, sellbuyButton.TradeType));
            });
        }
    }



}

    
