using System;
using GameCore.Services;
using GameUI.Views;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class WordBookViewOpener : MonoBehaviour, IViewOpener, IDisposable
{
    [Inject] private ScreensService _screensService;

    private Button _button;

    private void Awake()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(OpenScreen);
    }
    public void OpenScreen()
    {
        _ = _screensService.OpenAsync<WordBookView>();
    }

    public void Dispose()
    {
        _button.onClick.RemoveListener(OpenScreen);
    }


}