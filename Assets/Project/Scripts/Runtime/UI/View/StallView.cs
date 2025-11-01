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
        private ParticleSystem _particleEmitter;
        private StallChangeItemsView _changeItemsInBoxesView;

        private CompositeDisposable _disposables = new();


        #region  Init

        public override void Initialize()
        {
            Bind();
            foreach (var box in stallItemBoxes)
            {
                _disposables.Add(box.Subscribe(() => HandleBoxSelection(box)));
            }



            InitializeDragger().Forget();
            InitializeParticleEmitter().Forget();
        }



        private async UniTask InitializeDragger()
        {
            HiddenContainer hiddenContainer = new();

            var handle = Addressables.InstantiateAsync(dragObjectRef, hiddenContainer.Container);
            await handle.Task;

            if (handle.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
            {
                _dragger = handle.Result.GetComponent<Dragger>();
                _dragger.onDragEnd += DespawnItem;

                _placer = handle.Result.GetComponent<Placer>();

                handle.Result.gameObject.SetActive(false);

                _disposables.Add(_dragger);
                _disposables.Add(_placer);
                hiddenContainer.Release(handle.Result.transform);
            }
        }

        private async UniTask InitializeParticleEmitter()
        {
            var handle = Addressables.InstantiateAsync(particleEmitterRef);
            await handle.Task;

            if (handle.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
            {
                _particleEmitter = handle.Result.GetComponent<ParticleSystem>();
            }
        }




        #endregion

        private void HandleBoxSelection(StallItemBox box)
        {
            ViewModel.SelectStallBox(box.id);
            if (ViewModel.CanSpawnNewItem())
            {
                SpawnItem();
            }
        }

        private void SpawnItem()
        {
            _dragger.gameObject.SetActive(true);
            _dragger.StartDrag(bypassCheck: true);
        }

        private void DespawnItem()
        {
            if (!_placer.Placing && _dragger.IsDragging)
            {
                _particleEmitter.transform.position = _dragger.transform.position;
                _particleEmitter.Play();
                _dragger.gameObject.SetActive(false);
            }
        }

        private void ToggleChangeItemsInBoxesView(bool visible)
        {
            _changeItemsInBoxesView.gameObject.SetActive(visible);
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
            _dragger.onDragEnd -= DespawnItem;
            Addressables.ReleaseInstance(_dragger.gameObject);
            Addressables.ReleaseInstance(_particleEmitter.gameObject);
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