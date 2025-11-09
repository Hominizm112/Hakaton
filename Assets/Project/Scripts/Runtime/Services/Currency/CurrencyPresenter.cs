using UniRx;
using UnityEngine;
using Zenject;

public class CurrencyPresenter : EventListener
{
    [SerializeField] private CurrencyView _view;

    [Inject] private SaveManager _saveManager;
    [Inject] private AudioHub _audioHub;

    public ReactiveCommand<int> OnValueChanged = new();

    private CurrencyModel _model;

    [Inject]
    public void Construct()
    {
        _model = new CurrencyModel(0);
        _model.OnCurrencyChanged += HandleCurrencyChanged;

        SubscribeToEvent<CurrencyChangedEvent>((e) => _audioHub.PlayOneShot(SoundType.CoinToss, .1f));
        SubscribeToEvent<LoadDataEvent>(_ => LoadData());
    }

    private void LoadData()
    {
        int savedCurrency = _saveManager?.GetInt("currency", 0) ?? 0;
        _model.AddCurrency(savedCurrency);
    }

    private void HandleCurrencyChanged(int newAmount)
    {
        SaveCurrency();

        OnValueChanged.Execute(newAmount);
    }

    private void HandleAddCurrency(int amount)
    {
        _model.AddCurrency(amount);
    }

    private void HandleSpendCurrency(int amount)
    {
        bool success = _model.SpendCurrency(amount);
    }

    private void SaveCurrency()
    {
        _saveManager.SetInt("currency", _model.CurrencyAmount);
    }

    public bool CanAfford(int amount) => _model.CanAfford(amount);
    public bool TrySpendCurrency(int amount) => _model.SpendCurrency(amount);
    public void AddCurrency(int amount) => _model.AddCurrency(amount);
    public int GetCurrency() => _model.CurrencyAmount;

    public override void Dispose()
    {
        if (_model != null)
        {
            _model.OnCurrencyChanged -= HandleCurrencyChanged;
        }

    }
}
