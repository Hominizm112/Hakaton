using UnityEngine;
using GameCore.Services;
using Zenject;

public class TeaMixViewOpener : MonoBehaviour, IViewOpener
{
    [Inject] private ScreensService _screensService;
    public void OpenScreen()
    {
        _ = _screensService.OpenAsync<TeaMixView>();
    }
}
