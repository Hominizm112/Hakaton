using System;
using Cysharp.Threading.Tasks;
using GameCore.Services;
using TeaGame.Views;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class StallChangeItemsViewOpener : MonoBehaviour, IViewOpener, IDisposable
{
    [Inject] private ScreensService _screensService;

    private bool _oppened;
    private Button _button;
    private StallChangeItemsView _stallChangeItemsView;

    private void Awake()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(OpenScreen);
    }
    public async UniTask ToggleScreen()
    {
        if (!_oppened)
        {
            _stallChangeItemsView = (StallChangeItemsView)await _screensService.OpenAsync(typeof(StallChangeItemsView));
            _oppened = true;
        }
        else
        {
            if (_stallChangeItemsView != null)
            {
                _stallChangeItemsView.gameObject.SetActive(false);

            }
            _oppened = false;
        }
    }

    public void Dispose()
    {
        _button.onClick.RemoveListener(OpenScreen);
    }

    public void OpenScreen()
    {
        ToggleScreen().Forget();
    }
}