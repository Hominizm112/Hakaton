using UnityEngine;
using System.Collections.Generic;

public class MarketAssetInfo
{
    public readonly StockConfig Config;

    public int CurrentMarketPrice { get; set; }

    public MarketAssetInfo(StockConfig config)
    {
        Config = config;
        CurrentMarketPrice = config.InitialPrice;
    }
}


public static class MarketData
{
    public static Dictionary<Ticker, MarketAssetInfo> _allMarketStocks; 
    public static Dictionary<Ticker, MarketAssetInfo> _allMarketBonds;
    public static IReadOnlyDictionary<Ticker, MarketAssetInfo> AllMarketStocks => _allMarketStocks;
    public static IReadOnlyDictionary<Ticker, MarketAssetInfo> AllMarketBonds => _allMarketBonds;
    static MarketData()
    {
   
    }
    
    public static void Initialize(MarketConfigContainer container)
    {
        if (container == null || container.AllStockConfigs == null)
            {
                //добавить обработку ошибки
                //throw new ArgumentNullException("MarketConfigContainer  равно null.");
            }
        var marketDict = new Dictionary<Ticker, MarketAssetInfo>();
        foreach (var config in container.AllStockConfigs)
        {

            if (config != null && !marketDict.ContainsKey(config.Ticker))
            {
                marketDict.Add(config.Ticker, new MarketAssetInfo(config));
            }
        }
        _allMarketStocks = marketDict;
    }
    
}

public class MarketDataLoader : MonoBehaviour
{
    //Tooltip(" MarketConfigContainer")]
    [SerializeField] private string configAssetName = "GlobalMarketConfigs";
    private EventBus  _eventBus;


}
