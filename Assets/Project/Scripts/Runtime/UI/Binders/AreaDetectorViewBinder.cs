
using System;
using TMPro;
using UniRx;
using UnityEngine;

namespace GameCore.UI
{
    [Serializable]
    public class AreaDetectorViewBinder : ViewBinder<ReactiveCommand<ItemData>>
    {
        [SerializeField] private AreaDetector _areaDetector;
        private ReactiveCommand<ItemData> _placeInAreaCommand;


        public AreaDetectorViewBinder(string id) : base(id)
        {
        }

        public override void Parse(ReactiveCommand<ItemData> value)
        {
            _placeInAreaCommand = value;
            _areaDetector.PlacedItem
                .Subscribe(item => _placeInAreaCommand.Execute(item))
                .AddTo(disposables);

        }

        public override void Dispose()
        {
            base.Dispose();
            _placeInAreaCommand = null;
        }
    }
}