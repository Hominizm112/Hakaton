using UnityEngine;
using System.Collections.Generic;
using UnityEngine.LightTransport;

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
public class Bond : SampleActiv
{
    public readonly BondConfig BondInfo;
    public string RepaymentDate;
    public string CouponPaymentDate;
    public int QuantityPayQuater;

    public Bond(BondConfig bondConfig)
    {
        BondInfo = bondConfig;
    }
}



public enum RatingIssuer
{
    AAA,
    BBB,
    CCC,
    D,
}
