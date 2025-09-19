using UnityEngine;
using System.Collections.Generic;
public abstract class IAsset
{
    public float CurrentValue { get; set; }
    public int Quantity { get; set; }
   

}
namespace MyGame.Enums
{
    public enum TradeType
    {
        Buy,
        Sell
    }
}
