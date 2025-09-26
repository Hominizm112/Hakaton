using UnityEngine;
using System.Collections.Generic;
using MyGame.Enums;
using System;
using Unity.VisualScripting;
using System.Linq;

public interface IActiv { }

public abstract class SampleActiv : IActiv
{
    public float CurrentValue;
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

public class PortfollioService : MonoService, IPortfolioService
{
    private PortfolioSummary _portfolioSummary = new PortfolioSummary();
    private Mediator _mediator;


    public override List<Type> requiredServices { get; protected set; } = new List<Type>();


    public Dictionary<Ticker, Stock> AvailableStocks { get; private set; }


    //все доступные акции из списка
    public Dictionary<Ticker, Bond> AvailableBonds { get; private set; }


    //событие об успешной покупке/продаже
    public event Action<PortfolioSummary> OnPortfolioUpdated;


    private void Awake()
    {
        Mediator.Instance.RegisterService(this);
    }


    public override void Initialize(Mediator mediator = null)
    {
        base.Initialize();

        mediator.GlobalEventBus.Subscribe<TradeRequestEvent>(HandleTradeRequest);

        LoadInitialData();
    }

    public void LoadInitialData()//логика сохранения и инициализации
    {
        AvailableStocks = new Dictionary<Ticker, Stock>();
        AvailableBonds = new Dictionary<Ticker, Bond>();
        //загрузка активов портфолио
    }

    private void HandleTradeRequest(TradeRequestEvent @event)
    {
        // поиск и покупка нужного объекта
        IActiv activ = FindAssetByTicker(@event.Ticker);

        if (!activ.IsUnityNull())
        {
            TradeAssets(@event.TradeType, activ, @event.Quantity);
        }
    }

    private IActiv FindAssetByTicker(Ticker ticker)
    {
        if (AvailableStocks.TryGetValue(ticker, out Stock stock))
        {
            return stock;
        }

        if (AvailableBonds.TryGetValue(ticker, out Bond bond))
        {
            return bond;
        }
        return null;
    }


    public PortfolioSummary GetPortfolioSummary()//отображение портфеля
    {

        return _portfolioSummary;

    }

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
    public bool TradeAssets(TradeType tradeType, IActiv asset, int quantity)
    {
        if (asset == null || quantity <= 0)
        {
            _mediator.GlobalEventBus.Publish<DebugLogErrorEvent>(new("Неверные параметры торговой операции"));
            return false;
        }

        float assetPrice;
        Ticker ticker;
        Type assetType;

        // определение типа актива
        if (asset is Stock stock)
        {
            assetPrice = stock.CurrentValue;
            if (!stock.StockInfo.IsUnityNull())
            {
                ticker = stock.StockInfo.Ticker;
                assetType = typeof(Stock);
            }
            else
            {
                _mediator.GlobalEventBus.Publish<DebugLogErrorEvent>(new("Информация об активе отсутствует"));
                return false;
            }
        }
        else if (asset is Bond bond)
        {
            assetPrice = bond.CurrentValue;
            if (!bond.BondInfo.IsUnityNull())
            {
                ticker = bond.BondInfo.Ticker;
                assetType = typeof(Bond);
            }
            else
            {
                _mediator.GlobalEventBus.Publish<DebugLogErrorEvent>(new("Информация об активе отсутствует"));
                return false;
            }
        }
        else
        {
            _mediator.GlobalEventBus.Publish<DebugLogErrorEvent>(new("Неподдерживаемый тип актива"));
            return false;
        }

        float totalCost = quantity * assetPrice;

        switch (tradeType)
        {
            case TradeType.Buy:
                return BuyAsset(assetType, ticker, quantity, totalCost);
            case TradeType.Sell:
                return SellAsset(assetType, ticker, quantity, totalCost);
            default:
                break;
        }

        _mediator.GlobalEventBus.Publish<DebugLogErrorEvent>(new("Неподдерживаемый тип операции"));
        return false;
    }

    #region BuyActiv


    private bool BuyAsset(Type assetType, Ticker ticker, int quantity, float totalCost)
    {
        if (_portfolioSummary.CashBalance < totalCost)
        {
            _mediator.GlobalEventBus.Publish<DebugLogErrorEvent>(new("Недостаточно средств для покупки"));
            return false;
        }

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

        OnPortfolioUpdated?.Invoke(_portfolioSummary);
        ColorfulDebug.LogGreen($"Успешная покупка {ticker}");

        return true;
    }
    #endregion


    #region SellActiv
    private bool SellAsset(Type assetType, Ticker ticker, int quantity, float totalCost)
    {
        if (assetType == typeof(Stock))
        {

            if (!_portfolioSummary.MyActives.ContainsKey(ticker) || _portfolioSummary.MyActives[ticker].Quantity < quantity)
            {
                _mediator.GlobalEventBus.Publish<DebugLogErrorEvent>(new($"Недостаточно акций {ticker} для продажи."));
                return false;
            }
        }
        if (assetType == typeof(Bond))
        {

            if (!_portfolioSummary.MyActives.ContainsKey(ticker) || _portfolioSummary.MyActives[ticker].Quantity < quantity)
            {
                _mediator.GlobalEventBus.Publish<DebugLogErrorEvent>(new($"Недостаточно облигаций {ticker} для продажи."));
                return false;
            }
        }

        _portfolioSummary.CashBalance += totalCost;

        if (assetType == typeof(Stock))
        {


            _portfolioSummary.MyActives[ticker].RemoveQuantity(quantity);
            if (_portfolioSummary.MyActives[ticker].Quantity <= 0)
            {
                _portfolioSummary.MyActives.Remove(ticker);
            }
            _portfolioSummary.StocksValue -= totalCost;
            _portfolioSummary.CountStocks -= quantity;
        }

        if (assetType == typeof(Bond))
        {


            _portfolioSummary.MyActives[ticker].RemoveQuantity(quantity);
            if (_portfolioSummary.MyActives[ticker].Quantity <= 0)
            {
                _portfolioSummary.MyActives.Remove(ticker);
            }
            _portfolioSummary.BondsValue -= totalCost;
            _portfolioSummary.CountBonds -= quantity;
        }

        OnPortfolioUpdated?.Invoke(_portfolioSummary);
        ColorfulDebug.LogGreen($"Продажа {ticker} успешна. Продано {quantity} штук.");
        return true;

    }
    #endregion

    // пополнение баланса кошелька приложения
    public void AddCash(int amount)
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
            _portfolioSummary.CashBalance += amount;

        }
        //недостаточно средств???

    }

    public IActiv GetAssetByTicker(Ticker ticker)// поиск актива по тикеру
    {
        if (AvailableStocks.TryGetValue(ticker, out Stock stock))
        {
            return stock;
        }

        else if (AvailableBonds.TryGetValue(ticker, out Bond bond))
        {
            return bond;
        }

        else
        {
            _mediator.GlobalEventBus.Publish<DebugLogErrorEvent>(new("Актива с заданным тикером не существует"));
            return null;
        }
    }
    //покупка иных
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
