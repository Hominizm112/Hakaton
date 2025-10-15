using System;

public class Bond :  SampleActiv<BondConfig>
{
    public readonly BondConfig BondInfo;
<<<<<<< Updated upstream
    public DateTime RepaymentDate;
    public DateTime CouponPaymentDate;
    public int QuantityPayQuater;
=======
    //public DateTime RepaymentDate;
    //public DateTime CouponPaymentDate;
    //public int QuantityPayQuater;
    private readonly BondConfig _config;
>>>>>>> Stashed changes
    public override Ticker Ticker => throw new NotImplementedException();
    public override BondConfig Config => _config; 

<<<<<<< Updated upstream
    public Bond(BondConfig bondConfig)
=======
    public Bond(BondConfig bondConfig, int initialCurrentValue,int initialQuantity)
        : base(initialCurrentValue, initialQuantity: initialQuantity, bondConfig)
>>>>>>> Stashed changes
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
