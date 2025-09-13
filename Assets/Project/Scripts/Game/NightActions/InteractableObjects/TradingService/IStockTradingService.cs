using System.Collections.Generic;
using UnityEngine;

public interface IStockTradingService
{
    List<Stock> GetAvailableStocks();
    Stock GetStockData(string stockId);
   // TradeResult BuyStock(string stockId, int quantity);
   // TradeResult SellStock(string stockId, int quantity);
   // List<StockPosition> GetStockPortfolio();
    //decimal CalculateStockValue(string stockId, int quantity);
}

public class Stock: MonoService
{
    public string ID { get; }
    public string CompanyName { get; }
    public float StockPrice { get; }
    public string Sector { get; }
    public float DayHigh { get; }
    public float DayLow { get; }
}

public class StockInfoPortfolio: MonoService
{
    public string StockID { get; }
    public int Quantity { get; }
    public float CurrentValue { get; }
    public float GainLoss{ get; }
    public float GainLossPercent { get; }


}

