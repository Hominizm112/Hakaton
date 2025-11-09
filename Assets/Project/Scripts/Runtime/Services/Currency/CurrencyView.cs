using UnityEngine;
using TMPro;
using Zenject;
using UniRx;
using GameCore.UI;

public class CurrencyView : View
{
    [SerializeField] private TMP_Text _currencyText;

    [Inject] private CurrencyPresenter _currencyPresenter;

    private CompositeDisposable _disposables = new();

    public override void Initialize()
    {
        _currencyPresenter.OnValueChanged
            .Subscribe(UpdateCurrencyDisplay)
            .AddTo(_disposables);
    }

    public void UpdateCurrencyDisplay(int amount)
    {
        if (_currencyText != null)
        {
            _currencyText.text = amount.ToString();
        }
    }

    public override void Dispose()
    {
        _disposables.Dispose();
    }


}
