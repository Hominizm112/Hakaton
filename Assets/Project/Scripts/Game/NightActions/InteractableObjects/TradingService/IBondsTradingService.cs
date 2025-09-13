using UnityEngine;
using System.Collections.Generic;

public interface IBondsTradingService
{    
    List<Bond> GetAvailableBonds();
    Bond GetBondData(string bondId);
    //TradeResult BuyBond(string bondId, int quantity);
   // TradeResult SellBond(string bondId, int quantity);
   // List<BondPosition> GetBondPortfolio();
    //float CalculateBondYield(string bondId, int quantity);
    float CalculateBondMaturityValue(string bondId, int quantity);
}
public class Bond: MonoService
{
    public string BondsID { get; }
    public string Issuer { get; }//эмитент
    public float CouponRate { get; }
    public float CouponValue { get; }
    public int MaturityYears { get; }
    public float CurrentValue { get; }
    public string Rating { get; }
    public int QuantityCouponMonth { get; }
}

public class BondInfoPortfolio: MonoService
{
    public string BondsID { get; }
    public string Issuer { get; }
    public int Quantity { get; }
    public float CurrentValue { get; }
    public float GainLoss { get; }
    public float GainLossPercent { get; }
    public float AnnualCoupon { get; }
}
