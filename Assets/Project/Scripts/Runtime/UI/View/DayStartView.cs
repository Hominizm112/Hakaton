namespace TeaGame.Runtime.UI.View
{
    using System;
    using Cysharp.Threading.Tasks;
    using DG.Tweening;
    using GameCore.UI;
    using UniRx;
    using UnityEngine;
    using Zenject;

    public class DayStartView : View
    {
        [Inject] private EventBus _eventBus;
        [SerializeField] private GameObject container;
        [SerializeField] private RectTransform title;
        [SerializeField] private CanvasGroup titleCanvasGroup;
        [SerializeField] private Vector2 finalSize = Vector2.one;
        [SerializeField] private int showDelay;
        [SerializeField] private float showDuration;
        [SerializeField] private Ease showEase;
        [SerializeField] private float hideDuration;
        [SerializeField] private Ease hideEase;
        [SerializeField] private int hideDelay;



        public override void Initialize()
        {
            container.SetActive(false);
            disposables.Add(_eventBus.Subscribe<LoadingScreenLiftEvent>(_ => Show().Forget()));
        }

        public async UniTask Show()
        {
            await UniTask.Delay(TimeSpan.FromSeconds(showDelay), ignoreTimeScale: true);
            container.SetActive(true);
            await title.DOSizeDelta(finalSize, showDuration).SetEase(showEase).AsyncWaitForCompletion();
            await UniTask.Delay(TimeSpan.FromSeconds(hideDelay), ignoreTimeScale: true);
            titleCanvasGroup.DOFade(0, hideDuration).SetEase(hideEase);
        }
    }
}