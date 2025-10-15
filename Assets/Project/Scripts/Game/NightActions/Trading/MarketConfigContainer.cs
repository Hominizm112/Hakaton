using UnityEngine;

[CreateAssetMenu(fileName = "MarketConfigContainer", menuName = "Assets/Market Config Container")]
public class MarketConfigContainer : ScriptableObject
{
    public StockConfig[] AllStockConfigs;
    public BondConfig[] AllBondConfigs;

}