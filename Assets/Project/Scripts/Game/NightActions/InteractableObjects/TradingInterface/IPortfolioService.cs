using UnityEngine;
using System.Collections.Generic;

public interface IPortfolioService
{
    PortfolioSummary GetPortfolioSummary();
    //проверка цены
    void UpdateStockPrices(Dictionary<string, float> newPrices);
    void UpdateBondPrices(Dictionary<string, float> newPrices);
    //операции с активами
    void BuyStock(string symbol, int quantity, float price);
    void SellStock(string symbol, int quantity, float price);
    void BuyBond(string isin, int quantity, float price);
    void SellBond(string isin, int quantity, float price);
    void AddCash(float amount);
    //покупка иных
    void CheckOtherStocks();
    void CheckOtherBonds();
    //Analytics
    void GeneratePortfolioReport();
}

public class PortfolioSummary
{
    public float CashBalance;
    public float StocksValue { get;}
    public float BondsValue { get;}
    public float TotalValue => StocksValue + BondsValue;
    public float TotalGainLoss { get; }
    public float DayGainLoss { get; }
}