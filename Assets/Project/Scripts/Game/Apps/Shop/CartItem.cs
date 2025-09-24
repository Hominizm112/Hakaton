using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CartItemData
{
    public Commodity commodity;
    public int quantity;

    public CartItemData(Commodity commodity, int quantity)
    {
        this.commodity = commodity;
        this.quantity = quantity;
    }
}

public class CartItem : MonoBehaviour
{
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text priceText;
    [SerializeField] private TMP_Text quantityText;

    [SerializeField] private Button removeButton;
    [SerializeField] private Button addButton;
    [SerializeField] private Button reduceButton;

    private CartItemData _itemData;
    private ShopApp _shopApp;

    private void Awake()
    {
        removeButton.onClick.AddListener(HandleRemove);
        addButton.onClick.AddListener(HandleAdd);
        reduceButton.onClick.AddListener(HandleReduce);
        _shopApp = Mediator.Instance.GetService<AppController>().GetApp<ShopApp>() as ShopApp;
    }

    public void SetItem(CartItemData cartItemData)
    {
        if (_itemData != null && cartItemData != _itemData) return;

        _itemData = cartItemData;
        nameText.text = cartItemData.commodity.commodityName;
        priceText.text = (cartItemData.commodity.basePrice * cartItemData.quantity).ToString();
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


}
