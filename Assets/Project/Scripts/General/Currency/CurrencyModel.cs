using System;

public class CurrencyModel
{
    public event Action<int> OnCurrencyChanged;
    private int _currencyAmount;
    public int CurrencyAmount
    {
        get => _currencyAmount;
        set
        {
            if (_currencyAmount != value)
            {
                _currencyAmount = value;
                OnCurrencyChanged?.Invoke(_currencyAmount);
            }
        }
    }

    public CurrencyModel(int initialAmount = 0)
    {
        _currencyAmount = initialAmount;
    }

    public bool CanAfford(int amount)
    {
        return _currencyAmount >= amount;
    }

    public void AddCurrency(int amount)
    {
        if (amount < 0)
        {
            throw new ArgumentException("Cannot add negative currency");
        }
        CurrencyAmount += amount;
    }

    public bool SpendCurrency(int amount)
    {
        if (amount < 0)
        {
            throw new ArgumentException("Cannot spend negative currency");
        }

        if (!CanAfford(amount))
        {
            return false;
        }

        CurrencyAmount -= amount;
        return true;
    }

    public void SetCurrency(int amount)
    {
        if (amount < 0)
        {
            throw new ArgumentException("Currency cannot be negative");
        }

        CurrencyAmount = amount;
    }

}
