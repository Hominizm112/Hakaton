using System;
using MyGame.Enums;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Localization.Components;
using Zenject;


public class PortfolioPresenter : MonoService
{
    [SerializeField] private LocalizeStringEvent localizeStringEvent;
    private PortfolioView _view;
    private MarketData _marketData;
    [Inject] private PortfollioService _model;
    private PortfolioSummary _portfolioSummary = new PortfolioSummary();
    [Inject] private TradingWindowView _tradingWindowView;

    public override void Initialize()
    {
        base.Initialize();
        //_mediator.GlobalEventBus.Subscribe<AssetListChangedEvent>(HandleAssetListChanged);
        //var allAssets = _model.Assets;
        var allAssets = _portfolioSummary.MyActives;
        var allAssetsInfo = allAssets.ToDictionary(
        kvp => kvp.Key,
        kvp => kvp.Key.ToString() // Используем Ticker как отображаемое имя
    );

        if (_tradingWindowView != null)
        {              //Mediator.Instance.GlobalEventBus.Subscribe<OpenTradeWindowEvent>(HandleOpenEvent);
            _tradingWindowView.OnTradeConfirmed += HandleConfirmTrade;
        }
        //_mediator.GlobalEventBus.Subscribe<AssetListChangedEvent>(HandleAssetListChanged);
        PortfolioInitialize();
        InitializeView(_view);

    }
    public void InitializeView(PortfolioView portfolioView)
    {
        _view = GetComponent<PortfolioView>();
        _view.OnAddCashClicked += HandleAddCash;
        _view.OnCheckOtherStocksClicked += HandleCheckOtherStock;
        _view.OnCheckOtherBondsClicked += HandleCheckOtherBond;
        _view.OnGetAnalyticsClicked += HandleGetPortfolioReport;
        ///_tradingWindowView.OnTradeConfirmed += HandleConfirmTrade;

    }


    private void PortfolioInitialize()//инициализация портфолио,создание кнопок
    {
        //инициализация модели
        _portfolioSummary.UpdateCashBalance();
        _model.PortfolioInitialize();
        PortfolioSummary summary = _model.GetSummary();

        //UI
        _view.UpdatePortfolioView(summary);
        foreach (var kvp in summary.MyActives)
        {
            Ticker ticker = kvp.Key;
            AssetItemView itemView = _view.CreateAssetItemView();
            int price = _model.GetAssetPrice(ticker);
            int quantity = _model.GetQuantityByTicker(ticker);
            _view.CreateAssetItemView();
            //_view.UpdateAssetButton(ticker, newPrice, newQuantity);
            itemView.Initialize(ticker, price, quantity, true);
            itemView.OnOpenTradeRequested += HandleOpenTradeWindowRequest;
        }
        _view.CreatePortfolioView();

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

        if (price <= 0.0)
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
        int totalCost = asset.CurrentValue * quantity;
        HandleTradeActiv(tradeType, asset, quantity);

    }

    #region HandleTrade
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

        _view.UpdatePortfolioView(_portfolioSummary);//старая сводка?
        _model.UpdatePortfolioValue(AssetType, totalCost, quantity, TradeType.Buy);
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
        _model.UpdatePortfolioValue(AssetType, totalCost, quantity, TradeType.Sell);

    }
    #endregion
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



}
