using UnityEngine;

[CreateAssetMenu(fileName = "StockConfig", menuName = "Portfolio/StockConfig")]
public class StockConfig : ScriptableObject
{
<<<<<<< Updated upstream
    public Ticker Ticker { get;}
=======
    public Ticker Ticker { get; }

    //[SerializeField] private Ticker Ticker;
    /// <summary>
    /// public  Ticker Ticker=>
    /// </summary>
>>>>>>> Stashed changes
    public LevelStability LevelStability { get; }//(фикс)
    public Country Country { get; }
    public Sector Sector { get; }
    public float PercenttNextDiv { get; }//(фикс)
    public float AverageDivYield { get; }// средняя дивидендная доходность(фикс)
    public float AmountNextDiv { get; }//(фикс)
}
