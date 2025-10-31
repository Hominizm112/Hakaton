using System;
using UniRx;

public class InventoryViewModel : IDisposable
{
    private readonly IInventoryService _inventoryService;
    private readonly CompositeDisposable _disposables = new CompositeDisposable();

    public IReadOnlyReactiveCollection<ItemModel> Items => _inventoryService.Items;
    public ReactiveCommand<ItemModel> SellItemCommand { get; } = new();

    public InventoryViewModel(IInventoryService inventoryService)
    {
        _inventoryService = inventoryService;

        SellItemCommand.Subscribe(SellItem).AddTo(_disposables);
    }

    private void SellItem(ItemModel item)
    {
        _inventoryService.RemoveItem(item.Id);
    }

    public void Dispose()
    {
        foreach (var item in _disposables)
        {
            item.Dispose();
        }
    }
}
