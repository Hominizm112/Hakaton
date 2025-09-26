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
public class Bond
{
    public BondConstInfo BondInfo{ get; set; }
    public float CurrentValue { get; set; }
    public string RepaymentDate { get; }
    public string CouponPaymentDate { get; }
    public int QuantityPayQuater { get; }
}

public class BondConstInfo
{
    public Ticker Ticker { get; set; }
    public Country Country { get; }
    public Sector Sector { get; }
    public float CouponValue { get; }
    public float NominalValue { get; }
    public RatingIssuer RatingIssuer { get; }  
}

public enum RatingIssuer
{
    AAA,
    BBB,
    CCC,
    D,
}
