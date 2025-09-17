using System.Collections.Generic;
using UnityEngine;

public interface IStockTradingService
{
    //List<Stock> GetAvailableStocks();
   // Stock GetStockData(string stockId);
   // void CheckDescribeStocks();
   //TradeResult BuyStock(string stockId, int quantity);
   //TradeResult SellStock(string stockId, int quantity);
    // List<StockPosition> GetStockPortfolio();
    //decimal CalculateStockValue(string stockId, int quantity);
}

public class Stock
{
    public string StockName { get; }
    public string ID { get; }
    public float CurrentValue { get; }//цена
    public int Quantity{ get; }
    public float OpenPrice { get; }
    public float ClosePrice { get; }
    public float GainLoss{ get; }
    public float GainLossPercent { get; }
    public float AmountNextDiv { get; }
    public float PercenttNextDiv { get; }
    public string DateNextDiv { get; }
    public string LevelStability { get; }
    public float AverageDivYield{ get; }// средняя дивидендная доходность
    public string Sector { get; }
    public string Country { get; }

}
public class StockInfoPortfolio
{
    public Dictionary<string, int> Stocks = new();
    public int Quantity { get; }
    public float StocksSummaryValue { get; }
    public float CurrentValue { get; }

}

