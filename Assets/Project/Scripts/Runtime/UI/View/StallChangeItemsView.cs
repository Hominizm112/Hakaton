using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using GameCore.Services;
using GameCore.UI;
using UniRx;
using Zenject;


namespace TeaGame.Views
{
    public class StallChangeItemsView : View<StallChangeItemsViewModel>
    {
        private CompositeDisposable _disposables = new();

        private InventoryView _inventoryView;

        [Inject] private ScreensService _screensService;


        public override void Initialize()
        {
            Bind();
            CreateInventoryView().Forget();
        }

        private async UniTask CreateInventoryView()
        {
            _inventoryView = await _screensService.OpenAsync<InventoryView>();

        }



        private void OnEnable()
        {
            _inventoryView?.gameObject.SetActive(true);
        }

        private void OnDisable()
        {
            _inventoryView?.gameObject.SetActive(false);
        }






        public override void Dispose()
        {
            base.Dispose();
            _disposables.Dispose();
        }


    }
}