using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[CreateAssetMenu(fileName = "MarketConfigContainer", menuName = "Market/Config Container")]
public class MarketConfigContainer : ScriptableObject
{
    [Header("Конфигурации Акций")]
    [SerializeField] 
    private List<StockConfig> _allStockConfigs = new List<StockConfig>();

    [Header("Конфигурации Облигаций")]
    [SerializeField]
    private List<BondConfig> _allBondConfigs = new List<BondConfig>();

    public IEnumerable<IAssetConfig> AllStockConfigs => _allStockConfigs.Cast<IAssetConfig>();
    public IEnumerable<IAssetConfig> AllBondConfigs => _allBondConfigs.Cast<IAssetConfig>();

}