using UnityEngine;
using System.Collections.Generic;


namespace MyGame.Enums
{
    public enum TradeType
    {
        Buy,
        Sell
    }

    public enum BuyTransactionState
    {
        NotEnough,
        NoNeedCreatedButton,
        NeedCreatedButton,

    }

    public enum SellTransactionState
    {
        NotEnough,
        NeedRemovedButton,
        NoNeedRemovedButton,

    }

}
