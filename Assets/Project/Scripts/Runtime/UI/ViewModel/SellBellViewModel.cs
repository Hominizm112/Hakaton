using GameCore.UI;
using UniRx;
using UnityEngine;

public class SellBellViewModel : ViewModel
{
    private RefTypeViewModelBinder<ReactiveCommand<MouseButtonClick>> _sellButton = new("sellButton");
    private RefTypeViewModelBinder<ReactiveCommand<ItemData>> _sellArea = new("sellArea");

    private ReactiveProperty<ItemData> _itemInSellArea = new();

    public override void Initialize()
    {
        Bind(_sellButton, _sellArea);

        _sellButton.Value
            .Subscribe(mbc =>
            {
                if (mbc == MouseButtonClick.Up)
                {
                    TrySellItem();
                }
            })
            .AddTo(disposables);

        _sellArea.Value
            .Subscribe(item => _itemInSellArea.Value = item)
            .AddTo(disposables);

    }

    private void TrySellItem()
    {
        if (_itemInSellArea.Value == null)
        {
            throw new System.ArgumentNullException("Item was not found in sell area");
        }

        Debug.Log("sold");


    }
}