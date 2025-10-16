
using System.Collections.Generic;
using MyGame.Enums;
using System;


public abstract class SampleActiv<TConfig> : IActiv<TConfig> where TConfig : IAssetConfig
{
    private int _quantity;
    public int Quantity => _quantity;
    public abstract Ticker Ticker { get; }
    public abstract TConfig Config { get; }  
    public int CurrentValue { get; protected set; } 

    public void AddQuantity(int amount)
    {
        _quantity += amount;
    }

    public void RemoveQuantity(int amount)
    {
        _quantity -= amount;
    }

    public SampleActiv(int initialValue, int initialQuantity, TConfig config)
    {
        CurrentValue = initialValue;
        _quantity = initialQuantity;
    }

}

public class PortfollioService //MonoService // IPortfolioService
{

    private MarketData _marketData;
    private PortfolioSummary _portfolioSummary = new PortfolioSummary();
    private Mediator _mediator;
    //public override List<Type> requiredServices { get; protected set; } = new List<Type>();
    //public Dictionary<Ticker, Stock> AvailableStocks { get; private set; }
    //public Dictionary<Ticker, Bond> AvailableBonds { get; private set; }
    private Dictionary<Ticker, IActiv> AvailableAssets { get; set; } = new();
    public IReadOnlyDictionary<Ticker, IActiv> Assets => AvailableAssets;

    public Action<PortfolioSummary> OnPortfolioUpdated { get; internal set; }

    private void Awake()
    {
        Mediator.Instance.RegisterService(this);
    }

    public void PortfolioInitialize()
    {
        AvailableAssets = new Dictionary<Ticker, IActiv>();
        

        
    }

    #region FindByTicker

    public IActiv GetAssetByTicker(Ticker ticker)// поиск актива по тикеру
    {

        if (_portfolioSummary.MyActives.TryGetValue(ticker, out IActiv activ))
        {
            return activ;
        }

       // _mediator.GlobalEventBus.Publish<DebugLogErrorEvent>(new("Актива с заданным тикером не существует"));
        return null;

    }
    public int GetAssetPrice(Ticker ticker)//поиск цены по тикеру
    {
        if (_portfolioSummary.MyActives.TryGetValue(ticker, out IActiv activ))
        {
            return activ.CurrentValue;
        }

        throw new KeyNotFoundException($"Актив с тикером {ticker} не найден в портфеле игрока.");

    }

    public int GetQuantityByTicker(Ticker targetTicker)//возврат поля количества по тикеру
    {

        if (_portfolioSummary.MyActives.TryGetValue(targetTicker, out IActiv activ))
        {
            return activ.Quantity;
        }
        else
        {
            return 0;
        }
    }
    
#endregion
    public void UpdatePortfolioValue(Type assetType, int totalCost, int quantity, TradeType TypeOperation)
    {
        _portfolioSummary.RecalculateValueCount(assetType, TypeOperation, totalCost, quantity);
        _portfolioSummary.RecalculateCashBalance(TypeOperation, totalCost);
    }


    //кнопки быстрой продажи покупки
    #region BuyActiv

    public BuyTransactionState BuyAsset(Type assetType, Ticker ticker, int price, int quantity)
    {
        if (!_marketData.AllMarketBonds.ContainsKey(ticker) || !HasEnoughCash(price*quantity))
        {
            return BuyTransactionState.NotEnough;
        }

        _portfolioSummary.AddQuantity(ticker,quantity);//обновление количества

        if (_portfolioSummary.MyActives.TryGetValue(ticker, out IActiv asset))
        {
            return BuyTransactionState.NoNeedCreatedButton;
        }

        else
        {
            IActiv newAsset = CreateAssetInstance(assetType, ticker, price, quantity);
            _portfolioSummary.AddMyActive(ticker, newAsset);
            return BuyTransactionState.NeedCreatedButton;
        }

    }
    

    #region SellActiv
    public SellTransactionState SellAsset(Type assetType, Ticker ticker, int quantity, int totalCost)
    {
        if (!HasEnoughQuantityActive(ticker, quantity))//недостаточно активов
        {
            return SellTransactionState.NotEnough;
        }

        IActiv asset = GetAssetByTicker(ticker);
        _portfolioSummary.RemoveQuantity(ticker,quantity);

        if (asset.Quantity <= 0)
        {
            _portfolioSummary.RemoveMyActive(ticker);
            return SellTransactionState.NeedRemovedButton; //удаление кнопки
        }
        else
        {
            //просто изменение количества
            return SellTransactionState.NoNeedRemovedButton;
        }
    }

    #endregion
    public IActiv CreateAssetInstance(Type assetType, Ticker ticker, int price, int quantity)
    {
        IAssetConfig baseConfig = _marketData.FindAssetConfigInMarket(ticker, assetType);
        IActiv asset = null;

        //switch??
        if (assetType == typeof(Stock))
        {
            if (baseConfig is StockConfig stockConfig)
            {
                asset = new Stock(stockConfig, price, quantity);
            }
        }

        else if (assetType == typeof(Bond))
        {
            if (baseConfig is BondConfig bondConfig)
            {
                asset = new Bond(bondConfig, price, quantity);
            }
        }

        return asset;
    }
    #endregion

    public bool HasEnoughQuantityActive(Ticker ticker, int quantity)
    {
       return !(!_portfolioSummary.MyActives.ContainsKey(ticker) || 
             _portfolioSummary.MyActives[ticker].Quantity < quantity);
    }

    public bool HasEnoughCash(int totalCost)
    {
        return _portfolioSummary.CashBalance >= totalCost;
    }
 
    //покупка иных
    public void CheckOtherStocks()
    {
        ///
    }
    public void CheckOtherBonds()
    {

    }
    private void CalculatDayGainLossPercent()
    {

    }

    private void CalculateDayGainLoss()
    {

    }
    private void CalculateTotalGainLossPercent()
    {

    }
    private void CalculateTotalGainLoss()
    {

    }
    //Analytics
    private void GeneratePortfolioReport()
    {


    }

    internal PortfolioSummary GetPortfolioSummary()
    {
        throw new NotImplementedException();
    }
}
