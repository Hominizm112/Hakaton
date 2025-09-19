using UnityEngine;
using TMPro;

[RequireComponent(typeof(TMP_Text))]
public class CurrencyViewSubscriber : MonoBehaviour
{
    private TMP_Text _currencyText;
    private void Awake()
    {
        _currencyText = GetComponent<TMP_Text>();
        Mediator.Instance.GlobalEventBus.Subscribe<CurrencyChangedEvent>(e => HandleDisplay(e.NewAmount));
        HandleDisplay(Mediator.Instance.GetService<CurrencyPresenter>().GetCurrency());
    }

    private void HandleDisplay(int amount)
    {
        _currencyText.text = amount.ToString();
    }

    private void OnDestroy()
    {
        Mediator.Instance?.GlobalEventBus.Unsubscribe<CurrencyChangedEvent>(e => HandleDisplay(e.NewAmount));
    }
}
