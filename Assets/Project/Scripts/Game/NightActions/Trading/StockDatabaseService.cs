using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

public class MarketAssetInfo
{
    public readonly IAssetConfig Config;
    public int CurrentMarketPrice { get; set; }
    public MarketAssetInfo(IAssetConfig config)
    {
        Config = config;
       // CurrentMarketPrice = config.InitialPrice;
    }
}


public class MarketData
{
    public Dictionary<Ticker, MarketAssetInfo> _allMarketStocks;
    public Dictionary<Ticker, MarketAssetInfo> _allMarketBonds;
    public IReadOnlyDictionary<Ticker, MarketAssetInfo> AllMarketStocks => _allMarketStocks;
    public IReadOnlyDictionary<Ticker, MarketAssetInfo> AllMarketBonds => _allMarketBonds;
    public MarketData(MarketConfigContainer container)
    {
        if (container == null || container.AllStockConfigs == null)
        {
            Debug.LogError("MarketData инициализирован с пустым контейнером.");
            _allMarketStocks = new Dictionary<Ticker, MarketAssetInfo>();
            _allMarketBonds = new Dictionary<Ticker, MarketAssetInfo>();
            return;
        }

        _allMarketStocks = container.AllStockConfigs
        .Where(config => config != null)
        .ToDictionary(
            config => config.Ticker,
            config => new MarketAssetInfo(config)
        );

        _allMarketBonds = container.AllBondConfigs
        .Where(config => config != null)
        .ToDictionary(
            config => config.Ticker,
            config => new MarketAssetInfo(config)
        );

        Debug.Log($"MarketData успешно загружена. Акций: {_allMarketStocks.Count}, Облигаций: {_allMarketBonds.Count}");
    }
//поиск информации об активе по тикеру
    public IAssetConfig FindAssetConfigInMarket(Ticker ticker, Type assetType)
    {
        IReadOnlyDictionary<Ticker, MarketAssetInfo> targetDictionary = null;

        targetDictionary = assetType switch
        {
            Type stockType when stockType == typeof(Stock) => _allMarketStocks,

            Type bondType when bondType == typeof(Bond) => _allMarketBonds,

            _ => throw new ArgumentException($"Неподдерживаемый тип актива: {assetType}. Ожидается Stock или Bond.")
        };

        if (targetDictionary.TryGetValue(ticker, out MarketAssetInfo assetInfo))
        {
            return assetInfo.Config;
        }
        throw new KeyNotFoundException($"Актив с тикером {ticker} типа {assetType.Name} не найден в данных рынка.");

    }
    //поиск активов по тикеру
    public MarketAssetInfo FindAssetInAllAssets(Ticker ticker, Type assetType)
    {
        //копия кода
        IReadOnlyDictionary<Ticker, MarketAssetInfo> targetDictionary = null;
        targetDictionary = assetType switch
        {
            Type stockType when stockType == typeof(Stock) => _allMarketStocks,

            Type bondType when bondType == typeof(Bond) => _allMarketBonds,

            _ => throw new ArgumentException($"Неподдерживаемый тип актива: {assetType}. Ожидается Stock или Bond.")
        };

        if (targetDictionary.TryGetValue(ticker, out MarketAssetInfo assetInfo))
        {
            return assetInfo;
        }

        throw new KeyNotFoundException($"Актив с тикером {ticker} типа {assetType.Name} не найден в данных рынка.");
    }

}

