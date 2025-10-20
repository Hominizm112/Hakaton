using UnityEngine;

[CreateAssetMenu(fileName = "BondConfig", menuName = "Portfolio/BondConfig")]
public class BondConfig : ScriptableObject,IAssetConfig 
{
    [Header("Базовая конфигурация актива")]
    [SerializeField] private Ticker _ticker;
    public Ticker Ticker => _ticker;
    public Country Country { get; }
    public Sector Sector { get; }
    public float CouponValue { get; }
    public float NominalValue { get; }
    public RatingIssuer RatingIssuer { get; }
}
