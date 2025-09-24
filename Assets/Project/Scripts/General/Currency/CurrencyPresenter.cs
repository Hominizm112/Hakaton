using System;
using UnityEngine;

public class CurrencyPresenter : MonoBehaviour, IInitializable
{
    [SerializeField] private CurrencyView _view;

    public Action<int> OnValueChanged;

    private CurrencyModel _model;
    private SaveManager _saveManager;
    private Mediator _mediator;

    public void Initialize(Mediator mediator)
    {
        _mediator = mediator;


        _saveManager = mediator.GetService<SaveManager>();

        int savedCurrency = _saveManager?.GetInt("currency", 0) ?? 0;
        _model = new CurrencyModel(savedCurrency);

        _model.OnCurrencyChanged += HandleCurrencyChanged;

        _mediator.GlobalEventBus.Subscribe<CurrencyChangedEvent>((e) => _mediator.GetService<AudioHub>().PlayOneShot(SoundType.CoinToss, .1f));
    }

    public void InitializeView(CurrencyView currencyView)
    {
        _view = currencyView;
        _view.OnAddCurrencyClicked += HandleAddCurrency;
        _view.OnSpendCurrencyClicked += HandleSpendCurrency;
        UpdateView();
    }

    private void HandleCurrencyChanged(int newAmount)
    {
        UpdateView();
        SaveCurrency();

        _mediator.GlobalEventBus.Publish(new CurrencyChangedEvent(newAmount));
    }

    private void UpdateView()
    {
        if (_view != null)
        {
            _view.UpdateCurrencyDisplay(_model.CurrencyAmount);
        }
    }

    private void HandleAddCurrency()
    {
        int amount = _view.GetTestAddAmount();
        _model.AddCurrency(amount);
    }

    private void HandleSpendCurrency()
    {
        int amount = _view.GetTestSpendAmount();
        bool success = _model.SpendCurrency(amount);

        if (!success)
        {
            _view.ShowInsufficientFunds();
        }
    }

    private void SaveCurrency()
    {
        _saveManager.SetInt("currency", _model.CurrencyAmount);
    }

    public bool CanAfford(int amount) => _model.CanAfford(amount);
    public bool TrySpendCurrency(int amount) => _model.SpendCurrency(amount);
    public void AddCurrency(int amount) => _model.AddCurrency(amount);
    public int GetCurrency() => _model.CurrencyAmount;

    private void OnDestroy()
    {
        if (_model != null)
        {
            _model.OnCurrencyChanged -= HandleCurrencyChanged;
        }

        if (_view != null)
        {
            _view.OnAddCurrencyClicked -= HandleAddCurrency;
            _view.OnSpendCurrencyClicked -= HandleSpendCurrency;
        }
    }
}
