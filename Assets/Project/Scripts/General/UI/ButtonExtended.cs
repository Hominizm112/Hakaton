using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.WSA;

[RequireComponent(typeof(Button))]
public abstract class ButtonExtended : MonoBehaviour
{
    protected Button _button;
    protected Action OnButtonClick;
    protected virtual void Awake()
    {
        _button = GetComponent<Button>();
        OnButtonClick += HandleClick;
        _button.onClick.AddListener(() => OnButtonClick?.Invoke());
    }

    protected virtual void OnDestroy()
    {
        _button.onClick.RemoveAllListeners();
    }

    protected abstract void HandleClick();
}
