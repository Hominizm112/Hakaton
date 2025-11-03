using UnityEngine;
using GameCore.Services;
using Zenject;
using TeaGame.Views;

public class TeaMixViewOpener : MonoBehaviour, IViewOpener
{
    [Inject] private ScreensService _screensService;
    public void OpenScreen()
    {
        _ = _screensService.OpenAsync(typeof(TeaMixView));
    }
}
