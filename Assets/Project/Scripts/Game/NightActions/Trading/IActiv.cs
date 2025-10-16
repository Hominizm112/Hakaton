public interface IActiv
{
    int CurrentValue { get; }
    Ticker Ticker { get; }
    int Quantity { get; } 
}

public interface IActiv<TConfig> : IActiv where TConfig : IAssetConfig
{
    TConfig Config { get; }

}