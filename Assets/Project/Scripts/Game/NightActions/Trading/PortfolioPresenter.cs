using System;
using MyGame.Enums;
using System.Collections.Generic;
using System.Linq;
//using UnityEngine.Localization.Components;

//using UnityEngine.Localization.Components;


public class PortfolioPresenter
{
    //[SerializeField] private LocalizeStringEvent localizeStringEvent;
    private PortfolioView _view;
    private MarketData _marketData;
    //[SerializeField] private LocalizeStringEvent localizeStringEvent;
    private PortfollioService _model;
    private Mediator _mediator;
    private PortfolioSummary _portfolioSummary = new PortfolioSummary();
    private TradingWindowView _tradingWindowView;
    
    
    public void InitializeView(PortfolioView portfolioView)
    {
        _view = portfolioView;
        _view.OnAddCashClicked += HandleAddCash;
        _view.OnCheckOtherStocksClicked += HandleCheckOtherStock;
        _view.OnCheckOtherBondsClicked += HandleCheckOtherBond;
        _view.OnGetAnalyticsClicked += HandleGetPortfolioReport;
      //  _tradingWindowView.OnTradeConfirmed += HandleConfirmTrade;
       // _tradingWindowView.OnTradeConfirmed += HandleConfirmTrade;
    }

    public void Initialize(Mediator mediator)
    {
        _mediator = mediator;
        _model = _mediator.GetService<PortfollioService>();
        //_mediator.GlobalEventBus.Subscribe<AssetListChangedEvent>(HandleAssetListChanged);
        var allAssets = _model.Assets;
        var allAssetsInfo = allAssets.ToDictionary(
        kvp => kvp.Key, 
        kvp => kvp.Key.ToString() // Используем Ticker как отображаемое имя
    );

        _tradingWindowView = _mediator.GetService<TradingWindowView>();
        //_mediator.GlobalEventBus.Subscribe<AssetListChangedEvent>(HandleAssetListChanged);

        SetupAssetList();

    }

    private void SetupAssetList()//инициализация портфолио,создание кнопок
    {
        _portfolioSummary.UpdateCashBalance();
        _view.CreatePortfolioView();
        _model.PortfolioInitialize();

        Dictionary<Ticker, string> allAssetsInfo = GetCombinedAssetInfo();
        foreach (var kvp in allAssetsInfo)
        {
            Ticker ticker = kvp.Key;
            string displayName = kvp.Value;
            AssetItemView itemView = _view.CreateAssetItemView();
            int price = _model.GetAssetPrice(ticker);
            int quantity = _model.GetQuantityByTicker(ticker);
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
 
    //private Dictionary<Ticker, string> GetAllStocksForDisplay()
   //{
       // var allStocks = _marketData.AllMarketStocks;
        //return allStocks.ToDictionary(
        //    kvp => kvp.Key
           // kvp => CompanyInfo.ActiveName[kvp.Key]
       // );

   // }
    private void HandleInfoActiv()
    {
    
    }

    //открытие окна торговли
    private void HandleOpenTradeWindowRequest(Ticker ticker, TradeType tradeType)
    {
        int price = _model.GetAssetPrice(ticker);

        if (price <= 0.0f)
        {
            _mediator.GlobalEventBus.Publish<DebugLogErrorEvent>(new($"Цена для актива {ticker} не найдена."));
            return;
        }

        //IActiv asset = _model.GetAssetByTicker(ticker);
        _tradingWindowView.Show(tradeType, ticker, price);
        // _tradingWindowView.UpdateAssetPrice(price);
    }

    private void HandleConfirmTrade(TradeType tradeType, Ticker ticker, int quantity)
    {
        IActiv asset = _model.GetAssetByTicker(ticker);//поиск актива по тикеру
        if (asset == null)
        {
            _mediator.GlobalEventBus.Publish<DebugLogErrorEvent>(new("Актив не найден"));
            return;
        }
        float totalCost =asset.CurrentValue * quantity;
        HandleTradeActiv(tradeType, asset, quantity);
        
    }
    private void HandleTradeActiv(TradeType tradeType, IActiv asset, int quantity)
    {
        if (asset == null || quantity <= 0)
        {
            _mediator.GlobalEventBus.Publish<DebugLogErrorEvent>(new("Неверные параметры торговой операции"));
            return;
        }

        int assetPrice = asset.CurrentValue;
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
    
    private bool TryGetAssetInfo(IActiv asset, out float price, out Ticker ticker, out Type type)
    {
        price = 0f;
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

    private void HandleBuy(Type AssetType, Ticker ticker, int quantity, int assetPrice)
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
                    IActiv newAsset = _model.CreateAssetInstance(AssetType, ticker, assetPrice, quantity);
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
                    // ColorfulDebug.LogGreen($"Успешная покупка {ticker}");
                    break;

                }
        }

        _view.UpdatePortfolioView(_portfolioSummary);
        _model.UpdatePortfolioValue(AssetType,totalCost,quantity,TradeType.Buy);  
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
                    // ColorfulDebug.LogGreen($"Кнопка {ticker} удалена");
                    break;
                }
            case SellTransactionState.NotEnough:
                {
                    _mediator.GlobalEventBus.Publish<DebugLogErrorEvent>(new($"Недостаточно {ticker} для продажи."));
                    return;
                }

        }

        _view.UpdatePortfolioView(_portfolioSummary);
        _model.UpdatePortfolioValue(AssetType,totalCost,quantity,TradeType.Sell);
    
    }

    private void HandleAddCash(int amount)
    {
        if (amount <= 0)
        {
            _mediator.GlobalEventBus.Publish<DebugLogErrorEvent>(new("Некорректный ввод"));
            return;
        }
        _portfolioSummary.AddCashBalance(amount);

    }

    private void UpdatePortfolioModel()
    {

    }
    //private void HandleAssetListChanged(AssetListChangedEvent @event)
    //{
       // SetupAssetList();
    //}
    private void HandleCheckOtherStock()
    {

    }

    private void HandleCheckOtherBond()
    {

    }

    private void HandleGetPortfolioReport()
    {

    }

    private void OnDestroy()
    {

    }


}
