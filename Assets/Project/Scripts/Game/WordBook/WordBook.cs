using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WordBook : MonoBehaviour
{

    private const string WORDS_RESOURCE_PATH = "Configs/Words";
    public static List<WordOfPower> LoadWords()
    {
        return Resources.LoadAll<WordOfPower>(WORDS_RESOURCE_PATH).ToList();
    }

    public static void UpdateWordViews(
       List<WordOfPower> wordOfPowers,
       GameObject wordViewPrefab,
       Transform wordViewHolder,
       Action<WordOfPower> wordSelectCallback,
       List<WordView> existingWordViews)
    {
        if (wordViewPrefab == null)
        {
            Debug.LogError("WordView prefab is null!");
            return;
        }

        if (wordViewHolder == null)
        {
            Debug.LogError("Word view holder is null!");
            return;
        }

        existingWordViews.RemoveAll(view => view == null);

        foreach (var wordOfPower in wordOfPowers)
        {
            if (wordOfPower == null) continue;

            bool viewExists = existingWordViews.Exists(view =>
                view != null && view.WordOfPower == wordOfPower);

            if (!viewExists)
            {
                CreateWordView(wordOfPower, wordViewPrefab, wordViewHolder, wordSelectCallback, existingWordViews);
            }
        }

        for (int i = existingWordViews.Count - 1; i >= 0; i--)
        {
            var view = existingWordViews[i];
            if (view != null && !wordOfPowers.Contains(view.WordOfPower))
            {
                if (Application.isPlaying)
                {
                    UnityEngine.Object.Destroy(view.gameObject);
                }
                else
                {
                    UnityEngine.Object.DestroyImmediate(view.gameObject);
                }
                existingWordViews.RemoveAt(i);
            }
        }
    }

    private static void CreateWordView(
        WordOfPower wordOfPower,
        GameObject wordViewPrefab,
        Transform wordViewHolder,
        Action<WordOfPower> wordSelectCallback,
        List<WordView> wordViews)
    {
        var wordViewInstance = UnityEngine.Object.Instantiate(wordViewPrefab, wordViewHolder);
        var wordView = wordViewInstance.GetComponent<WordView>();

        if (wordView == null)
        {
            Debug.LogError("WordView prefab doesn't have WordView component!");
            UnityEngine.Object.Destroy(wordViewInstance);
            return;
        }

        wordView.SetWord(wordOfPower, wordSelectCallback);
        wordViews.Add(wordView);
    }

    public static void UnlockWord(string wordId)
    {
        List<WordOfPower> wordsOfPower = LoadWords();

        foreach (var item in wordsOfPower)
        {
            if (item.id == wordId)
            {
                item.isUnlocked = true;
            }
        }
    }

}
