using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using GameCore.UI;
using UniRx;
using UnityEngine;
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


    [Inject] WordBookService _wordBookService;

    private CompositeDisposable _disposables;


    public override void Initialize()
    {
        Bind(_addWordToSelectedButton, _removeWordFromSelectedButton);

        // _addWordToSelectedButton.Value.Subscribe(AddToSelected)
        //     .AddTo(_disposables);


        // _removeWordFromSelectedButton.Value.Subscribe(r =>
        //     {
        //         if (r == MouseButtonClick.Up)
        //             RemoveFromSelected();
        //     })
        //     .AddTo(_disposables);

        RefreshWordOfPowers().Forget();
    }

    public async UniTask RefreshWordOfPowers()
    {
        Debug.Log("refreshing words");
        var words = await _wordBookService.LoadWordsAsync();
        _wordOfPowers.Value = words;

    }

    public void SetCurrentWord(WordOfPower wordOfPower)
    {
        _selectedWord.Value = wordOfPower;
    }

    public void AddToSelected(MouseButtonClick mouseButtonClick)
    {
        if (mouseButtonClick == MouseButtonClick.Down) return;
        _selectedWords.Add(SelectedWord.Value);
    }

    public void RemoveFromSelected()
    {
        _selectedWords.Remove(SelectedWord.Value);
    }

    public bool CanCreateAnotherView()
    {
        return true;
    }
}
