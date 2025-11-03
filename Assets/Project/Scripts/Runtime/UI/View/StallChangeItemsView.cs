using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using GameCore.Services;
using GameCore.UI;
using UniRx;
using UnityEngine;
using Zenject;


namespace TeaGame.Views
{
    public class StallChangeItemsView : View<StallChangeItemsViewModel>
    {

        [SerializeField] private ButtonExtended selectItemButton;

        [Inject] private ScreensService _screensService;
        [Inject] private StallViewModel _stallViewModel;
        [Inject] private EventBus _eventBus;

        public ReactiveCommand<ItemData> ItemSelected = new();

        private CompositeDisposable _disposables = new();
        private List<Action> _onDispose = new();
        private ItemData _selectedItem;
        private InventoryView _inventoryView;

        private bool _initialized;


        public override async void Initialize()
        {
            Bind();
            await CreateInventoryView();
            _inventoryView.ItemSelected
                .Subscribe(data => _selectedItem = data)
                .AddTo(_disposables);

            selectItemButton.OnButtonClick += SelectItemCallback;
            _onDispose.Add(() => selectItemButton.OnButtonClick -= SelectItemCallback);

            _initialized = true;
        }

        private async UniTask CreateInventoryView()
        {
            _inventoryView = (InventoryView)await _screensService.OpenAsync(typeof(InventoryView));

        }

        private void SelectItemCallback()
        {
            ItemSelected.Execute(_selectedItem);
            _stallViewModel.TryPlaceItem(_selectedItem);
        }

        private void OnEnable()
        {
            OpenWindow();
        }

        private void OnDisable()
        {
            _eventBus.Publish<ScreenCloseEvent>(new(this));
            _inventoryView?.gameObject.SetActive(false);
        }

        private async void OpenWindow()
        {
            await UniTask.WaitUntil(() => _initialized == true);

            _inventoryView?.gameObject.SetActive(true);
            _eventBus.Publish<ScreenOpenEvent>(new(this));

        }






        public override void Dispose()
        {
            base.Dispose();
            _disposables.Dispose();
            selectItemButton.OnButtonClick -= SelectItemCallback;

            foreach (var item in _onDispose)
            {
                item?.Invoke();
            }

        }


    }
}