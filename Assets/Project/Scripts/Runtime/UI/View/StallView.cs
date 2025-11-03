using System;
using Cysharp.Threading.Tasks;
using GameCore.UI;
using UniRx;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace TeaGame.Views
{
    public class StallView : View<StallViewModel>, Window
    {
        [SerializeField] private StallItemBox[] stallItemBoxes;
        [SerializeField] private AssetReference particleEmitterRef;
        [SerializeField] private AssetReference dragObjectRef;


        private Dragger _dragger;
        private Placer _placer;

        private CompositeDisposable _disposables = new();


        public override async void Initialize()
        {
            Bind();
            foreach (var box in stallItemBoxes)
            {
                _disposables.Add(box.Subscribe(() => ViewModel.SelectStallBox(box.id)));
            }

            ViewModel.SpawnItem
                .Subscribe(_ => SpawnItem())
                .AddTo(_disposables);


            InitializeAddressables();
        }

        private async void InitializeAddressables()
        {
            (_dragger, _placer) = await ViewModel.InitializeDragger(dragObjectRef);
        }

        private void SpawnItem()
        {
            _dragger.gameObject.SetActive(true);
            _placer.SetContainingItem(ViewModel.GetItemInSelectedBox());
            _dragger.StartDrag(bypassCheck: true);
        }




        public void OnEnable()
        {
            OnOpen();
        }
        public void OnDisable()
        {
            OnClose();
        }
        public void OnClose()
        {
        }

        public void OnOpen()
        {
        }

        public override void Dispose()
        {
            base.Dispose();
            _disposables.Dispose();


        }


    }

    [Serializable]
    public struct StallItemBox : IDisposable
    {
        public ButtonExtended button;
        public string id;

        private Action callback;

        public IDisposable Subscribe(Action callback)
        {
            this.callback = callback;
            button.OnMouseDown.AddListener(() => callback());
            return this;
        }

        public void Dispose()
        {
            if (callback != null)
            {
                Action localCallback = callback;
                button.OnMouseDown.RemoveListener(() => localCallback());
                callback = null;
            }
        }
    }
}