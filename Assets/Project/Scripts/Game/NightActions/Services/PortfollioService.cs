using UnityEngine;
using System.Collections.Generic;
using MyGame.Enums;

public class PortfollioService : MonoService, IPortfolioService
{
    private PortfolioSummary _portfolioSummary = new PortfolioSummary();
    private Stock _stocks = new Stock();
    private Bond _bonds = new Bond();
    
    public PortfolioSummary GetPortfolioSummary()//отображение портфеля
    {
        var summary = new PortfolioSummary
        {
            CashBalance = _portfolioSummary.CashBalance,
            StocksValue = _portfolioSummary.StocksValue,
            BondsValue = _portfolioSummary.BondsValue,
            TotalGainLoss = _portfolioSummary.TotalGainLoss,
            DayGainLoss = _portfolioSummary.DayGainLoss,
            MyStocks = _stocks.Stocks != null ?
                   new Dictionary<string, int>(_stocks.Stocks) : new Dictionary<string, int>(),
            MyBonds = _bonds.BondName  != null ?
                   new Dictionary<string, int>(_bonds.BondName ) : new Dictionary<string, int>(),
            CountStocks = _portfolioSummary.CountStocks,
            CountBonds = _portfolioSummary.CountBonds
        };

        if (summary.TotalValue == 0)
        {
            Debug.Log("Портфель пуст");
        }
        else
        {
            Debug.Log("💼 Информация о портфеле:");
            Debug.Log($"Баланс: {summary.CashBalance:C}");
            Debug.Log($"Стоимость акций: {summary.StocksValue:C}");
            Debug.Log($"Стоимость облигаций: {summary.BondsValue:C}");
            Debug.Log($"Общая стоимость: {summary.TotalValue:C}");
            if (summary.CountStocks > 0)
            {
                Debug.Log("📈 АКЦИИ в портфеле:");
                foreach (var stock in summary.MyStocks)
                {
                    Debug.Log($"  {stock.Key}: {stock.Value} шт.");
                }
            }
            else
            {
                Debug.Log("📈 Акций в портфеле нет");
            }

            if (summary.CountBonds > 0)
            {
                Debug.Log("Облигации в портфеле:");
                foreach (var bond in summary.MyBonds)
                {
                    Debug.Log($"  {bond.Key}: {bond.Value} шт.");
                }
            } 
            else
            {
                Debug.Log("Облигаций в портфеле нет");
            }
        }

        return summary;
    }

 
    private Dictionary<string, object> _availableAssets;
    public void PortfolioManager(PortfolioSummary portfolio, Dictionary<string, object> availableAssets)
    {
        _portfolioSummary = portfolio;
        _availableAssets = availableAssets;
    }

   public bool TradeAssets(TradeType tradeType, object asset, int quantity)
{
    
    if (asset == null || quantity <= 0)
    {
        Debug.LogError("Неверные параметры торговой операции");
        return false;
    }

    float assetPrice;
    string ticker;
    bool isStock = false;

    // определение типа актива
    if (asset is Stock stock)
    {
        assetPrice = stock.CurrentValue;
        ticker = stock.Ticker;
        isStock = true;
    }
    else if (asset is Bond bond)
    {
        assetPrice = bond.CurrentValue;
        ticker = bond.Ticker;
    }
    else
    {
        Debug.LogError("Неподдерживаемый тип актива");
        return false;
    }

    float totalCost = quantity * assetPrice;

    if (tradeType == TradeType.Buy)
    {
        return BuyAsset(isStock, ticker, quantity, totalCost, assetPrice);
    }
    else if (tradeType == TradeType.Sell)
    {
        return SellAsset(isStock, ticker, quantity, totalCost);
    }

    return false;
}
 #region BuyActiv
    private bool BuyAsset(bool isStock, string ticker, int quantity, float totalCost, float assetPrice)
    {

        if (_portfolioSummary.CashBalance < totalCost)
        {
            Debug.Log("Недостаточно средств для совершения покупки.");
            return false;
        }

        _portfolioSummary.CashBalance -= totalCost;

        if (isStock)
        {
            if (_portfolioSummary.MyStocks.ContainsKey(ticker))
            {
                _portfolioSummary.MyStocks[ticker] += quantity;
            }
            else
            {
                _portfolioSummary.MyStocks.Add(ticker, quantity);
            }
            _portfolioSummary.StocksValue += totalCost;
            _portfolioSummary.CountStocks += quantity;
        }
        else
        {
            if (_portfolioSummary.MyBonds.ContainsKey(ticker))
            {
                _portfolioSummary.MyBonds[ticker] += quantity;
            }
            else
            {
                _portfolioSummary.MyBonds.Add(ticker, quantity);
            }
            _portfolioSummary.BondsValue += totalCost;
            _portfolioSummary.CountBonds += quantity;
        }

        Debug.Log($"Успешная покупка {ticker}. Куплено {quantity} штук по цене {assetPrice}");
        return true;
    }
#endregion BuylActiv
#region SellActiv
private bool SellAsset(bool isStock, string ticker, int quantity, float totalCost)
{
    if (isStock)
    {
        if (!_portfolioSummary.MyStocks.ContainsKey(ticker) || _portfolioSummary.MyStocks[ticker] < quantity)
        {
            Debug.Log($"Недостаточно акций {ticker} для продажи.");
            return false;
        }
    }
    else
    {
        if (!_portfolioSummary.MyBonds.ContainsKey(ticker) || _portfolioSummary.MyBonds[ticker] < quantity)
        {
            Debug.Log($"Недостаточно облигаций {ticker} для продажи.");
            return false;
        }
    }

    _portfolioSummary.CashBalance += totalCost;

    if (isStock)
    {
        _portfolioSummary.MyStocks[ticker] -= quantity;
        if (_portfolioSummary.MyStocks[ticker] <= 0)
        {
            _portfolioSummary.MyStocks.Remove(ticker);
        }
        _portfolioSummary.StocksValue -= totalCost;
        _portfolioSummary.CountStocks -= quantity;
    }
    else
    {
        _portfolioSummary.MyBonds[ticker] -= quantity;
        if (_portfolioSummary.MyBonds[ticker] <= 0)
        {
            _portfolioSummary.MyBonds.Remove(ticker);
        }
        _portfolioSummary.BondsValue -= totalCost;
        _portfolioSummary.CountBonds -= quantity;
    }

    Debug.Log($"Продажа {ticker} успешна. Продано {quantity} штук.");
    return true;
}
#endregion SellActiv
    public void AddCash(float amount)//пополнение баланса
    {

        if (amount <= 0)
        {
        }
        if (amount > 100000f)
        {
        }
        //недостаточно средств
        _portfolioSummary.CashBalance += amount;
    }

   //public float GetCashBalance() {
    //private float _cashBalance = 0f;
   //}
    //покупка иных
    public void CheckOtherStocks()
    {

    }
   public void CheckOtherBonds() {
    
   }
    //Analytics
   public void GeneratePortfolioReport() {

}
}

