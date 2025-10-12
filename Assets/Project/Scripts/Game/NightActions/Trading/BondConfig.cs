using UnityEngine;

[CreateAssetMenu(fileName = "BondConfig", menuName = "Portfolio/BondConfig")]
public class BondConfig : ScriptableObject,IAssetConfig
{
    public Ticker Ticker { get; set; }
    public Country Country { get; }
    public Sector Sector { get; }
    public int CouponValue { get; }
    public int NominalValue { get; }
    public RatingIssuer RatingIssuer { get; }
}
