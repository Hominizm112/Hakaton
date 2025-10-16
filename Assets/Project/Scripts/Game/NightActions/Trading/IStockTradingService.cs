
public class Stock :  SampleActiv<StockConfig>//одна акция в портфеле
{
    public readonly StockConfig StockInfo;
    public override Ticker Ticker => StockInfo.Ticker; 
    private readonly StockConfig _config;
    public override StockConfig Config => _config; 
   // public float OpenPrice { get; private set; } //меняются извне
    //public float ClosePrice { get; private set; }
    //public float GainLossDay{ get; private set; }
    //public float GainLossPercentDay{ get; private set; }
    //public DateTime DateNextDiv;

    public Stock(StockConfig stockConfig,int initialCurrentValue,int initialQuantity)
        : base(initialCurrentValue, initialQuantity: initialQuantity, stockConfig)
    {
       // ClosePrice = initialCurrentValue;
        StockInfo = stockConfig;
    }
}


public enum Ticker
{
    //акции
    None,
    GDC,
    SRV,
    GWI,
    TSW,
    VRD,
    AEON,
    EGC,
    CSD,
    RRT,
    CHAI,
    GNM,
    //облигации
    UA0000F8132,//F=Financial
    UA0000С8000,
    CH0000E9010,
    CH0000I7878,
    CH0000I0001,
    CH0000H0004,
    CH0000F7777,
    GE0000T0009,
    GE0000I9090,
    GE0000E6789,
    JA0000H5645,
    JA0000H5646,
    JA0000T7566,
    JA0000T0001,
    FR0000F8880,
    CA0001E7655,
    CA0002E7890,
    SK2300T3212,
    SK2300H0004,
    SK2100RS981,
    SK2400E9999,
}


public enum Country
{
    USA,
    China,
    Germany,
    France,
    Japan,
    South_Korea,
    Canada,
}

public enum Sector
{
    Financial,
    Energy,
    Technology,
    Healthcare,
    Consumer_Goods,
    Industrial,
    Real_Estate,
}
public enum LevelStability
{
    High,
    Normal,
    Low,
}


