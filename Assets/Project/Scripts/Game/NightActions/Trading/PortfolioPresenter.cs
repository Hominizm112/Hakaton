using UnityEngine;
using System;
using MyGame.Enums;
using System.Collections.Generic;
using System.Linq;

public class PortfolioPresenter : MonoBehaviour
{
    [SerializeField] private PortfolioView _view;
    private AssetItemView _itemView;
    private PortfollioService _model;
    private Mediator _mediator;
    private PortfolioSummary _portfolioSummary = new PortfolioSummary();
    private TradingWindowView _tradingWindowView;
    private Dictionary<Ticker, AssetItemView> _assetItemViews; // Храним ссылки на созданные View
    public void InitializeView(PortfolioView portfolioView)
    {
        _view = portfolioView;
        _view.OnAddCashClicked += HandleAddCash;
        _view.OnCheckOtherStocksClicked += HandleCheckOtherStock;
        _view.OnCheckOtherBondsClicked += HandleCheckOtherBond;
        _view.OnGetAnalyticsClicked += HandleGetPortfolioReport;
        _tradingWindowView.OnTradeConfirmed += HandleConfirmTrade; 
    }

    public void Initialize(Mediator mediator)
    {
        _mediator = mediator;
        _model = _mediator.GetService<PortfollioService>();
        _tradingWindowView = _mediator.GetService<TradingWindowView>();
        _mediator.GlobalEventBus.Subscribe<AssetListChangedEvent>(HandleAssetListChanged);

        SetupAssetList();
        UpdatePortfolioSummary();

    }
    private void UpdatePortfolioSummary()
    {
        IReadOnlyDictionary<Ticker, SampleActiv> currentPortfolio =_portfolioSummary.MyActives;
        int totalValue = _model.RecalculateTotalPortfolioValue();
        int currentCash = _model.GetAvailableCash(); 
        //_view.UpdateCashDisplay(currentCash); 

    }

    private void SetupAssetList()
    {
        Dictionary<Ticker, string> allAssetsInfo = GetCombinedAssetInfo();
        foreach (var kvp in allAssetsInfo)
        {
            Ticker ticker = kvp.Key;
            string displayName = kvp.Value;
            AssetItemView itemView = _view.CreateAssetItemView();
            itemView.Initialize(ticker, price, quantity, true);//где взять price, quantit
            itemView.OnOpenTradeRequested += HandleOpenTradeWindowRequest;
            //itemView.OnAssetDetailsClicked += HandleAssetDetailsClicked;
        }
    }

    private Dictionary<Ticker, string> GetCombinedAssetInfo()
    {
        var allAssets = _model.Assets;
        return allAssets.ToDictionary(
        kvp => kvp.Key,            
        kvp => kvp.Key.ToString()
    );
    }

    private Dictionary<Ticker, string> GetAllAssetsForDisplay()
    {
        var allAssets = _model.GetAllAvailableAssets();
        return allAssets.ToDictionary(
        kvp => kvp.Key,            
        kvp => CompanyInfo.ActiveName[kvp.Key]
    );
    }

    //открытие окна торговли
    private void HandleOpenTradeWindowRequest(Ticker ticker, TradeType tradeType)
    {
        int price = _model.GetAssetPrice(ticker);//заменить на int

        if (price <= 0)
        {
            _mediator.GlobalEventBus.Publish<DebugLogErrorEvent>(new($"Цена для актива {ticker} не найдена."));
            return;
        }
        
        //IActiv asset = _model.GetAssetByTicker(ticker);
        _tradingWindowView.Show(tradeType, ticker, price);
       // _tradingWindowView.UpdateAssetPrice(price);//<---заменить на int
    }

    private void HandleConfirmTrade(TradeType tradeType, Ticker ticker, int quantity)
    {
        IActiv asset = _model.GetAssetByTicker(ticker);//поиск актива по тикеру
        if (asset == null)
        {
            _mediator.GlobalEventBus.Publish<DebugLogErrorEvent>(new("Актив не найден"));
            return;
        }
        HandleTradeActiv(tradeType, asset, quantity);
        _tradingWindowView.TradeWindowClose();

    }
    
    private void HandleTradeActiv(TradeType tradeType, IActiv asset, int quantity)
    {
        if (asset == null)
        {
            _mediator.GlobalEventBus.Publish<DebugLogErrorEvent>(new("Неверные параметры торговой операции"));
            return;
        }

        int assetPrice = asset.CurrentValue;//<---заменить на int
        Ticker ticker = asset.Ticker;
        Type assetType = asset.GetType();

        // if (!TryGetAssetInfo(asset, out assetPrice, out ticker, out assetType))
        // {
        //   return;
        // }
        switch (tradeType)
        {
            case TradeType.Buy:
                {
                    HandleBuy(assetType, ticker, quantity, assetPrice);

                }
                break;
            case TradeType.Sell:
                {
                    HandleSell(assetType, ticker, quantity, assetPrice);
                }
                break;
            default:
                _mediator.GlobalEventBus.Publish<DebugLogErrorEvent>(new("Неподдерживаемый тип операции"));
                break;
        }
    }
    
