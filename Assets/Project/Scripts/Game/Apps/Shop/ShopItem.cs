using UnityEngine;
using UnityEngine.UI;

public class ShopItem : ButtonExtended
{
    [SerializeField] private Commodity commodity;

    protected override void HandleClick()
    {
        Select();
    }

    public void Select()
    {
        (Mediator.Instance.GetService<AppController>().GetApp<ShopApp>() as ShopApp).AddToCart(commodity);
    }


}
