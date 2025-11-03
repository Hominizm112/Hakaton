using UnityEngine;
using GameCore.UI;
using UnityEngine.Localization.Components;
using System.Collections.Generic;
using Zenject;
using UniRx;
using Cysharp.Threading.Tasks;
using UnityEngine.AddressableAssets;
using Debug = ColorfulDebug;
using System;
using TMPro;
using System.Linq;
using DG.Tweening;


namespace TeaGame.Views
{
    public class WordBookView : View<WordBookViewModel>
    {
        [SerializeField] private Transform wordViewHolder;
        [SerializeField] private GameObject wordDescriptionView;
        [SerializeField] private LocalizeStringEvent wordSelectedText;
        [SerializeField] private LocalizeStringEvent wordDescriptionText;
        [SerializeField] private AssetReference wordViewPrefabRef;

        [SerializeField] private ButtonExtendedViewBinder addWordToSelectedButton = new("addWordToSelectedButton");
        [SerializeField] private ButtonExtendedViewBinder removeWordFromSelectedButton = new("removeWordFromSelectedButton");
        [SerializeField] private Transform selectedWordsHolder;
        [SerializeField] private TMP_Text selectedWordsCupText;
        [SerializeField] private int poolSize;

        private List<WordView> _wordViews = new();
        private CompositeDisposable _disposables = new();
        private bool _wordViewsInitialized;
        private List<WordView> _selectedWordViews;

        private Dictionary<WordView, Tween> _selectedWordViewAnimations = new();

        private ReactiveProperty<int> _wordCap = new();



        protected Action<WordOfPower> onWordSelected;
        protected Action onWordListRefreshed;




        public override void Initialize()
        {
            Bind(addWordToSelectedButton, removeWordFromSelectedButton);

            ViewModel.WordOfPowers
                .Subscribe(words => RefreshWordViews(words))
                .AddTo(_disposables);

            if (!_wordViewsInitialized)
            {
                RefreshWordViews(ViewModel.WordOfPowers.Value);
            }

            if (addWordToSelectedButton != null && removeWordFromSelectedButton != null)
            {
                ViewModel.SelectedWords
                    .ObserveAdd()
                    .Select(addEvent => addEvent.Value)
                    .Subscribe(lastAddedWord =>
                    {
                        HandleSelectedWordsCollectionAddedAsync(lastAddedWord).Forget();
                    })
                    .AddTo(_disposables);

                ViewModel.SelectedWords
                    .ObserveRemove()
                    .Select(addEvent => addEvent.Value)
                    .Subscribe(lastAddedWord =>
                    {
                        HandleSelectedWordsCollectionRemoved(lastAddedWord);
                    })
                    .AddTo(_disposables);
                CreateSelectedWordsViewPool().Forget();
            }

            ViewModel.WordCap
               .Subscribe(cap => _wordCap.Value = cap)
               .AddTo(_disposables);

            _wordCap
                .Subscribe(_ => UpdateWordCapText())
                .AddTo(_disposables);

            ViewModel.SelectedWords
                .ObserveCountChanged()
                .Subscribe(_ => UpdateWordCapText())
                .AddTo(_disposables);

            _wordCap.Value = ViewModel.WordCap.Value;

        }

        private void UpdateWordCapText()
        {
            selectedWordsCupText.text = ViewModel.SelectedWords.Count + "/" + _wordCap;
        }

        private void RefreshWordViews(List<WordOfPower> words)
        {
            _wordViewsInitialized = true;

            if (words == null || wordViewPrefabRef == null) return;
            _wordViews.RemoveAll(view => view == null);

            foreach (var wordOfPower in words)
            {
                if (wordOfPower == null) continue;

                bool viewExists = _wordViews.Exists(view =>
                    view != null && view.WordOfPower == wordOfPower);

                if (!viewExists)
                {
                    _ = CreateWordView(wordOfPower);
                }
            }

            var wordViewsContainer = _wordViews;
            foreach (var view in wordViewsContainer)
            {
                if (view != null && !words.Contains(view.WordOfPower))
                {
                    Addressables.ReleaseInstance(view.gameObject);
                    _wordViews.Remove(view);
                }
            }

            onWordListRefreshed?.Invoke();
        }

