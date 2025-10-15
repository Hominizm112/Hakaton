using System;
using MyGame.Enums;
using System.Collections.Generic;
using System.Linq;
//using UnityEngine.Localization.Components;


<<<<<<< Updated upstream
public class PortfolioPresenter : MonoBehaviour
=======
public class PortfolioPresenter
>>>>>>> Stashed changes
{
    //[SerializeField] private LocalizeStringEvent localizeStringEvent;
    private PortfolioView _view;
    private MarketData _marketData;
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
        _tradingWindowView.OnTradeConfirmed += HandleConfirmTrade;
    }

    public void Initialize(Mediator mediator)
    {
        _mediator = mediator;
        _model = _mediator.GetService<PortfollioService>();
<<<<<<< Updated upstream
        _mediator.GlobalEventBus.Subscribe<AssetListChangedEvent>(HandleAssetListChanged);
        var allAssets = _model.Assets;
        var allAssetsInfo = allAssets.ToDictionary(
        kvp => kvp.Key, 
        kvp => kvp.Key.ToString() // Используем Ticker как отображаемое имя
    );
         SetupAssetList();
=======
        _tradingWindowView = _mediator.GetService<TradingWindowView>();
        //_mediator.GlobalEventBus.Subscribe<AssetListChangedEvent>(HandleAssetListChanged);

        SetupAssetList();
>>>>>>> Stashed changes

    }

    private void SetupAssetList()//инициализация портфолио,создание кнопок
    {
        _portfolioSummary.UpdateCashBalance();
        _view.CreatePortfolioView();
        _model.PortfolioInitialize();

        Dictionary<Ticker, string> allAssetsInfo = GetCombinedAssetInfo();
        _view.ClearAssetListContainer();//очищение контейнера перед перерисовкой
        foreach (var kvp in allAssetsInfo)
        {
            Ticker ticker = kvp.Key;
            string displayName = kvp.Value;
<<<<<<< Updated upstream
            AssetItemView itemView = _view.CreateAssetItemView(displayName);
            itemView.Initialize(ticker, displayName);
=======
            AssetItemView itemView = _view.CreateAssetItemView();
            int price = _model.GetAssetPrice(ticker);
            int quantity = _model.GetQuantityByTicker(ticker);
            itemView.Initialize(ticker, price, quantity, true);//где взять price, quantit
>>>>>>> Stashed changes
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
<<<<<<< Updated upstream

    public void LoadInitialData()
    {
=======
 
    private Dictionary<Ticker, string> GetAllStocksForDisplay()
    {
        var allStocks = _marketData.AllMarketStocks;
        return allStocks.ToDictionary(
            kvp => kvp.Key,
            kvp => CompanyInfo.ActiveName[kvp.Key]
        );


>>>>>>> Stashed changes
    }
    private void HandleInfoActiv()
    {
<<<<<<< Updated upstream

=======
        var allBonds = _marketData.AllMarketBonds;
        return allBonds.ToDictionary(
            kvp => kvp.Key,
            kvp => CompanyInfo.ActiveName[kvp.Key]
        );
>>>>>>> Stashed changes
    }

    //открытие окна торговли
    private void HandleOpenTradeWindowRequest(Ticker ticker, TradeType tradeType)
    {
<<<<<<< Updated upstream
        float price = _model.GetAssetPrice(ticker);
=======
        int price = _model.GetAssetPrice(ticker);
>>>>>>> Stashed changes

        if (price <= 0.0f)
        {
            _mediator.GlobalEventBus.Publish<DebugLogErrorEvent>(new($"Цена для актива {ticker} не найдена."));
            return;
        }
<<<<<<< Updated upstream
        IActiv asset = _model.GetAssetByTicker(ticker);
        _tradingWindowView.Show(tradeType, ticker);
        _tradingWindowView.UpdateAssetPrice(price);//<---заменить на int
=======

        //IActiv asset = _model.GetAssetByTicker(ticker);
        _tradingWindowView.Show(tradeType, ticker, price);
        // _tradingWindowView.UpdateAssetPrice(price);
>>>>>>> Stashed changes
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

<<<<<<< Updated upstream
        float assetPrice = asset.CurrentValue;//<---заменить на int
=======
        int assetPrice = asset.CurrentValue;
>>>>>>> Stashed changes
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
                    ColorfulDebug.LogGreen("Успешная покупка");
                }
                break;
            case TradeType.Sell:
                {
                    HandleSell(assetType, ticker, quantity, assetPrice);
                    ColorfulDebug.LogGreen("Успешная продажа");
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

<<<<<<< Updated upstream
    private void HandleBuy(Type AssetType, Ticker ticker, int quantity, float assetPrice)
=======
    private void HandleBuy(Type AssetType, Ticker ticker, int quantity, int assetPrice)
>>>>>>> Stashed changes
    {
        float totalCost = quantity * assetPrice;
        if (!_model.HasEnoughCash(totalCost))
        {
<<<<<<< Updated upstream
            _mediator.GlobalEventBus.Publish<DebugLogErrorEvent>(new("Недостаточно средств для покупки"));
            return;
=======
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
>>>>>>> Stashed changes
        }
        _model.BuyAsset(AssetType, ticker, quantity, totalCost);
        ColorfulDebug.LogGreen($"Успешная покупка {ticker}");//или публикация события
    }

    private void HandleSell(Type AssetType, Ticker ticker, int quantity, float assetPrice)
    {
        float totalCost = quantity * assetPrice;
        if (!_model.HasEnoughQuantityActive(ticker, quantity))
        {
<<<<<<< Updated upstream
            _mediator.GlobalEventBus.Publish<DebugLogErrorEvent>(new($"Недостаточно {ticker} для продажи."));
            return;
=======
            case SellTransactionState.NoNeedRemovedButton:
                {
                    //ColorfulDebug.LogGreen($"Успешная продажа {ticker}");
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

>>>>>>> Stashed changes
        }
        _model.SellAsset(AssetType, ticker, quantity, totalCost);
        ColorfulDebug.LogGreen($"Успешная продажа {ticker}");//или публикация события
    }

   // private void HandleTradeRequest(TradeRequestEvent @event)
   // {
        // поиск и покупка нужного объекта
       // IActiv activ = FindAssetByTicker(@event.Ticker);

       // if (activ != null)
       // {
        //    HandleTradeActiv(@event.TradeType, activ, @event.Quantity);
       // }
   // }

    //private IActiv FindAssetByTicker(Ticker ticker)
    //{
       // if (_model.AvailableStocks.TryGetValue(ticker, out Stock stock))
       // {
       //     return stock;
       // }

       // if (_model.AvailableBonds.TryGetValue(ticker, out Bond bond))
      //  {
       //     return bond;
       // }
       // return null;
   // }

    private void HandleAddCash(int amount)
    {
        if (amount <= 0)
        {
            _mediator.GlobalEventBus.Publish<DebugLogErrorEvent>(new("Некорректный ввод"));
            return;
        }
        _portfolioSummary.AddCashBalance(amount);

<<<<<<< Updated upstream
        if (_mediator.GetService<CurrencyPresenter>().TrySpendCurrency(amount))
        {
            _model.AddCash(amount);
        }
=======
>>>>>>> Stashed changes
    }
    private void HandleAssetListChanged(AssetListChangedEvent @event)
    {
        SetupAssetList();
    }
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
