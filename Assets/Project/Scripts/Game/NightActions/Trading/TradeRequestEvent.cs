
namespace MyGame.Enums
{
    public enum TradeType
    {
        Buy,
        Sell
    }

    public enum ActiveType
    {
        Stock,
        Bond
    }
    public enum SellTransactionState
    {
        NotEnough,
        NeedRemovedButton,
        NoNeedRemovedButton,
    }

    public enum BuyTransactionState
    {
        NotEnough,
        NoNeedCreatedButton,
        NeedCreatedButton

    }
}
