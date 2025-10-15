using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using Unity.VisualScripting;
using UnityEngine.Localization.Components;

public class WordView : MonoBehaviour
{
    [SerializeField] private LocalizeStringEvent localizeStringEvent;

    public WordOfPower WordOfPower { get; private set; }

    private Action<WordOfPower> _selectCallback;
    private Button _button;

    private void Awake()
    {
        _button = GetComponent<Button>();
        if (_button == null)
        {
            ColorfulDebug.LogError("WordView requires a Button component");
        }
    }


    public void SetWord(WordOfPower wordOfPower, Action<WordOfPower> selectCallback)
    {
        if (wordOfPower == null)
        {
            ColorfulDebug.LogError("Attempted to set null word to WordView");
            return;
        }

        WordOfPower = wordOfPower;
        _selectCallback = selectCallback;

        UpdateDisplay();
    }

    private void UpdateDisplay()
    {
        if (localizeStringEvent != null)
        {
            localizeStringEvent.StringReference = WordOfPower.word;
        }

    }

    private void OnButtonClick()
    {
        _selectCallback?.Invoke(WordOfPower);
    }

    private void OnEnable()
    {
        if (_button != null)
        {
            _button.onClick.AddListener(OnButtonClick);
        }
    }

    private void OnDisable()
    {
        if (_button != null)
        {
            _button.onClick.RemoveListener(OnButtonClick);
        }
    }

    public void Cleanup()
    {
        WordOfPower = null;
        _selectCallback = null;

        if (_button != null)
        {
            _button.onClick.RemoveAllListeners();
        }
    }


}
