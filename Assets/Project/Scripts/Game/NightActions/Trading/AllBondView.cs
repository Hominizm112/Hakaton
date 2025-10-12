using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "MarketDataProvider", menuName = "Portfolio/MarketDataProvider")]
public class MarketDataProvider : ScriptableObject
{
    public List<StockConfig> AllStockConfigs;
    public List<BondConfig> AllBondConfigs;
}