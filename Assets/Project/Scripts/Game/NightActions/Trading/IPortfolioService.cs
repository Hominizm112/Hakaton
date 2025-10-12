using UnityEngine;
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
    private readonly Dictionary<Ticker, SampleActiv> _MyActives = new();
    public IReadOnlyDictionary<Ticker, SampleActiv> MyActives => _MyActives;
    public int CountStocks;
    public int CountBonds;
    public int CashBalance;
    public int StocksValue;
    public int BondsValue;
    public int TotalValue => StocksValue + BondsValue + CashBalance;
    //public float TotalGainLoss;
    //public float TotalGainLossPercent;
    //public float DayGainLoss;
    // public float DayGainLossPercent;
    public void AddMyActive(Ticker ticker,SampleActiv newAsset)
    {
        if (newAsset == null)
        {
            return;
        }

        _MyActives.Add(newAsset.Ticker, newAsset);
    }

    public void RemoveMyActive(Ticker ticker)
    {

        if (!_MyActives.TryGetValue(ticker, out SampleActiv asset))
        {
            return;
        }

        _MyActives.Remove(ticker);

    }

    public void RecalculateValueCount(Type assetType, TradeType TypeOperation, int totalCost, int quantity)
    {//true=buy

        int factor = TypeOperation switch
        {
            TradeType.Buy => 1,
            TradeType.Sell => -1,
            _ => 0

        };
        
        int costAdjustment = totalCost * factor;

        int quantityAdjustment = quantity * (int)factor; 


        switch (assetType)
        {
            case Type t when t == typeof(Stock):
                StocksValue += costAdjustment;
                CountStocks += quantityAdjustment;
                break;

            case Type t when t == typeof(Bond):
                BondsValue += costAdjustment;
                CountBonds += quantityAdjustment;
                break;

            default:
                //ColorfulDebug.LogRed($"Неизвестный тип актива: {assetType}");
                break;

        }

    }

    public void RecalculateCashBalance(TradeType TypeOperation, int totalCost)//<--int
    {
        int factor = TypeOperation switch
        {
            TradeType.Buy => 1,
            TradeType.Sell => -1,
            _ => 0

        };
        int cashAdjustment = totalCost * (-factor); 
        CashBalance += cashAdjustment;
    }


}