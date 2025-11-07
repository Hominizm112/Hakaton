using GameCore.Behaviours;
using GameCore.UI;
using GameCore.Utils;
using TeaGame.Services;
using UniRx;
using Zenject;

public class TeaMixViewModel : ViewModel
{

    private RefTypeViewModelBinder<ReactiveCommand<MouseButtonClick>> _holdButton = new("holdButton");
    private RefTypeViewModelBinder<ReactiveCommand<float>> _completionSlider = new("completionSlider");

    private RangeFloat _perfectRange, _goodRange;
    private CompositeDisposable _disposables = new();

    [Inject] TeaMixerService _teaMixerService;
    [Inject] ObjectRegistry _objectRegistry;

    public override void Initialize()
    {
        Bind(_holdButton, _completionSlider);
        _holdButton.Value.Subscribe(HandleButtonClick).AddTo(_disposables);
        _completionSlider.Value.Subscribe(HandleValueChange).AddTo(_disposables);
    }

    public void Initialize(RangeFloat perfectRange, RangeFloat goodRange)
    {
        _perfectRange = perfectRange;
        _goodRange = goodRange;

    }

    private void HandleValueChange(float value)
    {

    }

    private void HandleButtonClick(MouseButtonClick value)
    {
        switch (value)
        {
            case MouseButtonClick.Down:

                break;
            case MouseButtonClick.Up:

                break;
        }
    }

    private void HandleButtonDown()
    {
        _completionSlider.Value = 0;
    }


    private void HandleButtonUp()
    {

    }

    public void Mix(float value)
    {

        // float quality = 0.25f;
        // if (_perfectRange.InRange(value))
        // {
        //     quality = 1f;
        // }
        // else if (_goodRange.InRange(value))
        // {
        //     quality = 0.5f;
        // }

        var mixedTea = _teaMixerService.MixTea();
        var placer = _objectRegistry.Get<Placer>();
        placer.SetContainingItem(mixedTea);


    }

    public bool IsTeaToCookExists()
    {
        return _teaMixerService.TeaToCook.Value != null
        && _teaMixerService.TeaToCook.HasValue
        && _teaMixerService.TeaToCook.Value.itemTag.Value == ItemTag.TeaBase;
    }

    public override void Dispose()
    {
        base.Dispose();
        _disposables.Dispose();
    }


}
