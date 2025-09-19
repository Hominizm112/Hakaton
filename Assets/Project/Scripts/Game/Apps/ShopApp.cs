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
    private List<CartItem> _cartItems = new();

    private Action<CartItemData> _onCartChanged;
    private int totalCost;

    private void OnEnable()
    {
        _onCartChanged += _ => CalculateCartCost();
    }

    public void AddToCart(Commodity commodity)
    {
        if (!ExistsCartItemData(commodity))
        {
            CartItem cartItem = Instantiate(cartItemPrefab, Vector3.zero, Quaternion.identity, cartHolder).GetComponent<CartItem>();
            cartItem.transform.localScale = Vector3.one;
            _cartItems.Add(cartItem);
            _cartList.Add(new(commodity, 100, 0));
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
        _cartItems.Remove(cartItem);
        CalculateCartCost();

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
            totalCost += item.price * item.quantity;
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
        for (int i = _cartItems.Count - 1; i >= 0; i--)
        {
            Destroy(_cartItems[i]);
        }
        _cartItems.Clear();
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
        if (!Mediator.Instance.GetService<CurrencyPresenter>().CanAfford(totalCost) || _cartItems.Count == 0) return;

        Mediator.Instance.GetService<CurrencyPresenter>().TrySpendCurrency(totalCost);
    }

    private void OnDisable()
    {
        ClearCart();
        _onCartChanged = null;
    }

}

