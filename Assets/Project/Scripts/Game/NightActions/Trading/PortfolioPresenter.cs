using UnityEngine;
using System;

public class PortfolioPresenter : MonoBehaviour
{
    [SerializeField] private PortfolioView _view;
    private PortfollioService _model;
    private Mediator _mediator;
    public void InitializeView(PortfolioView portfolioView)
    {
        _view = portfolioView;
        _view.OnActiveInfoClicked += HandleInfoActiv;
        _view.OnBuyActiveClicked += HandleBuyActiv;
        _view.OnSellActiveClicked += HandleSellActiv;
        _view.OnAddCashClicked += HandleAddCash;
        _view.OnCheckOtherStocksClicked += HandleCheckOtherStock;
        _view.OnCheckOtherBondsClicked += HandleCheckOtherBond;
        _view.OnGetAnalyticsClicked += HandleGetPortfolioReport;
    }
    public void Initialize(Mediator mediator)
    {
        _mediator = mediator;
    }

    private void HandleInfoActiv()
    {

    }
    private void HandleBuyActiv()
    {

    }

    private void HandleSellActiv()
    {

    }

    private void HandleAddCash(int amount)
    {
        if (amount <= 0)
        {
            _mediator.GlobalEventBus.Publish<DebugLogErrorEvent>(new("Некорректный ввод"));
            return;
        }
        else if (amount > 100000)
        {
            _mediator.GlobalEventBus.Publish<DebugLogErrorEvent>(new("Достигнут лимит пополнения"));
            return;
        }

        if (_mediator.GetService<CurrencyPresenter>().TrySpendCurrency(amount))
        {
            _model.AddCash(amount);
        }

    }

    private void HandleCheckOtherStock()
    {

    }

    private void HandleCheckOtherBond()
    {

    }

    private void HandleGetPortfolioReport()
    {

    }

    private void OnDestroy()
    {

    }


}
