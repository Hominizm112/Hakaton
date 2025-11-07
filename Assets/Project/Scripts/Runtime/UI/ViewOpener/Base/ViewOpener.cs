using UnityEngine;
using GameCore.Utils;
using GameCore.UI;
using UnityEngine.UI;
using Zenject;
using GameCore.Services;
using TriInspector;
using System.Collections.Generic;
using System;
public class ViewOpener : MonoBehaviour
{
    [Group("Type")][HideLabel][SerializeField][Dropdown(nameof(GetTypes))] private string type;
    public Type ScreenType => TypeExtensions.GetType(type);
    private IEnumerable<string> GetTypes() => TypeExtensions.FilterTypes<View>();

    [Inject] private ScreensService _screensService;

    private Button _button;
    private void Awake()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(OpenScreen);
    }

    private void OpenScreen()
    {
        _ = _screensService.OpenAsync(ScreenType);

    }

    private void OnDestroy()
    {
        _button.onClick.RemoveListener(OpenScreen);
    }


}
