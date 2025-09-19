using UnityEngine;
using System.Collections.Generic;
using MyGame.Enums;

public interface IPortfolioService
{
    PortfolioSummary GetPortfolioSummary();
    //операции с активами
    bool TradeAssets(TradeType tradeType, object asset, int quantity);
    void AddCash(float amount);
    //float GetCashBalance();
    //покупка иных
    void CheckOtherStocks();
    void CheckOtherBonds();
    //Analytics
    void GeneratePortfolioReport();
   
}

public class PortfolioSummary
{//данные для отображения
    public int CountStocks{ get; set; }
    public Dictionary<string, int> MyStocks = new();
    public int CountBonds{ get; set; }
    public Dictionary<string, int> MyBonds = new();

    public float CashBalance { get; set; }
    public float StocksValue { get;set; }
    public float BondsValue { get;set; }
    public float TotalValue => StocksValue + BondsValue + CashBalance ;
    public float TotalGainLoss { get;set; }
    public float DayGainLoss { get;set; }
    
}