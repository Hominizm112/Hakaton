using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using GameCore.UI;
using TeaGame.Services;
using UniRx;
using Zenject;

public class WordBookViewModel : ViewModel
{
    private RefTypeViewModelBinder<ReactiveCommand<MouseButtonClick>> _addWordToSelectedButton = new("addWordToSelectedButton");
    private RefTypeViewModelBinder<ReactiveCommand<MouseButtonClick>> _removeWordFromSelectedButton = new("removeWordFromSelectedButton");
    private ReactiveProperty<List<WordOfPower>> _wordOfPowers = new();
    public IReadOnlyReactiveProperty<List<WordOfPower>> WordOfPowers => _wordOfPowers;

    private ReactiveProperty<WordOfPower> _selectedWord = new();
    public IReadOnlyReactiveProperty<WordOfPower> SelectedWord => _selectedWord;

    private ReactiveCollection<WordOfPower> _selectedWords = new();
    public IReadOnlyReactiveCollection<WordOfPower> SelectedWords => _selectedWords;

    private ReactiveProperty<ItemData> _selectedItem = new();
    public IReadOnlyReactiveProperty<ItemData> SelectedItem => _selectedItem;

    private ReactiveProperty<int> _wordCap = new();
    public IReadOnlyReactiveProperty<int> WordCap => _wordCap;

    [Inject] WordBookService _wordBookService;
    [Inject] TeaMixerService _teaMixerService;

    private CompositeDisposable _disposables = new();

    public override void Initialize()
    {
        Bind(_addWordToSelectedButton, _removeWordFromSelectedButton);

        _addWordToSelectedButton.Value.Subscribe(mbc =>
            {
                if (mbc == MouseButtonClick.Up)
                    AddToSelected();
            })
            .AddTo(_disposables);


        _removeWordFromSelectedButton.Value.Subscribe(mbc =>
            {
                if (mbc == MouseButtonClick.Up)
                    RemoveFromSelected();
            })
            .AddTo(_disposables);

        RefreshWordOfPowers().Forget();

        _wordBookService.SelectedTeaForConstruct
            .Subscribe(item =>
            {
                _selectedItem.Value = item;
            })
            .AddTo(_disposables);

        _selectedWords
            .ObserveAdd()
            .Subscribe(change =>
            {
                _teaMixerService.wordsForTea.Add(change.Value);
            })
            .AddTo(_disposables);

        _selectedWords
            .ObserveRemove()
            .Subscribe(change =>
            {
                _teaMixerService.wordsForTea.Remove(change.Value);
            })
            .AddTo(_disposables);


        _selectedItem
            .Subscribe(item =>
            {
                if (item == null)
                {
                    _wordCap.Value = 0;
                    for (int i = _selectedWords.Count - 1; i >= 0; i--)
                    {
                        _selectedWords.RemoveAt(i);
                    }
                    return;
                }
                _wordCap.Value = item.GetConfig<TeaConfig>().wordCap;

            })
            .AddTo(_disposables);


        _selectedItem.Value = _wordBookService.SelectedTeaForConstruct.Value;

    }

    public async UniTask RefreshWordOfPowers()
    {
        var words = await _wordBookService.LoadWordsAsync();
        _wordOfPowers.Value = words;

    }

    public void SetCurrentWord(WordOfPower wordOfPower)
    {
        _selectedWord.Value = wordOfPower;
    }

    public bool AddToSelected()
    {
        if (!_selectedWords.Contains(SelectedWord.Value) && _selectedWords.Count < _wordCap.Value)
        {
            _selectedWords.Add(SelectedWord.Value);
            return true;
        }
        return false;
    }

    public void RemoveFromSelected()
    {
        if (_selectedWords.Contains(SelectedWord.Value))
            _selectedWords.Remove(SelectedWord.Value);
    }

    public bool CanCreateAnotherView()
    {
        return true;
    }
}
