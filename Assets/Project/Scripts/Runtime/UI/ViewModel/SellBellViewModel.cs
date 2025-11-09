using GameCore.UI;
using TeaGame.States;
using UniRx;
using Zenject;

public class SellBellViewModel : ViewModel
{
    private RefTypeViewModelBinder<ReactiveCommand<MouseButtonClick>> _sellButton = new("sellButton");
    private RefTypeViewModelBinder<ReactiveCommand<ItemData>> _sellArea = new("sellArea");

    private ReactiveProperty<ItemData> _itemInSellArea = new();
    private bool _isCustomerAtStall;

    [Inject] private StallState _stallState;

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

        _stallState.IsCustomerAtStall
            .Subscribe(val => _isCustomerAtStall = val)
            .AddTo(disposables);

    }

    private void TrySellItem()
    {
        if (CanSell())
        {
            _stallState.Sell(_itemInSellArea.Value);
        }

    }

    private bool CanSell()
    {
        return _isCustomerAtStall
            && _itemInSellArea.Value != null;
    }
}