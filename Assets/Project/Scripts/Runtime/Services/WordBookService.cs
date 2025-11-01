using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine.AddressableAssets;

public class WordBookService
{
    private const string WORDS_ADDRESSABLE_LABEL = "WordOfPower";

    public async UniTask<List<WordOfPower>> LoadWordsAsync()
    {
        var handle = Addressables.LoadAssetsAsync<WordOfPower>(WORDS_ADDRESSABLE_LABEL, null);
        var words = await handle.Task;

        return words.ToList();
    }
}