        private async UniTask<WordView> CreateWordView(WordOfPower wordOfPower = null)
        {
            var handle = Addressables.InstantiateAsync(wordViewPrefabRef);
            await handle.Task;

            if (handle.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
            {
                var view = handle.Result.GetComponent<WordView>();
                view.SetWord(wordOfPower, WordSelectionCallback);
                view.transform.SetParent(wordViewHolder);
                view.transform.localScale = Vector3.one;

                _disposables.Add(view);

                return view;
            }
            else
            {
                if (ProjectConstants.IS_DEVELOPER_MODE) Debug.LogError("Error while craeting WordView.");
                return null;
            }
        }


        private void WordSelectionCallback(WordOfPower wordOfPower)
        {
            onWordSelected?.Invoke(wordOfPower);
            ViewModel.SetCurrentWord(wordOfPower);
            HandleWordSelection(wordOfPower);
        }

        private void HandleWordSelection(WordOfPower wordOfPower)
        {
            wordDescriptionView.SetActive(true);
            wordSelectedText.StringReference = wordOfPower.word;
            wordDescriptionText.StringReference = wordOfPower.description;
        }

        private async UniTask CreateSelectedWordsViewPool()
        {
            _selectedWordViews = new(poolSize);

            for (int i = 0; i < poolSize; i++)
            {
                await CreateSelectedWordView();
            }
        }

        private async UniTask CreateSelectedWordView()
        {
            WordView view = await CreateWordView();
            view.transform.SetParent(selectedWordsHolder);

            if (_selectedWordViewAnimations.ContainsKey(view))
            {
                _selectedWordViewAnimations[view]?.Kill();
                _selectedWordViewAnimations.Remove(view);
            }

            view.gameObject.SetActive(false);
            _selectedWordViews.Add(view);
        }


        private async UniTask HandleSelectedWordsCollectionAddedAsync(WordOfPower wordOfPower)
        {
            var view = _selectedWordViews.Find(r => !r.gameObject.activeSelf);
            if (view)
            {
                AnimateSelectedWordShow(view);
                view.gameObject.SetActive(true);
                view.SetWord(wordOfPower, WordSelectionCallback);
            }
            else
            {
                if (ViewModel.CanCreateAnotherView())
                {
                    await CreateSelectedWordView();
                    HandleSelectedWordsCollectionAddedAsync(wordOfPower).Forget();
                }
            }
        }


        private void HandleSelectedWordsCollectionRemoved(WordOfPower wordOfPower)
        {
            var view = _selectedWordViews.Find(r => r.WordOfPower == wordOfPower);
            if (view)
            {
                view.ResetWord();
                view.gameObject.SetActive(false);
            }
        }

        private void AnimateSelectedWordShow(WordView wordView)
        {
            wordView.transform.localScale = new Vector3(1.2f, 0.9f, 1f);
            var tween = wordView.transform.DOScale(Vector3.one, 0.25f).SetEase(Ease.OutBack).OnComplete(() =>
                {
                    _selectedWordViewAnimations.Remove(wordView);
                });
            _selectedWordViewAnimations[wordView] = tween;

        }


        public void OnEnable()
        {
            OnOpen();
        }
        public void OnDisable()
        {
            OnClose();
        }

        public void OnOpen()
        {
            wordDescriptionView.SetActive(false);
        }

        public void OnClose()
        {
        }


        public override void Dispose()
        {
            base.Dispose();
            _disposables.Dispose();

            foreach (var view in _wordViews)
            {
                Addressables.ReleaseInstance(view.gameObject);
            }

        }

    }
}