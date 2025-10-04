using System;

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
    public DateTime RepaymentDate;
    public DateTime CouponPaymentDate;
    public int QuantityPayQuater;
    public override Ticker Ticker => throw new NotImplementedException();

    public override object Config => throw new NotImplementedException();

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
