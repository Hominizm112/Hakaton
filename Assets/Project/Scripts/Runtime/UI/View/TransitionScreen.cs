using DG.Tweening;
using GameCore.UI;
using TeaGame.Runtime.Services;
using UnityEngine;
using Zenject;

public class TransitionScreen : View
{
    [Inject] private EventBus _eventBus;
    [Inject] private ScenesService _scenesService;
    [SerializeField] private GameObject container;
    [SerializeField] private CanvasGroup canvasGroup;

    public override void Initialize()
    {
        disposables.Add(_eventBus.Subscribe<SceneStartLoadEvent>(_ => Drop()));
        disposables.Add(_eventBus.Subscribe<SceneLoadedEvent>(_ => Lift()));
        if (_scenesService.LoadingScreenLiftRequested)
        {
            Lift();
        }
        else
        {
            Drop();
            Lift();
        }

    }

    public void Drop()
    {
        canvasGroup.alpha = 1;
        _eventBus.Publish<LoadingScreenDropEvent>(new());
        container.SetActive(true);
    }

    public void Lift()
    {
        canvasGroup.DOFade(0, 0.5f).SetEase(Ease.OutSine).OnComplete(() =>
        {
            container.SetActive(false);
            _eventBus.Publish<LoadingScreenLiftEvent>(new());
        });
    }


}
