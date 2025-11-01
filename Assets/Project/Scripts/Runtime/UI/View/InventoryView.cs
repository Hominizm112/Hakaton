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

        private CompositeDisposable _disposables = new();
        private List<ItemView> _itemViews = new();


        public override void Initialize()
        {
            Bind();
            InitializeStartingItems().Forget();

            ViewModel.OnRefreshItems
                .Subscribe(collectionActionData => FetchCommodityViewsAsync(collectionActionData).Forget())
                .AddTo(_disposables);
        }


        private async UniTask InitializeStartingItems()
        {
            foreach (var item in ViewModel.Items)
            {
                await HandleItemAdded(item);
            }
        }


        private void OnEnable()
        {
        }

        private void OnDisable()
        {

        }




        public async UniTask FetchCommodityViewsAsync(InventoryService.CollectionActionData collectionActionData)
        {

            switch (collectionActionData.CollectionAction)
            {
                case InventoryService.CollectionAction.Add:
                    HandleItemAdded(collectionActionData.ItemData).Forget();
                    break;

                case InventoryService.CollectionAction.Remove:
                    HandleItemRemoved(collectionActionData.ItemData);
                    break;
            }



        }

        private async UniTask HandleItemAdded(ItemData item)
        {
            UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus asyncOperationStatus = UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.None;

            do
            {
                if (TryFindFreeItemView(out ItemView freeView))
                {
                    freeView.Initialize(item);
                    freeView.gameObject.SetActive(true);
                    return;
                    // asyncOperationStatus = UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded;
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
            print(_itemViews.Count);
            view = _itemViews.Find(r => !r.gameObject.activeSelf);
            return view != null;
        }

        private async UniTask CreateCommodityView()
        {
            HiddenContainer hiddenContainer = new();

            var handle = Addressables.InstantiateAsync(itemViewRef, hiddenContainer.Container);
            await handle.Task;

            if (handle.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
            {
                var result = handle.Result.GetComponent<ItemView>();
                _itemViews.Add(result);
                _disposables.Add(result);

                result.gameObject.SetActive(false);
                result.transform.SetParent(itemViewsHolder);

                hiddenContainer.Release(result.transform);

            }
        }


        public override void Dispose()
        {
            base.Dispose();
            _disposables.Dispose();
        }

    }
}