    private bool TryGetAssetInfo(IActiv asset, out int price, out Ticker ticker, out Type type)
    {
        price = 0;
        ticker = Ticker.None;
        type = null;
        switch (asset)
        {
            case Stock stock when stock.StockInfo != null:
                price = stock.CurrentValue;
                ticker = stock.StockInfo.Ticker;
                type = typeof(Stock);
                return true;

            case Bond bond when bond.BondInfo != null:
                price = bond.CurrentValue;
                ticker = bond.BondInfo.Ticker;
                type = typeof(Bond);
                return true;

            default:
                return false;
        }
    }

    private void HandleBuy(Type AssetType, Ticker ticker, int quantity, int assetPrice)//<--int
    {
       int totalCost = quantity * assetPrice;

        BuyTransactionState transactionStatus = _model.BuyAsset(AssetType, ticker, assetPrice, quantity);
        switch (transactionStatus)
        {
            case BuyTransactionState.NotEnough:
                {
                    _mediator.GlobalEventBus.Publish<DebugLogErrorEvent>(new("Недостаточно средств для покупки"));
                    return;
                }
            case BuyTransactionState.NeedCreatedButton:
                {
                    SampleActiv newAsset = _model.CreateAssetInstance(AssetType,ticker,assetPrice,quantity);
                    AssetItemView newButtonView = _view.CreateAssetItemView();
                    _view.RegisterAssetButton(ticker, newButtonView);
                    newButtonView.Initialize(ticker, assetPrice, quantity, true);
                    newButtonView.OnOpenTradeRequested += HandleOpenTradeWindowRequest;
                    break;

                }
            case BuyTransactionState.NoNeedCreatedButton:
                {
                    int newAssetQuantity = _model.GetQuantityByTicker(ticker) + quantity;
                    _view.UpdateAssetButton(ticker, assetPrice, newAssetQuantity);
                    ColorfulDebug.LogGreen($"Успешная покупка {ticker}");
                    break;

                }
        }

        _portfolioSummary.RecalculateValueCount(AssetType, true, totalCost, quantity);
        _portfolioSummary.RecalculateCashBalance(true, totalCost);
        _view.UpdatePortfolioView(_portfolioSummary);        
    }

    private void HandleSell(Type AssetType, Ticker ticker, int quantity, int assetPrice)
    {
        int totalCost = quantity * assetPrice;

        SellTransactionState transactionStatus = _model.SellAsset(AssetType, ticker, quantity, totalCost);

        switch (transactionStatus)
        {
            case SellTransactionState.NoNeedRemovedButton:
                {
                    ColorfulDebug.LogGreen($"Успешная продажа {ticker}");
                    break;
                }
            case SellTransactionState.NeedRemovedButton:
                {
                    _view.DeactivateAssetButton(ticker);
                    _portfolioSummary.MyActives[ticker].RemoveQuantity(quantity);
                    ColorfulDebug.LogGreen($"Кнопка {ticker} удалена");
                    break;
                }
            case SellTransactionState.NotEnough:
                {
                    _mediator.GlobalEventBus.Publish<DebugLogErrorEvent>(new($"Недостаточно {ticker} для продажи."));
                    return;
                }
        }
        _view.UpdatePortfolioView(_portfolioSummary);
        
    }

    private void HandleAddCash(int amount)
    {
        if (amount <= 0)
        {
            _mediator.GlobalEventBus.Publish<DebugLogErrorEvent>(new("Некорректный ввод"));
            return;
        }
        else if (amount > 100000)
        {
            _mediator.GlobalEventBus.Publish<DebugLogErrorEvent>(new("Достигнут лимит пополнения"));
            return;
        }

        if (_mediator.GetService<CurrencyPresenter>().TrySpendCurrency(amount))
        {
            _model.AddCash(amount);
        }
    }

    private void HandleInfoActiv()
    {

    }
    private void UpdatePortfolioModel()
    {

    }
    private void HandleAssetListChanged(AssetListChangedEvent @event)
    {
        SetupAssetList();
    }
    private void HandleCheckOtherStock()
    {

        _view.OpenOtherStockScreen();

    }

    private void HandleCheckOtherBond()
    {

        _view.OpenOtherBondScreen();

    }

    private void HandleGetPortfolioReport()
    {

        _view.SnowPortfolioReport();

    }

    private void OnDestroy()
    {

    }


}
