using UnityEngine;
using System.Collections.Generic;
using MyGame.Enums;
using System;
using Unity.VisualScripting;
using System.Linq;

public abstract class SampleActiv : IActiv
{
    public abstract Ticker Ticker { get; }
    public abstract object Config { get; }
    public float CurrentValue { get; }
    private int quantity;

    public int Quantity => quantity;

    public void AddQuantity(int amount)
    {
        quantity += amount;
    }

    public void RemoveQuantity(int amount)
    {
        quantity -= amount;
    }

}

public class PortfollioService : MonoService // IPortfolioService
{
    private PortfolioSummary _portfolioSummary = new PortfolioSummary();
    private Mediator _mediator;
    public override List<Type> requiredServices { get; protected set; } = new List<Type>();
    public Dictionary<Ticker, Stock> AvailableStocks { get; private set; }
    public Dictionary<Ticker, Bond> AvailableBonds { get; private set; }
    //public IReadOnlyDictionary<Ticker, Stock> Stocks => AvailableStocks;
    ///public IReadOnlyDictionary<Ticker, Bond> Bondds => AvailableBonds;

    private Dictionary<Ticker, IActiv> AvailableAssets { get; set; } = new();
    public IReadOnlyDictionary<Ticker, IActiv> Assets => AvailableAssets;

    public Action<PortfolioSummary> OnPortfolioUpdated { get; internal set; }

    private void Awake()
    {
        Mediator.Instance.RegisterService(this);
    }

    public void AddNewAsset(IActiv newAsset)//добавление нового актива
    {
        if (newAsset == null || AvailableAssets.ContainsKey(newAsset.Ticker))

            return;

        AvailableAssets.Add(newAsset.Ticker, newAsset);
        _mediator.GlobalEventBus.Publish(new AssetListChangedEvent(newAsset.Ticker));
    }
    public void LoadInitialData()//логика сохранения и инициализации
    {
        //AvailableStocks = new Dictionary<Ticker, Stock>();
        //AvailableBonds = new Dictionary<Ticker, Bond>();
        AvailableAssets = new Dictionary<Ticker, IActiv>();

        //загрузка активов портфолио
    }


    // public PortfolioSummary GetPortfolioSummary()//отображение портфеля
    //{

    // return _portfolioSummary;

    // }

    public float GetAssetPrice(Ticker ticker)//поиск цены по тикеру
    {
        SampleActiv activ = null;

        AvailableStocks.TryGetValue(ticker, out Stock stock);
        activ = stock.IsUnityNull() ? activ : stock;

        AvailableBonds.TryGetValue(ticker, out Bond bond);
        activ = bond.IsUnityNull() ? activ : bond;

        return activ.CurrentValue;
    }


    //кнопки быстрой продажи покупки

    #region BuyActiv

    public void BuyAsset(Type assetType, Ticker ticker, int quantity, float totalCost)
    {
        _portfolioSummary.CashBalance -= totalCost;
        _portfolioSummary.MyActives[ticker].AddQuantity(quantity);

        if (assetType == typeof(Stock))
        {
            _portfolioSummary.StocksValue += totalCost;
            _portfolioSummary.CountStocks += quantity;
        }

        if (assetType == typeof(Bond))
        {
            _portfolioSummary.BondsValue += totalCost;
            _portfolioSummary.CountBonds += quantity;
        }

        //OnPortfolioUpdated?.Invoke(_portfolioSummary);
    }
    #endregion

    #region SellActiv
    public bool SellAsset(Type assetType, Ticker ticker, int quantity, float totalCost)
    {
        _portfolioSummary.CashBalance += totalCost;
        _portfolioSummary.MyActives[ticker].RemoveQuantity(quantity);

        if (_portfolioSummary.MyActives[ticker].Quantity <= 0)
        {
            _portfolioSummary.MyActives.Remove(ticker);
        }

        if (assetType == typeof(Stock))
        {
            _portfolioSummary.StocksValue -= totalCost;
            _portfolioSummary.CountStocks -= quantity;
        }

        if (assetType == typeof(Bond))
        {
            _portfolioSummary.BondsValue -= totalCost;
            _portfolioSummary.CountBonds -= quantity;
        }

        return true;

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

    public bool HasEnoughCash(float totalCost)//заменить на int
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

    public IActiv GetAssetByTicker(Ticker ticker)// поиск актива по тикеру
    {

        if (AvailableStocks.TryGetValue(ticker, out Stock stock))
        {
            return stock;
        }

        if (AvailableBonds.TryGetValue(ticker, out Bond bond))
        {
            return bond;
        }

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

    internal PortfolioSummary GetPortfolioSummary()
    {
        throw new NotImplementedException();
    }
}
