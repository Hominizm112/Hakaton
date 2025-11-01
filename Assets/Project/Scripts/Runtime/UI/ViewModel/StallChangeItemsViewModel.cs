using GameCore.UI;
using UniRx;

public class StallChangeItemsViewModel : ViewModel
{
    private CompositeDisposable _disposables = new();
    public override void Initialize()
    {
    }

    public override void Dispose()
    {
        base.Dispose();
        _disposables.Dispose();
    }

}