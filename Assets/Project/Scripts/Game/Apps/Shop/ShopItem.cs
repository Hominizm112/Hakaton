using UnityEngine;
using TMPro;

public class ShopItem : MonoBehaviour
{
    [SerializeField] private Commodity commodity;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text priceText;

    protected void Awake()
    {
        nameText.text = commodity.commodityName;
        priceText.text = commodity.basePrice.ToString();
    }


    public void Select()
    {
        (Mediator.Instance.GetService<AppController>().GetApp<ShopApp>() as ShopApp).AddToCart(commodity);
    }


}
