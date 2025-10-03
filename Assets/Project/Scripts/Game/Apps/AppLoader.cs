using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

public class AppLoader : MonoBehaviour
{
    [Header("Slider settings")]
    [SerializeField] private RectTransform sliderRectTransform;
    [SerializeField] private List<LoadProgressKey> loadProgressKeys;
    [SerializeField] private GameObject loadingScreen;


    public Action<int> OnLoadingProgressUpdate;
    public Action OnLoadingComplete;
    public UnityEvent OnLoadingCompleteAction;

    private float _maxLength;
    private Coroutine _loadingCoroutine;
    private int _currentStep;


    private void Awake()
    {
        _maxLength = sliderRectTransform.sizeDelta.x;
        ResetLoader();
    }


    public void StartLoading()
    {
        loadingScreen.SetActive(true);
        ResetLoader();
        _loadingCoroutine = StartCoroutine(LoadApp(0));
    }

    public void StopLoading()
    {
        if (_loadingCoroutine != null)
        {
            StopCoroutine(_loadingCoroutine);
            _loadingCoroutine = null;
        }
    }

    private void ResetLoader()
    {
        sliderRectTransform.sizeDelta = new Vector2(0, sliderRectTransform.sizeDelta.y);
        _currentStep = 0;
        DOTween.Kill(sliderRectTransform);
    }

    private IEnumerator LoadApp(int step)
    {
        _currentStep = step;

        if (step >= loadProgressKeys.Count)
        {
            yield break;
        }

        yield return new WaitForSeconds(loadProgressKeys[step].stepTime);

        Mediator.Instance.GetService<AudioHub>().PlayOneShot(SoundType.PC_LoadAppSound, 0.2f);
        UpdateView(loadProgressKeys[step].targetPrecentage, step == loadProgressKeys.Count - 1);
        OnLoadingProgressUpdate?.Invoke(step);

        _loadingCoroutine = StartCoroutine(LoadApp(step + 1));
    }

    private void UpdateView(float width, bool isLast)
    {
        sliderRectTransform.DOSizeDelta(
            new Vector2(_maxLength / 100 * width, sliderRectTransform.sizeDelta.y),
            0.2f
        ).SetEase(Ease.OutExpo).OnComplete(() =>
        {
            if (isLast)
            {
                HandleLoadComplete();
            }
        });
    }


    private void HandleLoadComplete()
    {
        print("Loading completed");
        OnLoadingComplete?.Invoke();
        OnLoadingCompleteAction?.Invoke();
        OnLoadingComplete = null;

    }


}

[Serializable]
public struct LoadProgressKey
{
    public float targetPrecentage;
    public float stepTime;
}