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
        print("Selected");
        print(Mediator.Instance.name);
        print(Mediator.Instance.GetService<AppController>());
        print(Mediator.Instance.GetService<AppController>().GetApp<ShopApp>());
        Mediator.Instance.GetService<AppController>().GetApp<ShopApp>().AddToCart(commodity);
    }


}
