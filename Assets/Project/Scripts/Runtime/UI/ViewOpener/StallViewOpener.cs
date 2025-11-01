using GameCore.Services;
using TeaGame.Views;
using UnityEngine;
using Zenject;

public class StallViewOpener : MonoBehaviour, IViewOpener
{
    [Inject] private ScreensService _screensService;

    public void OpenScreen()
    {
        _ = _screensService.OpenAsync<StallView>();

    }

    void Awake()
    {
        OpenScreen();
    }

}
