using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

public class WordBook : MonoService
{

    [Header("Scene References")]
    [SerializeField] private GameObject window;
    [SerializeField] private Transform wordViewHolder;
    [SerializeField] private Transform selectedWordViewHolder;
    [SerializeField] private GameObject wordDescriptionView;
    [SerializeField] private LocalizeStringEvent wordSelectedText;
    [SerializeField] private LocalizeStringEvent wordDescriptionText;
    [SerializeField] private TMP_Text selectedWordCountText;
    [SerializeField] private Button addButton;
    [SerializeField] private Button removeButton;


    [Header("Static References")]
    [SerializeField] private GameObject wordViewPrefab;

    [Header("Settings")]
    [SerializeField] private int maxWordCount = 3;

    private bool isOpen => window.activeSelf;
    private List<WordOfPower> _wordOfPowers;
    private List<WordView> _wordViews = new();
    private List<WordView> _selectedWordViews = new();
    private WordOfPower _currentSelectedWord;

    private List<WordOfPower> _selectedWords = new();

    public WordOfPower GetCurrentSelectedWord() => _currentSelectedWord;


    public void Start()
    {
        Mediator.Instance.RegisterService(this);
    }

    public override void Initialize(Mediator mediator)
    {
        base.Initialize(mediator);

        AddEvent(_mediator.GlobalEventBus.Subscribe<TeaRemovedFromSelectionEvent>(_ => ResetSelectedWords()));
    }
    public void SwitchWindow()
    {
        window.SetActive(!window.activeSelf);
        if (isOpen)
        {
            HandleWindowOpen();
        }
        else
        {
            HandleWindowClose();
        }
    }

    private void HandleWindowOpen()
    {
        _wordOfPowers = LoadUnlockedWords();
        RefreshWordViews();
        RefreshSelectedWordView();
    }

    private void HandleWindowClose()
    {
        ClearSelection();
    }

    private void RefreshWordViews()
    {
        if (_wordOfPowers == null) return;

        UpdateWordViews(
            _wordOfPowers,
            wordViewPrefab,
            wordViewHolder,
            HandleWordSelection,
            _wordViews
        );
    }

    private void HandleWordSelection(WordOfPower wordOfPower)
    {
        print(wordOfPower == null);
        if (wordOfPower == null)
        {
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
        wordDescriptionText.StringReference = null;
        wordDescriptionView.SetActive(false);
    }


    public void AddWordToSelected()
    {
        if (_selectedWords.Count < maxWordCount && _currentSelectedWord != null)
        {
            if (!_selectedWords.Contains(_currentSelectedWord))
            {
                _selectedWords.Add(_currentSelectedWord);
            }
        }

        RefreshSelectedWordView();
    }

    public void RemoveWordFromSelected()
    {
        if (_currentSelectedWord != null && _selectedWords.Contains(_currentSelectedWord))
        {
            _selectedWords.Remove(_currentSelectedWord);
        }

        RefreshSelectedWordView();
    }

    private void RefreshSelectedWordView()
    {
        UpdateWordViews(
            _selectedWords,
            wordViewPrefab,
            selectedWordViewHolder,
            HandleWordSelection,
            _selectedWordViews
        );

        SetWordsCount();
    }

    private void SetWordsCount()
    {
        var stall = _mediator.GetService<StallService>();
        if (stall.SelectedCommodity != null)
        {
            selectedWordCountText.gameObject.SetActive(true);
            addButton.interactable = true;
            removeButton.interactable = true;

            selectedWordCountText.text = $"{_selectedWords.Count}/{stall.SelectedCommodity.maxWordOfPower}";

        }
        else
        {
            selectedWordCountText.gameObject.SetActive(false);
            addButton.interactable = false;
            removeButton.interactable = false;
        }
    }

    private void ResetSelectedWords()
    {
        _selectedWords.Clear();
        RefreshSelectedWordView();
    }

    public override void Dispose()
    {
        foreach (var item in _wordViews)
        {
            item.Cleanup();
        }

        foreach (var item in _selectedWordViews)
        {
            item.Cleanup();
        }

        _mediator.UnregisterService(this);
    }


    #region  Static

    private const string WORDS_RESOURCE_PATH = "Configs/Words";
    public static List<WordOfPower> LoadWords()
    {
        return Resources.LoadAll<WordOfPower>(WORDS_RESOURCE_PATH).ToList();
    }

    public static List<WordOfPower> LoadUnlockedWords()
    {
        var words = LoadWords();

        for (int i = words.Count - 1; i >= 0; i--)
        {
            if (!words[i].isUnlocked)
            {
                words.RemoveAt(i);
            }
        }

        if (words.Count == 0)
        {
            ColorfulDebug.LogError("No words loaded from resources");
        }

        return words;
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

    #endregion

}
