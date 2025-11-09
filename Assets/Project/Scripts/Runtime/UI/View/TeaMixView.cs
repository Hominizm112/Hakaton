using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using GameCore.UI;
using UnityEngine;


namespace TeaGame.Views
{
    public class TeaMixView : View<TeaMixViewModel>
    {

        [Header("Scene References")]
        [SerializeField] private ButtonExtendedViewBinder holdButton = new("holdButton");
        [SerializeField] private SliderViewBinder completionSlider = new("completionSlider");
        [SerializeField] private Braket[] brakets;
        [SerializeField] private GameObject teaMissingScreen;


        [Header("Settings")]
        [SerializeField] private RangeFloat perfectRange;
        [SerializeField] private RangeFloat goodRange;

        private Tween _sliderTween;
        private bool _initialized;


        private bool _canMix;

        public override void Initialize()
        {
            Bind(holdButton, completionSlider);

            holdButton.onMouseDown += StartHold;
            holdButton.onMouseUp += EndHold;

            completionSlider.onValueChange += OnSliderValueChange;

            ViewModel.Initialize(perfectRange, goodRange);

            _initialized = true;
        }

        public void OnEnable()
        {
            OnOpenAsync();
        }


        public void OnDisable()
        {
            OnClose();
        }

        public void OnClose()
        {
            completionSlider.SliderValue = 0;
        }

        public async void OnOpenAsync()
        {
            await UniTask.WaitUntil(() => _initialized == true);

            _canMix = ViewModel.IsTeaToCookExists();
            teaMissingScreen.SetActive(!_canMix);
        }

        public void StartHold()
        {
            if (!_canMix)
            {
                return;
            }

            float value = 0f;
            foreach (var braket in brakets) braket.animating = false;
            _sliderTween = DOTween.To(() => value, x => value = x, 1, 2)
                .OnUpdate(() => completionSlider.SliderValue = value)
                .SetEase(Ease.Linear);
        }

        public void EndHold()
        {
            if (_sliderTween != null)
            {
                _sliderTween?.Kill();
                _sliderTween = null;
            }

            _canMix = false;

            float sliderValue = completionSlider.SliderValue;

            ViewModel.Mix(sliderValue);

        }

        public void OnSliderValueChange(float value)
        {
            Braket currentBraket = new();

            foreach (var braket in brakets)
            {
                if (value > braket.value && !braket.animating)
                {
                    currentBraket = braket;
                    break;
                }
            }

            if (currentBraket.obj != null && !currentBraket.animating)
            {
                currentBraket.animating = true;
                currentBraket.obj.transform.DOPunchScale(new Vector3(0, 0.5f, 0), 0.25f, 1, 1);
            }
        }

        public override void Dispose()
        {
            base.Dispose();
            holdButton.onMouseDown -= StartHold;
            holdButton.onMouseUp -= EndHold;

            completionSlider.onValueChange -= OnSliderValueChange;

        }



        [Serializable]
        private class Braket
        {
            public GameObject obj;
            public float value;
            [HideInInspector] public bool animating;
        }

    }
}