using System.Collections.Generic;
using UnityEngine;

public interface IStockTradingService
{
    //void UpdateStockPrices(Dictionary<string, float> newPrices);

    //List<Stock> GetAvailableStocks();
    // Stock GetStockData(string stockId);
    // void CheckDescribeStocks();
    //TradeResult BuyStock(string stockId, int quantity);
    //TradeResult SellStock(string stockId, int quantity);
    // List<StockPosition> GetStockPortfolio();
    //decimal CalculateStockValue(string stockId, int quantity);
}

public class Stock: IAsset
{
    public string Ticker { get; set; }
    public Dictionary<string, int> Stocks = new();
    //public Company Company { get; } 
    public float OpenPrice { get; }
    public float ClosePrice { get; }
    public float GainLoss{ get; }
    public float GainLossPercent { get; }
    public float AmountNextDiv { get; }
    public float PercenttNextDiv { get; }
    public string DateNextDiv { get; }
    public string LevelStability { get; }
    public float AverageDivYield{ get; }// средняя дивидендная доходность


}


public enum LevelStability
    {
        High,
        Normal,
        Low
    }


