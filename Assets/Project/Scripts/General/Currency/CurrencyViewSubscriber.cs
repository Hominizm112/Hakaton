using UnityEngine;
using TMPro;
using Zenject;

[RequireComponent(typeof(TMP_Text))]
public class CurrencyViewSubscriber : MonoBehaviour
{
    private TMP_Text _currencyText;

    [Inject] private CurrencyPresenter _currencyPresenter;
    [Inject] private EventBus _eventBus;
    private void Awake()
    {
        _currencyText = GetComponent<TMP_Text>();
        _eventBus.Subscribe<CurrencyChangedEvent>(e => HandleDisplay(e.NewAmount));
        HandleDisplay(_currencyPresenter.GetCurrency());
    }

    private void HandleDisplay(int amount)
    {
        _currencyText.text = amount.ToString();
    }

    private void OnDestroy()
    {
        _eventBus.Unsubscribe<CurrencyChangedEvent>(e => HandleDisplay(e.NewAmount));
    }
}
