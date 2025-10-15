
using System.Collections.Generic;
using MyGame.Enums;
using System;
<<<<<<< Updated upstream
using Unity.VisualScripting;
using System.Linq;
=======
>>>>>>> Stashed changes


public abstract class SampleActiv<TConfig> : IActiv<TConfig> where TConfig : IAssetConfig
{
<<<<<<< Updated upstream
    public abstract Ticker Ticker { get; }
    public abstract object Config { get; }
    public float CurrentValue { get; }
    private int quantity;

    public int Quantity => quantity;
=======
    private int _quantity;
    public int Quantity => _quantity;
    public abstract Ticker Ticker { get; }
    public abstract TConfig Config { get; }  
    public int CurrentValue { get; protected set; } 
>>>>>>> Stashed changes

    public void AddQuantity(int amount)
    {
        quantity += amount;
    }

    public void RemoveQuantity(int amount)
    {
<<<<<<< Updated upstream
        quantity -= amount;
=======
        _quantity -= amount;
    }

    public SampleActiv(int initialValue, int initialQuantity, TConfig config)
    {
        CurrentValue = initialValue;
        _quantity = initialQuantity;
>>>>>>> Stashed changes
    }

}

<<<<<<< Updated upstream
public class PortfollioService : MonoService // IPortfolioService
{
=======
public class PortfollioService //MonoService // IPortfolioService
{

    private MarketData _marketData;
>>>>>>> Stashed changes
    private PortfolioSummary _portfolioSummary = new PortfolioSummary();
    private Mediator _mediator;
    public override List<Type> requiredServices { get; protected set; } = new List<Type>();
    public Dictionary<Ticker, Stock> AvailableStocks { get; private set; }
    public Dictionary<Ticker, Bond> AvailableBonds { get; private set; }
    //public IReadOnlyDictionary<Ticker, Stock> Stocks => AvailableStocks;
    ///public IReadOnlyDictionary<Ticker, Bond> Bondds => AvailableBonds;

    private Dictionary<Ticker, IActiv> AvailableAssets { get; set; } = new();
    public IReadOnlyDictionary<Ticker, IActiv> Assets => AvailableAssets;

    public Action<PortfolioSummary> OnPortfolioUpdated { get; internal set; }

