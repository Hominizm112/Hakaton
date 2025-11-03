using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using GameCore.UI;
using UniRx;
using UnityEngine;
using UnityEngine.AddressableAssets;


namespace TeaGame.Views
{
    public class InventoryView : View<InventoryViewModel>
    {
        [SerializeField] private Transform itemViewsHolder;
        [SerializeField] private AssetReference itemViewRef;

        public ReactiveCommand<ItemData> ItemSelected = new();

        private CompositeDisposable _disposables = new();
        private List<ItemView> _itemViews = new();

        private List<Action> _onDispose = new();

        public override void Initialize()
        {
            Bind();
            InitializeStartingItems();

            ViewModel.OnRefreshItems
                .Subscribe(collectionActionData => FetchCommodityViewsAsync(collectionActionData))
                .AddTo(_disposables);
        }


        private void InitializeStartingItems()
        {
            foreach (var item in ViewModel.Items)
            {
                HandleItemAdded(item);
            }
        }


        private void OnEnable()
        {
        }

        private void OnDisable()
        {

        }




        public void FetchCommodityViewsAsync(InventoryService.CollectionActionData collectionActionData)
        {

            switch (collectionActionData.CollectionAction)
            {
                case InventoryService.CollectionAction.Add:
                    HandleItemAdded(collectionActionData.ItemData);
                    break;

                case InventoryService.CollectionAction.Remove:
                    HandleItemRemoved(collectionActionData.ItemData);
                    break;
            }



        }

        private async void HandleItemAdded(ItemData item)
        {
            UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus asyncOperationStatus = UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.None;

            do
            {
                if (TryFindFreeItemView(out ItemView freeView))
                {
                    freeView.Initialize(item);
                    freeView.SelectButton.OnButtonClick += () => SelectItemCallback(item);
                    _onDispose.Add(() => freeView.SelectButton.OnButtonClick -= () => SelectItemCallback(item));

                    freeView.gameObject.SetActive(true);
                    asyncOperationStatus = UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded;
                }
                else
                {
                    await CreateCommodityView();
                }
            } while (asyncOperationStatus != UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded);
        }

        private void HandleItemRemoved(ItemData item)
        {
            var view = _itemViews.Find(view => view.ItemData == item);

            if (view)
            {
                view.gameObject.SetActive(false);
            }

        }

        private bool TryFindFreeItemView(out ItemView view)
        {
            view = _itemViews.Find(r => !r.gameObject.activeSelf);
            return view != null;
        }

        private async UniTask CreateCommodityView()
        {
            HiddenContainer hiddenContainer = new(itemViewsHolder);

            var handle = Addressables.InstantiateAsync(itemViewRef, hiddenContainer.Container);
            await handle.Task;

            if (handle.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
            {
                var result = handle.Result.GetComponent<ItemView>();
                _itemViews.Add(result);
                _disposables.Add(result);

                hiddenContainer.Release(result.transform);
                result.transform.localScale = Vector3.one;
                hiddenContainer.Dispose();

            }
        }

        private void SelectItemCallback(ItemData item)
        {
            ItemSelected.Execute(item);
        }


        public override void Dispose()
        {
            base.Dispose();
            _disposables.Dispose();

            foreach (var item in _onDispose)
            {
                item?.Invoke();
            }
        }

    }
}