using System.Collections.Generic;
using UnityEngine;

public class IStockTradingService
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

public class Stock//одна акция в портфеле
{
    public string Ticker { get; set; }
    public float CurrentValue { get; set; }
    //public Company Company { get; } 
    public float OpenPrice { get; }
    public float ClosePrice { get; }
    public float GainLossDay { get; }
    public float GainLossPercentDay { get; }
    public float AmountNextDiv { get; }//(фикс)
    public float PercenttNextDiv { get; }//(фикс)
    public string DateNextDiv { get; }//(фикс)
    public string LevelStability { get; }//(фикс)
    public float AverageDivYield { get; }// средняя дивидендная доходность(фикс)
    public string Country;
    public string Sector;


}

public enum LevelStability
    {
        High,
        Normal,
        Low
    }


