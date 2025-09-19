using UnityEngine;
using System.Collections.Generic;

public interface IBondsTradingService
{
    //void UpdateBondPrices(Dictionary<string, float> newPrices);
    
   // List<Bond> GetAvailableBonds();
    //Bond GetBondData(string bondId);
    //TradeResult BuyBond(string bondId, int quantity);
    // TradeResult SellBond(string bondId, int quantity);
    // List<BondPosition> GetBondPortfolio();
    //float CalculateBondYield(string bondId, int quantity);
    //float CalculateBondMaturityValue(string bondId, int quantity);
}
public class Bond: IAsset
{
    public string Ticker { get; set; }
    public Dictionary<string, int> BondName = new();
    public string RepaymentDate{ get; }
    public float CouponValue { get; }
    public string CouponPaymentDate { get; }
    public int QuantityPayQuater { get; }
    public float NominalValue{ get; }
    public string RatingIssuer { get; }

}


