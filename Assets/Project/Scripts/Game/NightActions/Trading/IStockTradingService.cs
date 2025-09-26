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
    public StockConstInfo StockInfo{ get; set; }
    public float CurrentValue { get; set; }
    public float OpenPrice { get; }
    public float ClosePrice { get; }
    public float GainLossDay { get; }
    public float GainLossPercentDay { get; }
    public string DateNextDiv { get; }
}
public class StockConstInfo
{
    public Ticker Ticker { get; set; }
    public LevelStability LevelStability { get; }//(фикс)
    public Country Country { get; }
    public Sector Sector { get; }
    public float PercenttNextDiv { get; }//(фикс)
    public float AverageDivYield { get; }// средняя дивидендная доходность(фикс)
    public float AmountNextDiv { get; }//(фикс)
}
public enum Ticker
{
    //акции
    GDC,
    SRV,
    GWI,
    TSW,
    VRD,
    AEON,
    EGC,
    CSD,
    RRT,
    CHAI,
    GNM,
    //облигации
    UA0000F8132,//F=Financial
    UA0000С8000,
    CH0000E9010,
    CH0000I7878,
    CH0000I0001,
    CH0000H0004,
    CH0000F7777,
    GE0000T0009,
    GE0000I9090,
    GE0000E6789,
    JA0000H5645,
    JA0000H5646,
    JA0000T7566,
    JA0000T0001,
    FR0000F8880,
    CA0001E7655,
    CA0002E7890,
    SK2300T3212,
    SK2300H0004,
    SK2100RS981,
    SK2400E9999,
}


public enum Country
{
    USA,
    China,
    Germany,
    France,
    Japan,
    South_Korea,
    Canada,
}

public enum Sector
{
    Financial,
    Energy,
    Technology,
    Healthcare,
    Consumer_Goods,
    Industrial,
    Real_Estate,
}
public enum LevelStability
{
    High,
    Normal,
    Low,
}


