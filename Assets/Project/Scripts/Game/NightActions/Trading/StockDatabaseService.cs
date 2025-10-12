using UnityEngine;
using System;
using System.Collections.Generic;



public class MarketAssetInfo
{
    public readonly StockConfig Config;

    public float CurrentMarketPrice { get; set; }

    public MarketAssetInfo(StockConfig config)
    {
        Config = config;
        CurrentMarketPrice = config.InitialPrice;
    }
}
public static class MarketData
{
    public static Dictionary<Ticker, MarketAssetInfo> _allMarketStocks;
    //static MarketData()
    //{//
       // MarketConfigContainer container = Resources.Load<MarketConfigContainer>("MarketConfigContainer");
   // }
    public static IReadOnlyDictionary<Ticker, MarketAssetInfo> AllMarketStocks
    {
        get
        {
            if (_allMarketStocks == null)
            {
                //добавить обработку ошибки
                //throw new InvalidOperationException("MarketData не была инициализирована. Загрузчик не завершил работу.");
            }
            return _allMarketStocks;
        }
        
    }

    public static void Initialize(MarketConfigContainer container)
    {
        if (container == null || container.AllStockConfigs == null)
        {
            //добавить обработку ошибки
            //throw new ArgumentNullException("MarketConfigContainer или его содержимое равно null.");
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
        //if (container == null)
       // {
            //добавить обработку
           // return;
       // }
        //StockConfig[] allConfigs = container.AllStockConfigs;

        //var marketDict = new Dictionary<Ticker, MarketAssetInfo>();

       // foreach (var config in allConfigs)
      //  {

          //  marketDict.Add(config.Ticker, new MarketAssetInfo(config));

       // }

       // AllMarketStocks = marketDict;






       // var marketDict = new Dictionary<Ticker, MarketAssetInfo>();
        //foreach (var config in allConfigs)
       // {
           // m//arketDict.Add(config.Ticker, new MarketAssetInfo(config));
       // }
       // AllMarketStocks = marketDict;
//
   // }
}

public class MarketDataLoader : MonoBehaviour
{
    [Tooltip("Имя MarketConfigContainer (например, 'GlobalMarketConfigs') в папке Resources.")]
    [SerializeField] private string configAssetName = "GlobalMarketConfigs";
    private EventBus  _eventBus;
    private System.Collections.IEnumerator Start()
    {
        if (string.IsNullOrEmpty(configAssetName))
        {
            Debug.LogError("MarketDataLoader: configAssetName не задан.");
            yield break;
        }
        ColorfulDebug.LogGreen($"Запуск асинхронной загрузки конфигурации: {configAssetName}");
        ResourceRequest request = Resources.LoadAsync<MarketConfigContainer>(configAssetName);
        yield return request;
        MarketConfigContainer container = request.asset as MarketConfigContainer;

        if (container == null)
        {
            // Обработка критической ошибки загрузки
            _eventBus.Publish(new DebugLogErrorEvent($"Критическая ошибка: не удалось найти или загрузить MarketConfigContainer '{configAssetName}'."));
            yield break;
        }


    }


}
