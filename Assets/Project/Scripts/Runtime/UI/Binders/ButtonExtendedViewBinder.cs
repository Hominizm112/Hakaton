using System;
using UniRx;
using UnityEngine;


namespace GameCore.UI
{
    [Serializable]
    public class ButtonExtendedViewBinder : ViewBinder<ReactiveCommand<MouseButtonClick>>
    {
        [SerializeField] private ButtonExtended _button;
        private ReactiveCommand<MouseButtonClick> _reactiveCommand;

        public Action onMouseDown;
        public Action onMouseUp;

        public ButtonExtendedViewBinder(string id) : base(id)
        {

        }

        public override void Parse(ReactiveCommand<MouseButtonClick> value)
        {
            _reactiveCommand = value;
            _button.OnMouseDown.AddListener(OnMouseDown);
            _button.OnMouseUp.AddListener(OnMouseUp);
        }

        public override void Dispose()
        {
            base.Dispose();

            _button.OnMouseDown.RemoveListener(OnMouseDown);
            _button.OnMouseUp.RemoveListener(OnMouseUp);
            _reactiveCommand = null;
        }


        private void OnMouseDown()
        {
            _reactiveCommand.Execute(MouseButtonClick.Down);
            onMouseDown?.Invoke();
        }

        private void OnMouseUp()
        {
            _reactiveCommand.Execute(MouseButtonClick.Up);
            onMouseUp?.Invoke();
        }
    }
}

public enum MouseButtonClick
{
    Down,
    Up
}