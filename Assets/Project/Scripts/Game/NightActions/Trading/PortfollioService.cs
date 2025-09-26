using UnityEngine;
using System.Collections.Generic;
using MyGame.Enums;
using System;

public class PortfollioService : MonoService, IPortfolioService
{
    private PortfolioSummary _portfolioSummary = new PortfolioSummary();
    public override List<Type> requiredServices { get; protected set; } = new List<Type>();
    public Dictionary<string, Stock> AvailableStocks { get; private set; }
    public Dictionary<string, Bond> AvailableBonds { get; private set; }//все доступные акции из списка
    public event Action<PortfolioSummary> OnPortfolioUpdated;//событие об успешной покупке/продаже
    private Mediator _mediator;
    private void Awake()
    {
        Mediator.Instance.RegisterService(this);
    }
    public override void Initialize(Mediator mediator = null)
    {
        base.Initialize(mediator);
        if (mediator != null && mediator.GlobalEventBus != null)
        {
            mediator.GlobalEventBus.Subscribe<TradeRequestEvent>(HandleTradeRequest);
        }
        LoadInitialData();
    }
    public void LoadInitialData()//логика сохранения и инициализации
    {
        AvailableStocks = new Dictionary<string, Stock>();
        AvailableBonds = new Dictionary<string, Bond>();
        //загрузка активов портфолио
    }

    private void HandleTradeRequest(TradeRequestEvent e)
    {
        // поиск и покупка нужного объекта
        object asset = FindAssetByTicker(e.Ticker);

        if (asset != null)
        {
            TradeAssets(e.TradeType, asset, e.Quantity);
        }
    }

    private object FindAssetByTicker(string ticker)
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
    public float GetAssetPrice(string ticker)//поиск цены по тикеру
    {

        if (AvailableStocks.TryGetValue(ticker, out Stock stock))

        {

            return stock.CurrentValue;

        }

        if (AvailableBonds.TryGetValue(ticker, out Bond bond))

        {
            return bond.CurrentValue;
        }


        return 0;
  }
  //кнопки быстрой продажи покупки
    public bool TradeAssets(TradeType tradeType, object asset, int quantity)
    {
        if (asset == null || quantity <= 0)
        {
            _mediator.GlobalEventBus.Publish<DebugLogErrorEvent>(new("Неверные параметры торговой операции"));
            return false;
        }

        float assetPrice;
        Ticker ticker;
        bool isStock = false;

        // определение типа актива
        if (asset is Stock stock)
        {
            assetPrice = stock.CurrentValue;
            if (stock.StockInfo != null)
            {
                ticker = stock.StockInfo.Ticker;
                isStock = true;
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
            if (bond.BondInfo != null)
            {
                ticker = bond.BondInfo.Ticker;
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
        
        if (tradeType == TradeType.Buy)
        {
            return BuyAsset(isStock, ticker, quantity, totalCost);
        }
        else if (tradeType == TradeType.Sell)
        {
            return SellAsset(isStock, ticker, quantity, totalCost);
        }
        _mediator.GlobalEventBus.Publish<DebugLogErrorEvent>(new("Неподдерживаемый тип операции"));
        return false;
    }
    #region BuyActiv
        private bool BuyAsset(bool isStock, Ticker ticker, int quantity, float totalCost)
        {
        if (_portfolioSummary.CashBalance < totalCost)
        {
            _mediator.GlobalEventBus.Publish<DebugLogErrorEvent>(new("Недостаточно средств для покупки"));
            return false;
        }
        _portfolioSummary.CashBalance -= totalCost;

        if (isStock)
        {
            if (_portfolioSummary.MyStocks.ContainsKey(ticker))//существующий актив
            {
                _portfolioSummary.MyStocks[ticker] += quantity;
            }
            else
            {
                _portfolioSummary.MyStocks.Add(ticker, quantity);
            }
            _portfolioSummary.StocksValue += totalCost;
            _portfolioSummary.CountStocks += quantity;
        }
        else
        {
            if (_portfolioSummary.MyBonds.ContainsKey(ticker))
            {
                _portfolioSummary.MyBonds[ticker] += quantity;
            }
            else
            {
                _portfolioSummary.MyBonds.Add(ticker, quantity);
            }
            _portfolioSummary.BondsValue += totalCost;
            _portfolioSummary.CountBonds += quantity;
        }
        OnPortfolioUpdated?.Invoke(_portfolioSummary);
        Debug.Log($"Успешная покупка {ticker}");
        return true;
    }
#endregion BuyActiv
#region SellActiv
private bool SellAsset(bool isStock, Ticker ticker, int quantity, float totalCost)
{
    if (isStock)
    {
        if (!_portfolioSummary.MyStocks.ContainsKey(ticker) || _portfolioSummary.MyStocks[ticker] < quantity)
        {
            _mediator.GlobalEventBus.Publish<DebugLogErrorEvent>(new($"Недостаточно акций {ticker} для продажи."));
            return false;
        }
    }
    else
    {
        if (!_portfolioSummary.MyBonds.ContainsKey(ticker) || _portfolioSummary.MyBonds[ticker] < quantity)
        {
           _mediator.GlobalEventBus.Publish<DebugLogErrorEvent>(new($"Недостаточно облигаций {ticker} для продажи."));
            return false;
        }
    }

    _portfolioSummary.CashBalance += totalCost;
    if (isStock)
    {
        _portfolioSummary.MyStocks[ticker] -= quantity;
        if (_portfolioSummary.MyStocks[ticker] <= 0)
        {
            _portfolioSummary.MyStocks.Remove(ticker);
        }
        _portfolioSummary.StocksValue -= totalCost;
        _portfolioSummary.CountStocks -= quantity;
    }
    else
    {
        _portfolioSummary.MyBonds[ticker] -= quantity;
        if (_portfolioSummary.MyBonds[ticker] <= 0)
        {
            _portfolioSummary.MyBonds.Remove(ticker);
        }
        _portfolioSummary.BondsValue -= totalCost;
        _portfolioSummary.CountBonds -= quantity;
    }
    OnPortfolioUpdated?.Invoke(_portfolioSummary);
    Debug.Log($"Продажа {ticker} успешна");
    return true;
}
    #endregion SellActiv

    public void AddCash(float amount)//пополнение баланса
    {
        if (amount <= 0)
        {
            _mediator.GlobalEventBus.Publish<DebugLogErrorEvent>(new("Некорректный ввод"));
        }
        else if (amount > 100000f)
        {
           _mediator.GlobalEventBus.Publish<DebugLogErrorEvent>(new("Достигнут лимит пополнения"));
        }
        //недостаточно средств???
        else
        {
            _portfolioSummary.CashBalance += amount;
        }
    }
   public object GetAssetByTicker(string ticker)// поиск актива по тикеру
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
    public void CalculatDayeGainLossPercent()
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

