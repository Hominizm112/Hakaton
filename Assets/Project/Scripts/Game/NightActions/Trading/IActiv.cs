using UnityEngine;

public interface IActiv
{ 
    int CurrentValue { get; }//<---заменить на int
    Ticker Ticker { get; } 
    object Config { get; }
}