using System;
using GameCore.Utils;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace GameCore.UI
{
    [Serializable]
    public class SliderViewBinder : ViewBinder<ReactiveCommand<float>>
    {
        [SerializeField] private Slider _slider;
        private ReactiveCommand<float> _reactiveCommand;
        private bool _isUpdatingFromSlider = false;

        public float SliderValue
        {
            get => _slider.value;
            set => _slider.value = value;
        }
        public Action<float> onValueChange;

        public SliderViewBinder(string id) : base(id)
        {

        }

        public override void Parse(ReactiveCommand<float> value)
        {
            _reactiveCommand = value;

            _slider.onValueChanged.RemoveListener(OnValueChanged);
            _slider.onValueChanged.AddListener(OnValueChanged);

            _reactiveCommand.Subscribe(val =>
            {
                if (!_isUpdatingFromSlider)
                {
                    _slider.value = val;
                }
            });


        }



        public override void Dispose()
        {
            base.Dispose();

            _slider.onValueChanged.RemoveListener(OnValueChanged);
            _reactiveCommand = null;
        }

        private void OnValueChanged(float value)
        {
            _isUpdatingFromSlider = true;
            onValueChange?.Invoke(value);
            _reactiveCommand.Execute(value);
            _isUpdatingFromSlider = false;
        }
    }
}