using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class TransitionScreen : MonoBehaviour, IInitializable
{
    [SerializeField] private AnimationSettings animationSettings;
    [SerializeField] private Image image;
    [SerializeField] private Color _transitionColor;
    private Color _transparentColor;

    public void Initialize(Mediator mediator)
    {
        mediator.RegisterService<TransitionScreen>(this);
        _transparentColor = _transitionColor;
        _transparentColor.a = 0;
        image.color = _transparentColor;
    }
    public void StartTransition(Action midTransitionCallback = null)
    {
        StartTransitionAnimation(midTransitionCallback);
    }

    public void EndTransition(Action endTransitionCallback = null)
    {
        EndTransitionAnimation(endTransitionCallback);
    }

    private void StartTransitionAnimation(Action callback)
    {
        image.gameObject.SetActive(true);
        image.DOColor(_transitionColor, animationSettings.duration).SetEase(animationSettings.ease).OnComplete(() => callback?.Invoke());
    }

    private void EndTransitionAnimation(Action callback)
    {
        image.DOColor(_transparentColor, animationSettings.duration).SetEase(animationSettings.ease).OnComplete(() => EndTransitionHandler(callback));
    }

    private void EndTransitionHandler(Action callback)
    {
        image.gameObject.SetActive(false);
        callback?.Invoke();
    }

}
