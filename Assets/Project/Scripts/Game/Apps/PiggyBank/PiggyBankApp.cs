using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PiggyBankApp : BaseApp
{
    [SerializeField] List<Sprite> piggySprites;
    [SerializeField] Image piggyImage;
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

        SaveManager saveManager = _mediator.GetService<SaveManager>();
        int amountCollected = saveManager.GetInt(PIGGY_BANK_SAVE_KEY);
        saveManager.SetInt(PIGGY_BANK_SAVE_KEY, 0);

        if (amountCollected == 0) return;

        _mediator.GetService<CurrencyPresenter>().AddCurrency(amountCollected);
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

        if (_mediator.GetService<CurrencyPresenter>().TrySpendCurrency(keypadInput))
        {
            SaveManager saveManager = _mediator.GetService<SaveManager>();
            int oldValue = saveManager.GetInt(PIGGY_BANK_SAVE_KEY);
            saveManager.SetInt(PIGGY_BANK_SAVE_KEY, oldValue + keypadInput);
        }

    }
}
