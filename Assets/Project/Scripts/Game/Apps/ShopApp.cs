using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ShopApp : BaseApp
{
    [SerializeField] private GameObject cartItemPrefab;
    [SerializeField] private Transform cartHolder;
    [SerializeField] private TMP_Text totalCostText;

    private List<CartItemData> _cartList = new();
    private List<CartItem> _cartObjects = new();

    private Action<CartItemData> _onCartChanged;
    private int totalCost;


    protected override void Awake()
    {
        base.Awake();
        _onCartChanged += _ => CalculateCartCost();
    }


    public void AddToCart(Commodity commodity)
    {
        if (!ExistsCartItemData(commodity))
        {
            CartItem cartItem = Instantiate(cartItemPrefab, Vector3.zero, Quaternion.identity, cartHolder).GetComponent<CartItem>();
            cartItem.transform.localScale = Vector3.one;
            _cartObjects.Add(cartItem);
            _cartList.Add(new(commodity, 0));
            _onCartChanged += r => cartItem.SetItem(r);
        }

        if (TryFindCartItemData(commodity, out var cartItemData))
        {
            cartItemData.quantity++;
            _onCartChanged?.Invoke(cartItemData);
        }

    }

    public void RemoveFromCart(CartItem cartItem, CartItemData data)
    {
        _cartList.Remove(data);
        _cartObjects.Remove(cartItem);
        _onCartChanged?.Invoke(data);
    }

    public void AddInCart(CartItemData data)
    {
        data.quantity++;
        _onCartChanged?.Invoke(data);

    }

    public void ReduceInCart(CartItemData data)
    {
        if (data.quantity <= 1) return;
        data.quantity--;
        _onCartChanged?.Invoke(data);
    }

    private void CalculateCartCost()
    {
        totalCost = 0;
        foreach (var item in _cartList)
        {
            totalCost += item.commodity.basePrice * item.quantity;
        }
        UpdateTotalCostDisplay(totalCost);
    }

    private void UpdateTotalCostDisplay(int value)
    {
        totalCostText.text = value.ToString();
    }



    private void ClearCart()
    {
        _cartList.Clear();
        for (int i = _cartObjects.Count - 1; i >= 0; i--)
        {
            Destroy(_cartObjects[i]);
        }
        _cartObjects.Clear();
    }

    private bool ExistsCartItemData(Commodity commodity)
    {
        return _cartList.Exists(r => r.commodity == commodity);
    }

    private bool TryFindCartItemData(Commodity commodity, out CartItemData foundItem)
    {
        int index = _cartList.FindIndex(r => r.commodity == commodity);

        if (index >= 0)
        {
            foundItem = _cartList[index];
            return true;
        }

        foundItem = default;
        return false;
    }

    public void TryBuyItemsInCart()
    {
        Mediator mediator = Mediator.Instance;
        if (!mediator.GetService<CurrencyPresenter>().CanAfford(totalCost) || _cartObjects.Count == 0) return;

        mediator.GetService<CurrencyPresenter>().TrySpendCurrency(totalCost);
        foreach (var item in _cartList)
        {
            mediator.GetService<ShopkeeperService>().AddCommodity(item.commodity, item.quantity);

        }

    }

    private void OnDisable()
    {
        ClearCart();
        _onCartChanged = null;
    }

}

