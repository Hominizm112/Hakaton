using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using TeaGame.Services;
using UniRx;
using UnityEngine.AddressableAssets;
using Zenject;

public class WordBookService : IDisposable
{
    [Inject] private TeaMixerService teaMixerService;

    public ReactiveProperty<ItemData> SelectedTeaForConstruct = new();


    private CompositeDisposable _disposables = new();


    [Inject]
    public void Construct()
    {
        teaMixerService.TeaToCook
            .Subscribe(item =>
            {
                SelectedTeaForConstruct.Value = item;
            })
            .AddTo(_disposables);
    }

    private const string WORDS_ADDRESSABLE_LABEL = "WordOfPower";

    public async UniTask<List<WordOfPower>> LoadWordsAsync()
    {
        var handle = Addressables.LoadAssetsAsync<WordOfPower>(WORDS_ADDRESSABLE_LABEL, null);
        var words = await handle.Task;

        return words.ToList();
    }

    public void Dispose()
    {
        _disposables.Dispose();
    }
}
