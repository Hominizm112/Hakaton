using GameCore.UI;
using UniRx;
using Zenject;
using CollectionActionData = InventoryService.CollectionActionData;
public class InventoryViewModel : ViewModel
{
    private ReactiveCollection<ItemData> _items = new();
    public IReadOnlyReactiveCollection<ItemData> Items => _items;

    public ReactiveCommand<CollectionActionData> OnRefreshItems = new();
    private ReactiveProperty<ItemData> _selectedItem = new();
    public IReadOnlyReactiveProperty<ItemData> SelectedItem => _selectedItem;

    private CompositeDisposable _disposables = new();

    [Inject] private InventoryService _inventoryService;

    public override void Initialize()
    {
        SubscribeForItemsChanged();
    }

    private void SubscribeForItemsChanged()
    {

        foreach (var item in _inventoryService.InventoryData.Items)
        {
            _items.Add(item);
        }


        _inventoryService.InventoryData.Items
            .ObserveAdd()
            .Subscribe(item =>
            {
                _items.Add(item.Value);
                HandleItemsUpdate(new(item.Value, InventoryService.CollectionAction.Add));
            })
            .AddTo(_disposables);

        _inventoryService.InventoryData.Items
            .ObserveRemove()
            .Subscribe(item =>
            {
                _items.Remove(item.Value);
                HandleItemsUpdate(new(item.Value, InventoryService.CollectionAction.Remove));
            })
            .AddTo(_disposables);

    }

    private void HandleItemsUpdate(CollectionActionData collectionActionData)
    {
        OnRefreshItems.Execute(collectionActionData);
    }

    public override void Dispose()
    {
        base.Dispose();
        _disposables.Dispose();
    }
}
