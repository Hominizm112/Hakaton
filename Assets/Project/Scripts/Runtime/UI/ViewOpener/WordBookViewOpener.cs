using System;
using GameCore.Services;
using TeaGame.Views;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class WordBookViewOpener : MonoBehaviour, IViewOpener
{
    [Inject] private ScreensService _screensService;
    private WordBookView _view;

    private Button _button;

    // private void Awake()
    // {
    //     _button = GetComponent<Button>();
    //     _button.onClick.AddListener(OpenScreen);
    // }
    public void OpenScreen()
    {
        _ = _screensService.OpenAsync(typeof(WordBookView));
    }

    // public void Dispose()
    // {
    //     _button.onClick.RemoveListener(OpenScreen);
    // }


}