    private void Awake()
    {
        Mediator.Instance.RegisterService(this);
    }

<<<<<<< Updated upstream
    public void AddNewAsset(IActiv newAsset)//добавление нового актива
    {
        if (newAsset == null || AvailableAssets.ContainsKey(newAsset.Ticker))

            return;

        AvailableAssets.Add(newAsset.Ticker, newAsset);
        _mediator.GlobalEventBus.Publish(new AssetListChangedEvent(newAsset.Ticker));
    }
    public void LoadInitialData()//логика сохранения и инициализации
    {
        //AvailableStocks = new Dictionary<Ticker, Stock>();
        //AvailableBonds = new Dictionary<Ticker, Bond>();
=======
    public void PortfolioInitialize()
    {
>>>>>>> Stashed changes
        AvailableAssets = new Dictionary<Ticker, IActiv>();
        

        
    }

    #region FindByTicker

    public IActiv GetAssetByTicker(Ticker ticker)// поиск актива по тикеру
    {

        if (_portfolioSummary.MyActives.TryGetValue(ticker, out IActiv activ))
        {
            return activ;
        }

       // _mediator.GlobalEventBus.Publish<DebugLogErrorEvent>(new("Актива с заданным тикером не существует"));
        return null;

<<<<<<< Updated upstream
    public float GetAssetPrice(Ticker ticker)//поиск цены по тикеру
    {
        SampleActiv activ = null;

        AvailableStocks.TryGetValue(ticker, out Stock stock);
        activ = stock.IsUnityNull() ? activ : stock;

        AvailableBonds.TryGetValue(ticker, out Bond bond);
        activ = bond.IsUnityNull() ? activ : bond;
=======
    }
    public int GetAssetPrice(Ticker ticker)//поиск цены по тикеру
    {
        if (_portfolioSummary.MyActives.TryGetValue(ticker, out IActiv activ))
        {
            return activ.CurrentValue;
        }

        throw new KeyNotFoundException($"Актив с тикером {ticker} не найден в портфеле игрока.");
>>>>>>> Stashed changes

    }

<<<<<<< Updated upstream
=======
    public int GetQuantityByTicker(Ticker targetTicker)//возврат поля количества по тикеру
    {

        if (_portfolioSummary.MyActives.TryGetValue(targetTicker, out IActiv activ))
        {
            return activ.Quantity;
        }
        else
        {
            return 0;
        }
    }
    
#endregion
    public void UpdatePortfolioValue(Type assetType, int totalCost, int quantity, TradeType TypeOperation)
    {
        _portfolioSummary.RecalculateValueCount(assetType, TypeOperation, totalCost, quantity);
        _portfolioSummary.RecalculateCashBalance(TypeOperation, totalCost);
    }
>>>>>>> Stashed changes


    //кнопки быстрой продажи покупки
    #region BuyActiv

<<<<<<< Updated upstream
    public void BuyAsset(Type assetType, Ticker ticker, int quantity, float totalCost)
    {
        _portfolioSummary.CashBalance -= totalCost;
        _portfolioSummary.MyActives[ticker].AddQuantity(quantity);

=======
    public BuyTransactionState BuyAsset(Type assetType, Ticker ticker, int price, int quantity)
    {
        if (!_marketData.AllMarketBonds.ContainsKey(ticker) || !HasEnoughCash(price*quantity))
        {
            return BuyTransactionState.NotEnough;
        }

        _portfolioSummary.AddQuantity(ticker,quantity);//обновление количества

        if (_portfolioSummary.MyActives.TryGetValue(ticker, out IActiv asset))
        {
            return BuyTransactionState.NoNeedCreatedButton;
        }

        else
        {
            IActiv newAsset = CreateAssetInstance(assetType, ticker, price, quantity);
            _portfolioSummary.AddMyActive(ticker, newAsset);
            return BuyTransactionState.NeedCreatedButton;
        }

    }
    

    #region SellActiv
    public SellTransactionState SellAsset(Type assetType, Ticker ticker, int quantity, int totalCost)
    {
        if (!HasEnoughQuantityActive(ticker, quantity))//недостаточно активов
        {
            return SellTransactionState.NotEnough;
        }

        IActiv asset = GetAssetByTicker(ticker);
        _portfolioSummary.RemoveQuantity(ticker,quantity);

        if (asset.Quantity <= 0)
        {
            _portfolioSummary.RemoveMyActive(ticker);
            return SellTransactionState.NeedRemovedButton; //удаление кнопки
        }
        else
        {
            //просто изменение количества
            return SellTransactionState.NoNeedRemovedButton;
        }
    }

    #endregion
    public IActiv CreateAssetInstance(Type assetType, Ticker ticker, int price, int quantity)
    {
        IAssetConfig baseConfig = _marketData.FindAssetConfigInMarket(ticker, assetType);
        IActiv asset = null;

        //switch??
>>>>>>> Stashed changes
        if (assetType == typeof(Stock))
        {
            _portfolioSummary.StocksValue += totalCost;
            _portfolioSummary.CountStocks += quantity;
        }

        if (assetType == typeof(Bond))
        {
<<<<<<< Updated upstream
            _portfolioSummary.BondsValue += totalCost;
            _portfolioSummary.CountBonds += quantity;
=======
            if (baseConfig is BondConfig bondConfig)
            {
                asset = new Bond(bondConfig, price, quantity);
            }
>>>>>>> Stashed changes
        }

        //OnPortfolioUpdated?.Invoke(_portfolioSummary);
    }
    #endregion

<<<<<<< Updated upstream
    #region SellActiv
    public bool SellAsset(Type assetType, Ticker ticker, int quantity, float totalCost)
    {
        _portfolioSummary.CashBalance += totalCost;
        _portfolioSummary.MyActives[ticker].RemoveQuantity(quantity);

        if (_portfolioSummary.MyActives[ticker].Quantity <= 0)
        {
            _portfolioSummary.MyActives.Remove(ticker);
        }

        if (assetType == typeof(Stock))
        {
            _portfolioSummary.StocksValue -= totalCost;
            _portfolioSummary.CountStocks -= quantity;
        }

        if (assetType == typeof(Bond))
        {
            _portfolioSummary.BondsValue -= totalCost;
            _portfolioSummary.CountBonds -= quantity;
        }

        return true;

    }
    #endregion
    public bool HasEnoughQuantityActive(Ticker ticker, int quantity)
    {

        if (!_portfolioSummary.MyActives.ContainsKey(ticker) || _portfolioSummary.MyActives[ticker].Quantity < quantity)
        {
            return false;
        }
        else
        {
            return true;
        }
=======
    public bool HasEnoughQuantityActive(Ticker ticker, int quantity)
    {
       return !(!_portfolioSummary.MyActives.ContainsKey(ticker) || 
             _portfolioSummary.MyActives[ticker].Quantity < quantity);
>>>>>>> Stashed changes
    }

    public bool HasEnoughCash(float totalCost)//заменить на int
    {
        return _portfolioSummary.CashBalance >= totalCost;
    }
 

<<<<<<< Updated upstream
    public IActiv GetAssetByTicker(Ticker ticker)// поиск актива по тикеру
    {

        if (AvailableStocks.TryGetValue(ticker, out Stock stock))
        {
            return stock;
        }

        if (AvailableBonds.TryGetValue(ticker, out Bond bond))
        {
            return bond;
        }

        _mediator.GlobalEventBus.Publish<DebugLogErrorEvent>(new("Актива с заданным тикером не существует"));
        return null;

    }


=======
>>>>>>> Stashed changes

    //покупка иных
    public void CheckOtherStocks()
    {
        ///
    }
    public void CheckOtherBonds()
    {

    }
    private void CalculatDayGainLossPercent()
    {

    }

    private void CalculateDayGainLoss()
    {

    }
    private void CalculateTotalGainLossPercent()
    {

    }
    private void CalculateTotalGainLoss()
    {

    }
    //Analytics
    private void GeneratePortfolioReport()
    {


    }

    internal PortfolioSummary GetPortfolioSummary()
    {
        throw new NotImplementedException();
    }
}
