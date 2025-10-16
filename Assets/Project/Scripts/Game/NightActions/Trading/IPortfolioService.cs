using System.Collections.Generic;
using MyGame.Enums;
using System;

public interface IPortfolioService
{
    //операции с активами
    bool TradeAssets(TradeType tradeType, IActiv asset, int quantity);
    void AddCash(int amount);
    //покупка иных
    void CheckOtherStocks();
    void CheckOtherBonds();
    void CalculatDayGainLossPercent();
    void CalculateDayGainLoss();
    void CalculateTotalGainLossPercent();
    void CalculateTotalGainLoss();
    //Analytics
    void GeneratePortfolioReport();
}

public class PortfolioSummary
{//данные для отображения

    private readonly Dictionary<Ticker, IActiv> _MyActives = new();
    public IReadOnlyDictionary<Ticker, IActiv> MyActives => _MyActives;
    private int _countStocks;
    private int _countBonds;
    private int _cashBalance;
    public int CashBalance =>_cashBalance;
    private int StocksValue;
    private int BondsValue;
    private int TotalValue => StocksValue + BondsValue + CashBalance;
    //public float TotalGainLoss;
    //public float TotalGainLossPercent;
    //public float DayGainLoss;
    // public float DayGainLossPercent;
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
                StocksValue += costAdjustment;
                _countStocks += quantityAdjustment;
                break;

            case Type t when t == typeof(Bond):
                BondsValue += costAdjustment;
                _countBonds += quantityAdjustment;
                break;

            default:
                //ColorfulDebug.LogRed($"Неизвестный тип актива: {assetType}");
                break;

        }

    }
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