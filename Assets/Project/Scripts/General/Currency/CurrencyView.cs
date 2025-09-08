using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class CurrencyView : MonoBehaviour
{
    [SerializeField] private TMP_Text _currencyText;
    [SerializeField] private Button _addCurrencyButton;
    [SerializeField] private Button _spendCurrencyButton;
    [SerializeField] private int _testAddAmount = 100;
    [SerializeField] private int _testSpendAmount = 100;

    public event Action OnAddCurrencyClicked;
    public event Action OnSpendCurrencyClicked;

    private Mediator _mediator;

    private void Awake()
    {
        _mediator = Mediator.Instance;
        _mediator.OnInitializationCompleted += () => _mediator.GetService<CurrencyPresenter>().InitializeView(this);
        _addCurrencyButton.onClick.AddListener(() => OnAddCurrencyClicked.Invoke());
        _spendCurrencyButton.onClick.AddListener(() => OnSpendCurrencyClicked.Invoke());
    }

    public void UpdateCurrencyDisplay(int amount)
    {
        if (_currencyText != null)
        {
            _currencyText.text = $"Currency: {amount}";
        }
    }

    public void ShowInsufficientFunds()
    {
        Debug.LogWarning("Insufficient funds!");
    }

    public int GetTestAddAmount() => _testAddAmount;
    public int GetTestSpendAmount() => _testSpendAmount;

    private void OnDestroy()
    {
        _addCurrencyButton.onClick.RemoveAllListeners();
        _spendCurrencyButton.onClick.RemoveAllListeners();
    }


}
