using UnityEngine;
using System.Collections.Generic;
using MyGame.Enums;
using System;
using System.Linq;
using UnityEditor.VersionControl;

public abstract class SampleActiv : IActiv
{
    public abstract Ticker Ticker { get; }
    public abstract object Config { get; }
    public int CurrentValue { get; protected set; } //<-- int
    private int _quantity;
    public int Quantity => _quantity;

    public void AddQuantity(int amount)
    {
        _quantity += amount;
    }

    public void RemoveQuantity(int amount)
    {
        _quantity -= amount;
    }

    public SampleActiv(int initialValue, int initialQuantity)
    {
        CurrentValue = initialValue;
        //_quantity = initialQuantity;
    }

}

public class PortfollioService : MonoService // IPortfolioService
{

    private PortfolioSummary _portfolioSummary = new PortfolioSummary();
    private Mediator _mediator;
    public override List<Type> requiredServices { get; protected set; } = new List<Type>();
    //public Dictionary<Ticker, Stock> AvailableStocks { get; private set; }
    //public Dictionary<Ticker, Bond> AvailableBonds { get; private set; }
    private Dictionary<Ticker, IActiv> AvailableAssets { get; set; } = new();
    public IReadOnlyDictionary<Ticker, IActiv> Assets => AvailableAssets;

    private void Awake()
    {
        Mediator.Instance.RegisterService(this);
    }

    public void LoadInitialData()//логика сохранения и инициализации
    {
       // AvailableStocks = new Dictionary<Ticker, Stock>();
        //AvailableBonds = new Dictionary<Ticker, Bond>();
        AvailableAssets = new Dictionary<Ticker, IActiv>();

        //загрузка активов портфолио
    }


    // public PortfolioSummary GetPortfolioSummary()//отображение портфеля
    //{

    // return _portfolioSummary;

    // }

    public int GetAssetPrice(Ticker ticker)//поиск цены по тикеру
    {
        SampleActiv activ = null;

        _portfolioSummary.MyActives.TryGetValue(ticker, out SampleActiv stock);
        if (stock != null)
        {
            activ = stock;
        }
        //AvailableStocks.TryGetValue(ticker, out Stock stock);
            //activ = stock.IsUnityNull() ? activ : stock;

        //_portfolioSummary.MyActives.TryGetValue(ticker, out SampleActiv bond);
        //AvailableBonds.TryGetValue(ticker, out Bond bond);
        ///activ = bond.IsUnityNull() ? activ : bond;

        return activ.CurrentValue;
    }

    public int GetQuantityByTicker(Ticker targetTicker)//возврат поля количества по тикеру
    {

        if (_portfolioSummary.MyActives.TryGetValue(targetTicker, out SampleActiv asset))
        {
            return asset.Quantity;
        }
        else
        {
            return 0;
        }
    }

    public void UpdatePortfolioValue(Type assetType, int totalCost, int quantity,bool TypeOperation)
    {
        _portfolioSummary.RecalculateValueCount(assetType, false, totalCost, quantity);
        _portfolioSummary.RecalculateCashBalance(false, totalCost);


    }

    //кнопки быстрой продажи покупки

    #region BuyActiv

    public BuyTransactionState BuyAsset(Type assetType, Ticker ticker,int  price,int quantity)//<int
    {
        
        if ()// ??если нету в списке всех доступных активов
        {
            return BuyTransactionState.NotEnough;
        }

        if (_portfolioSummary.MyActives.TryGetValue(ticker, out SampleActiv asset))
        {
            _portfolioSummary.MyActives[ticker].AddQuantity(quantity);
            return BuyTransactionState.NoNeedCreatedButton;
        }
        else
        {
            SampleActiv newAsset = CreateAssetInstance(assetType, ticker, price, quantity);
            _portfolioSummary.AddMyActive(ticker,newAsset);
            return BuyTransactionState.NeedCreatedButton;
        }
        
    }
    public SampleActiv CreateAssetInstance(Type assetType, Ticker ticker, int price, int quantity)
    {
        IAssetConfig baseConfig = MarketData.GetAssetConfig(ticker);
        SampleActiv asset = null;

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


    #region SellActiv
    public SellTransactionState SellAsset(Type assetType, Ticker ticker, int quantity, int totalCost)
    {
        if (!HasEnoughQuantityActive(ticker, quantity))
        {
            return SellTransactionState.NotEnough;
        }

        IActiv asset = GetAssetByTicker(ticker);

        if (asset.Quantity <= 0)//asset?
        {
            _portfolioSummary.RemoveMyActive(ticker);
            return SellTransactionState.NeedRemovedButton; //удаление кнопки
        }
        return SellTransactionState.NoNeedRemovedButton;
    }

    #endregion

    public bool HasEnoughQuantityActive(Ticker ticker, int quantity)
    {
        if (!_portfolioSummary.MyActives.ContainsKey(ticker) || _portfolioSummary.MyActives[ticker].Quantity < quantity)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    public bool HasEnoughCash(int totalCost)
    {
        if (_portfolioSummary.CashBalance < totalCost)
        {
            return false;
        }
        else
        {
            return true;
        }

    }
    //пополнение баланса кошелька приложения
    public void AddCash(int amount)
    {
        _portfolioSummary.CashBalance += amount;
    }


    public int  GetAvailableCash()
    {
        return _portfolioSummary.CashBalance;
    }


    public IActiv GetAssetByTicker(Ticker ticker)// поиск актива по тикеру
    {

        if (_portfolioSummary.MyActives.TryGetValue(ticker, out SampleActiv activ))
        {
            return activ;
        }

        // if (AvailableBonds.TryGetValue(ticker, out Bond bond))
        //{
        //  return bond;
        //}

        _mediator.GlobalEventBus.Publish<DebugLogErrorEvent>(new("Актива с заданным тикером не существует"));
        return null;

    }

    //покупка иных
    private void CheckOtherStocks()
    {
        ///
    }
    private void CheckOtherBonds()
    {

    }
    public void CalculatDayGainLossPercent()
    {

    }

    public void CalculateDayGainLoss()
    {

    }
    public void CalculateTotalGainLossPercent()
    {

    }
    public void CalculateTotalGainLoss()
    {

    }
    //Analytics
    public void GeneratePortfolioReport()
    {


    }
}
    
