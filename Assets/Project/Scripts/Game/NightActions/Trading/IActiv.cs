using UnityEngine;

public interface IActiv
{ 
    float CurrentValue { get; }//<---заменить на int
    Ticker Ticker { get; } 
    object Config { get; }
}