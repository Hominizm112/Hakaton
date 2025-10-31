using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class TeaMixService : MonoBehaviour
{
    // [Header("Scene References")]
    // [SerializeField] private GameObject teaCookWindow;
    // [SerializeField] private Slider slider;


    // [Header("Settings")]
    // [SerializeField] private RangeFloat prefectRange;
    // [SerializeField] private RangeFloat goodRange;
    // [SerializeField] private float duration;

    // [Inject] private StallService _stallService;
    // [Inject] private WordBook _wordBook;

    // Tween _sliderTween;
    // Timer timer = new();

    // private void Awake()
    // {
    //     slider.onValueChanged.AddListener(HandleValueChange);
    // }

    // public void StartHold()
    // {
    //     slider.value = 0;
    //     // foreach (var item in brakets) item.animating = false;    


    //     _sliderTween = slider.DOValue(1, duration).SetEase(Ease.Linear).OnComplete(() => EndHold(true));

    // }

    // public void EndHold(bool fromAnimation = false)
    // {
    //     if (!fromAnimation)
    //     {
    //         if (!_sliderTween.IsPlaying())
    //         {
    //             return;
    //         }
    //         if (!_sliderTween.IsComplete())
    //         {
    //             _sliderTween?.Kill();
    //         }

    //     }

    //     float sliderValue = slider.value;
    //     float quality = .25f;
    //     if (prefectRange.InRange(sliderValue))
    //     {
    //         quality = 1f;
    //     }
    //     else if (goodRange.InRange(sliderValue))
    //     {
    //         quality = .5f;
    //     }

    //     Mix(quality);

    // }

    // private void HandleValueChange(float value)
    // {
    //     // Braket braket = new();

    //     // foreach (var item in brakets)
    //     // {
    //     //     if (value > item.value && !item.animating)
    //     //     {
    //     //         braket = item;
    //     //         break;
    //     //     }
    //     // }

    //     // if (braket != null && braket.obj != null && !braket.animating)
    //     // {
    //     //     braket.animating = true;
    //     //     braket.obj.transform.DOPunchScale(new Vector3(0, 0.5f, 0), 0.25f, 1, 1);
    //     // }
    // }


    // private void Mix(float quality)
    // {
    //     // var mixedTea = TeaMixer.MixTea(_stallService.SelectedCommodity, _wordBook.GetSelectedWords());

    //     DOVirtual.DelayedCall(.5f, () =>
    //     {
    //         teaCookWindow.SetActive(false);
    //         // _stallService.SetTeaReady(mixedTea, quality);
    //         slider.value = 0;
    //     });

    // }




}

