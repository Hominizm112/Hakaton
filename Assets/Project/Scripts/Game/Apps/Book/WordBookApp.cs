using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WordBookApp : BaseApp
{
    [Header("References")]
    [SerializeField] private GameObject wordViewPrefab;
    [SerializeField] private Transform wordViewHolder;
    [SerializeField] private GameObject wordDescriptionView;
    [SerializeField] private TMP_Text wordSelectedText;
    [SerializeField] private TMP_Text wordDescriptionText;
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
        wordSelectedText.text = word.word;

        if (wordDescriptionText != null)
        {
            wordDescriptionText.text = word.description ?? "No description available.";
        }
    }

    private void ClearSelection()
    {
        _currentSelectedWord = null;
        wordSelectedText.text = string.Empty;

        if (wordDescriptionText != null)
        {
            wordDescriptionText.text = string.Empty;
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

