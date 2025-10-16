using UnityEngine;

[CreateAssetMenu(fileName = "StockConfig", menuName = "Portfolio/StockConfig")]
public class StockConfig : ScriptableObject, IAssetConfig 
{

    public Ticker Ticker { get;}


    //[SerializeField] private Ticker Ticker;
    /// <summary>
    /// public  Ticker Ticker=>
    /// </summary>
    public LevelStability LevelStability { get; }//(фикс)
    public Country Country { get; }
    public Sector Sector { get; }
    public int PercenttNextDiv { get; }//(фикс)
    public int AverageDivYield { get; }// средняя дивидендная доходность(фикс)
    public int AmountNextDiv { get; }//(фикс)
    public int InitialPrice;//начальная цена
}
