
public interface IActiv
<<<<<<< Updated upstream
{ 
    float CurrentValue { get; }//<---заменить на int
    Ticker Ticker { get; } 
    object Config { get; }
=======
{
    int CurrentValue { get; }
    Ticker Ticker { get; }
    int Quantity { get; }


}

public interface IActiv<TConfig> : IActiv where TConfig : IAssetConfig
{
    TConfig Config { get; } 
>>>>>>> Stashed changes
}