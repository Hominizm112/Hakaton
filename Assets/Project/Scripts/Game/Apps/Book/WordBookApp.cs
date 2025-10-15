using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Components;

public class WordBookApp : BaseApp
{
    [Header("References")]
    [SerializeField] private GameObject wordViewPrefab;
    [SerializeField] private Transform wordViewHolder;
    [SerializeField] private GameObject wordDescriptionView;
    [SerializeField] private LocalizeStringEvent wordSelectedText;
    [SerializeField] private LocalizeStringEvent wordDescriptionText;
    public bool unlock;

    private List<WordOfPower> _wordOfPowers;
    private List<WordView> _wordViews = new();
    private WordOfPower _currentSelectedWord;


    protected override void HandleAppOpen()
    {
        if (unlock)
        {
            WordBook.UnlockWord("new_word");
        }
        LoadWordData();
        RefreshWordViews();
    }

    protected override void HandleAppClose()
    {
        ClearSelection();
    }

    private void LoadWordData()
    {
        if (_wordOfPowers == null || _wordOfPowers.Count == 0)
        {
            _wordOfPowers = WordBook.LoadWords();

            for (int i = _wordOfPowers.Count - 1; i >= 0; i--)
            {
                if (!_wordOfPowers[i].isUnlocked)
                {
                    _wordOfPowers.RemoveAt(i);
                }
            }

            if (_wordOfPowers.Count == 0)
            {
                ColorfulDebug.LogError("No words loaded from resources");
            }
        }
    }

    private void RefreshWordViews()
    {
        if (_wordOfPowers == null) return;

        WordBook.UpdateWordViews(
                    _wordOfPowers,
                    wordViewPrefab,
                    wordViewHolder,
                    HandleWordSelection,
                    _wordViews
                );
    }

    private void HandleWordSelection(WordOfPower wordOfPower)
    {
        if (wordOfPower == null)
        {
            print("wordOfPower is null");
            return;
        }

        _currentSelectedWord = wordOfPower;
        UpdateWordDescription(_currentSelectedWord);
    }

    private void UpdateWordDescription(WordOfPower word)
    {
        wordDescriptionView.SetActive(true);
        wordSelectedText.StringReference = word.word;
        wordDescriptionText.StringReference = word.description;


    }

    private void ClearSelection()
    {
        _currentSelectedWord = null;
        wordSelectedText.StringReference = null;

        if (wordDescriptionText != null)
        {
            wordDescriptionText.StringReference = null;
        }

        wordDescriptionView.SetActive(false);
    }

    public WordOfPower GetCurrentSelectedWord() => _currentSelectedWord;

    private void OnDestroy()
    {
        foreach (var wordView in _wordViews)
        {
            wordView.Cleanup();
        }

        _wordViews.Clear();
    }




}

