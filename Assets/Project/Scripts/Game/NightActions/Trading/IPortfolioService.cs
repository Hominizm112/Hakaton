using System.Collections.Generic;
using MyGame.Enums;
using System;
public interface IPortfolioDisplayData
{
    int CountStocks { get; }
    int CountBonds { get; }
    int CashBalance { get; }
    int StocksValue { get; }
    int BondsValue { get; }
    int TotalValue { get; }
    IReadOnlyDictionary<Ticker, IActiv> MyActives { get; }
}
public interface IPortfolioService
{
    void AddQuantity(Ticker ticker, int quantity);
    void RemoveQuantity(Ticker ticker, int quantity);
    void AddMyActive(Ticker ticker, IActiv newAsset);
    void RemoveMyActive(Ticker ticker);
    void RecalculateValueCount(Type assetType, TradeType TypeOperation, int totalCost, int quantity);
    void AddCashBalance(int amount);
    int UpdateCashBalance();
    void RecalculateCashBalance(TradeType TypeOperation, int totalCost);
}

public class PortfolioSummary: IPortfolioService, IPortfolioDisplayData
{
    private readonly Dictionary<Ticker, IActiv> _MyActives = new();
    public IReadOnlyDictionary<Ticker, IActiv> MyActives => _MyActives;
    private PortfollioService _model;
    private int _countStocks;
    private int _countBonds;
    private int _cashBalance;
    private int _stocksValue;
    private int _bondsValue;
    private int _totalValue => StocksValue + BondsValue + CashBalance;
    public int CountBonds => _countBonds;
    public int CountStocks=> _countStocks;
    public int CashBalance => _cashBalance;
    public int StocksValue => _stocksValue;
    public int BondsValue => _bondsValue;
    public int TotalValue => _totalValue;
    
    //public float TotalGainLoss;
    //public float TotalGainLossPercent;
    //public float DayGainLoss;
    // public float DayGainLossPercent;
    #region UpdateQuantity
    public void AddQuantity(Ticker ticker, int quantity)
    {
        if (MyActives.TryGetValue(ticker, out IActiv existingAsset))
        {
            if (existingAsset is SampleActiv<IAssetConfig> activModel)
            {
                activModel.AddQuantity(quantity);
                //уведомить Presenter об изменении
                // NotifyPortfolioChanged();
                return;
            }

            throw new InvalidOperationException($"Объект с тикером {ticker} в портфеле не является классом Модели SampleActiv.");
        }
        throw new KeyNotFoundException($"Актив с тикером {ticker} не найден в портфеле игрока.");
    }

    public void RemoveQuantity(Ticker ticker, int quantity)
    {
        if (MyActives.TryGetValue(ticker, out IActiv existingAsset))
        {
            if (existingAsset is SampleActiv<IAssetConfig> activModel)
            {
                activModel.RemoveQuantity(quantity);
                //уведомить Presenter об изменении
                // NotifyPortfolioChanged();
                return;
            }

            throw new InvalidOperationException($"Объект с тикером {ticker} в портфеле не является классом Модели SampleActiv.");
        }
        throw new KeyNotFoundException($"Актив с тикером {ticker} не найден в портфеле игрока.");
    }


    public void AddMyActive(Ticker ticker, IActiv newAsset)
    {
        if (newAsset == null)
        {
            return;
        }

        _MyActives.Add(newAsset.Ticker, newAsset);
    }

    public void RemoveMyActive(Ticker ticker)
    {

        if (!_MyActives.TryGetValue(ticker, out IActiv asset))
        {
            return;
        }

        _MyActives.Remove(ticker);

    }
    #endregion

    public void RecalculateValueCount(Type assetType, TradeType TypeOperation, int totalCost, int quantity)
    {
        int factor = TypeOperation switch
        {
            TradeType.Buy => 1,
            TradeType.Sell => -1,
            _ => 0

        };

        int costAdjustment = totalCost * factor;

        int quantityAdjustment = quantity * factor;

        switch (assetType)
        {
            case Type t when t == typeof(Stock):
                _stocksValue += costAdjustment;
                _countStocks += quantityAdjustment;
                break;

            case Type t when t == typeof(Bond):
                _bondsValue += costAdjustment;
                _countBonds += quantityAdjustment;
                break;

            default:
                //ColorfulDebug.LogRed($"Неизвестный тип актива: {assetType}");
                break;
        }

    }

    public void ValueActivInitialState(int NewBondValue, int NewStockValue)
    {
        _bondsValue = NewBondValue;
        _stocksValue = NewStockValue;
    }


    public void PortfolioInitialState()
    {
        IActiv newAsset = _model.CreateAssetInstance(typeof(Stock),Ticker.SRV, 200, 1);
        _countStocks = 1;
        _countBonds = 0;
        _stocksValue = 200;
        _bondsValue = 0;
        AddMyActive(Ticker.SRV, newAsset);
    }

    #region UpdateCashBalance
    public void AddCashBalance(int amount)
    {
        _cashBalance += amount;

    }
    public int UpdateCashBalance()
    {
        return CashBalance;

    }

    public void RecalculateCashBalance(TradeType TypeOperation, int totalCost)
    {
        int factor = TypeOperation switch
        {
            TradeType.Buy => 1,
            TradeType.Sell => -1,
            _ => 0

        };
        int cashAdjustment = totalCost * (-factor);
        _cashBalance += cashAdjustment;
    }

}
#endregion