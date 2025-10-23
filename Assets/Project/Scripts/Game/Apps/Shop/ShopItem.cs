using UnityEngine;
using TMPro;
using UnityEngine.Localization.Components;

public class ShopItem : MonoBehaviour
{
    [SerializeField] private Commodity commodity;
    [SerializeField] private LocalizeStringEvent nameLocalizeEvent;
    [SerializeField] private TMP_Text priceText;

    protected void Awake()
    {
        nameLocalizeEvent.StringReference = commodity.commodityName;
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
