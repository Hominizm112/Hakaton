using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Localization.Components;
using Zenject;

public class CartItemData
{
    public int quantity;

    public CartItemData(int quantity)
    {
        this.quantity = quantity;
    }
}

[Bind(typeof(CartItem))]
public class CartItem : InjectableBehaviour
{
    /*
    [SerializeField] private LocalizeStringEvent nameText;
    [SerializeField] private TMP_Text priceText;
    [SerializeField] private TMP_Text quantityText;

    [SerializeField] private Button removeButton;
    [SerializeField] private Button addButton;
    [SerializeField] private Button reduceButton;

    [Inject] private AppController _appController;

    private CartItemData _itemData;
    private ShopApp _shopApp;

    public override void OnConstruct()
    {
        removeButton.onClick.AddListener(HandleRemove);
        addButton.onClick.AddListener(HandleAdd);
        reduceButton.onClick.AddListener(HandleReduce);
        _shopApp = _appController.GetApp<ShopApp>();
    }

    public void SetItem(CartItemData cartItemData)
    {
        if (_itemData != null && cartItemData != _itemData) return;

        _itemData = cartItemData;
        // nameText.StringReference = cartItemData.commodity.commodityName;
        // priceText.text = (cartItemData.commodity.basePrice * cartItemData.quantity).ToString();
        quantityText.text = cartItemData.quantity.ToString();
    }

    private void HandleRemove()
    {
        _shopApp.RemoveFromCart(this, _itemData);
        Destroy(gameObject);

    }

    private void HandleAdd()
    {
        _shopApp.AddInCart(_itemData);

    }

    private void HandleReduce()
    {
        _shopApp.ReduceInCart(_itemData);

    }

    private void OnDisable()
    {
        HandleRemove();
    }

    private void OnDestroy()
    {
        removeButton.onClick.RemoveAllListeners();
        addButton.onClick.RemoveAllListeners();
        reduceButton.onClick.RemoveAllListeners();
    }

*/
}
