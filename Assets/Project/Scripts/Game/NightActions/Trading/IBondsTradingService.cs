using System;

public class Bond :  SampleActiv<BondConfig>
{
    public readonly BondConfig BondInfo;
    //public DateTime RepaymentDate;
    //public DateTime CouponPaymentDate;
    //public int QuantityPayQuater;
    private readonly BondConfig _config;
    public override Ticker Ticker => throw new NotImplementedException();
    public override BondConfig Config => _config; 

    public Bond(BondConfig bondConfig, int initialCurrentValue,int initialQuantity)
        : base(initialCurrentValue, initialQuantity: initialQuantity, bondConfig)
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
