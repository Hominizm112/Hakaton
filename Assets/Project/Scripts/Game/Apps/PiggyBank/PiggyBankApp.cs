using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class PiggyBankApp : BaseApp
{
    [SerializeField] List<Sprite> piggySprites;
    [SerializeField] Image piggyImage;

    [Inject] private SaveManager _saveService;
    [Inject] private CurrencyPresenter _currencyPresenter;

    private int _piggyIntegrity = 0;
    private bool _isBroken;
    private const string PIGGY_BANK_SAVE_KEY = "PIGGY_BANK";


    public void UpdatePiggyDisplay()
    {
        _piggyIntegrity++;

        if (_piggyIntegrity >= piggySprites.Count)
        {
            return;
        }

        if (_piggyIntegrity >= piggySprites.Count - 1)
        {
            HandlePiggyDestruction();
        }
        piggyImage.sprite = piggySprites[_piggyIntegrity];
    }

    private void HandlePiggyDestruction()
    {
        _isBroken = true;

        int amountCollected = _saveService.GetInt(PIGGY_BANK_SAVE_KEY);
        _saveService.SetInt(PIGGY_BANK_SAVE_KEY, 0);

        if (amountCollected == 0) return;

        _currencyPresenter.AddCurrency(amountCollected);
    }

    public void AddToPiggyBank()
    {
        if (_isBroken)
        {
            return;
        }

        int keypadInput = _appController.GetApp<KeypadApp>().KeypadInput;
        if (keypadInput == 0)
        {
            return;
        }

        if (_currencyPresenter.TrySpendCurrency(keypadInput))
        {
            int oldValue = _saveService.GetInt(PIGGY_BANK_SAVE_KEY);
            _saveService.SetInt(PIGGY_BANK_SAVE_KEY, oldValue + keypadInput);
        }

    }
}
