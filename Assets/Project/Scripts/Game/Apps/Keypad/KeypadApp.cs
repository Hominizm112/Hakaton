using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class KeypadApp : BaseApp
{


    [SerializeField] private List<KeypadKey> keypadKeys;
    [SerializeField] private Button removeButton;
    [SerializeField] private TMP_Text keypadText;
    [SerializeField] private int maxSymbolCount;

    private int _currentKeypadInput = 0;
    public int KeypadInput => _currentKeypadInput;
    private AudioHub _audioHub;

    protected override void Awake()
    {
        base.Awake();
        oppenable = false;
        foreach (var item in keypadKeys)
        {
            item.button.onClick.AddListener(() => UpdateKeypadInput(item.number));
        }
        removeButton.onClick.AddListener(() => RemoveFromKeypadInput());
        removeButton.GetComponent<BaseButtonExtended>().OnButtonHold += RemoveFromKeypadInput;

        _audioHub = Mediator.Instance.GetService<AudioHub>();

        OnClose.AddListener(HandleClose);
    }

    private void HandleClose()
    {
        _currentKeypadInput = 0;
        UpdateKeypadInputDisplay(0);
    }




    private void UpdateKeypadInput(int number)
    {
        if (_currentKeypadInput.ToString().Length >= maxSymbolCount)
        {
            return;
        }
        _currentKeypadInput = _currentKeypadInput * 10 + number;
        UpdateKeypadInputDisplay(_currentKeypadInput);
    }

    private void RemoveFromKeypadInput()
    {
        if (_currentKeypadInput <= 0)
        {
            return;
        }

        _audioHub.PlayOneShot(SoundType.PC_TextChangeSound, 0.2f);
        _currentKeypadInput = _currentKeypadInput / 10;
        UpdateKeypadInputDisplay(_currentKeypadInput);
    }

    private void UpdateKeypadInputDisplay(long number)
    {
        keypadText.text = number.ToString();
    }

    private void OnDestroy()
    {
        foreach (var item in keypadKeys)
        {
            item.button.onClick.RemoveAllListeners();
        }
        removeButton.onClick.RemoveAllListeners();

    }
}

[Serializable]
public struct KeypadKey
{
    public Button button;
    public int number;
}
