using UnityEngine;
using System.Collections.Generic;

public interface IBondsTradingService
{    
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
    public string BondName { get; }
    public int Quantity{ get; }
    public float CurrentValue{ get;}
    public string RepaymentDate{ get; }
    public float CouponValue { get; }
    public string CouponPaymentDate { get; }
    public int QuantityPayQuater { get; }
    public float NominalValue{ get; }
    public string RatingIssuer { get; }

}

public class BondInfoPortfolio
{
    public Dictionary<string, int> Bonds = new();
    public int Quantity { get; }
    public float BondsSummaryValue { get; }
    public float CurrentValue { get; }
}

public enum RatingIssuer
{
    AAA,
    BBB,
    CCC,
    C,
    D,
}