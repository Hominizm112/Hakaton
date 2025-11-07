using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.Localization.Components;

public class WordView : MonoBehaviour, IDisposable
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
            return;
        }

        WordOfPower = wordOfPower;
        _selectCallback = selectCallback;

        UpdateDisplay();
    }

    public void ResetWord()
    {
        WordOfPower = null;
        _selectCallback = null;
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

    public void Dispose()
    {
        WordOfPower = null;
        _selectCallback = null;

        if (_button != null)
        {
            _button.onClick.RemoveAllListeners();
        }
    }


}
