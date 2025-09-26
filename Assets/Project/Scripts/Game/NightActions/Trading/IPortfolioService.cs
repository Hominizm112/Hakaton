using UnityEngine;
using System.Collections.Generic;
using MyGame.Enums;

public interface IPortfolioService
{
    //операции с активами
    bool TradeAssets(TradeType tradeType, IActiv asset, int quantity);
    void AddCash(int amount);
    //покупка иных
    //void CheckOtherStocks();
    //void CheckOtherBonds();
    void CalculatDayGainLossPercent();
    void CalculateDayGainLoss();
    void CalculateTotalGainLossPercent();
    void CalculateTotalGainLoss();
    //Analytics
    void GeneratePortfolioReport();

}

public class PortfolioSummary
{//данные для отображения
    public int CountStocks;
    public Dictionary<Ticker, SampleActiv> MyActives = new();
    public int CountBonds;
    public float CashBalance;
    public float StocksValue;
    public float BondsValue;
    public float TotalValue => StocksValue + BondsValue + CashBalance;
    public float TotalGainLoss;
    public float TotalGainLossPercent;
    public float DayGainLoss;
    public float DayGainLossPercent;
    public PortfolioSummary()
    {


    }

}