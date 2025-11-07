
using Cysharp.Threading.Tasks;
using GameCore.UI;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace TeaGame.UI.View
{
    public class ConsoleView : View<ConsoleViewModel>
    {

        [Header("References")]
        [SerializeField] private GameObject consoleObject;
        [SerializeField] private TMP_InputField inputField;
        [SerializeField] private TMP_Text outputText;
        [SerializeField] private ScrollRect scrollRect;
        [SerializeField] private int maxOutputLines;

        public override void Initialize()
        {
            Bind();

            ViewModel.switchConsoleVisible
                .Subscribe(SwitchConsoleVisible)
                .AddTo(disposables);

            ViewModel.navigateHistory
                .Subscribe(NavigateHistory)
                .AddTo(disposables);

            ViewModel.updateOutputDisplay
                .Subscribe(UpdateOutputDisplay)
                .AddTo(disposables);

            ViewModel.autoComplete
                .Subscribe(UpdateInputDisplay)
                .AddTo(disposables);

            inputField.onValueChanged.AddListener(HandleInputChange);
            inputField.onSubmit.AddListener(HandleInput);

            ViewModel.Initialize(maxOutputLines);
            SwitchConsoleVisible(false);
        }

        private void SwitchConsoleVisible(bool visible)
        {
            consoleObject.SetActive(visible);

            if (visible)
            {
                SelectInputField();
            }
        }

        private void NavigateHistory(string str)
        {
            UpdateInputDisplay(str);
            SelectInputField();
        }

        private void SelectInputField()
        {
            inputField.Select();
            inputField.ActivateInputField();
        }

        private void HandleInputChange(string input)
        {
            if (!inputField.isFocused) return;
            ViewModel.SetCurrentInput(input);
        }

        private void HandleInput(string input)
        {
            ViewModel.HandleInput(input);

            inputField.text = "";
            SelectInputField();
        }

        private void UpdateInputDisplay(string message)
        {
            inputField.text = message;
            inputField.caretPosition = inputField.text.Length;
            SelectInputField();
        }

        private void UpdateOutputDisplay(string message)
        {
            outputText.text = message;

            Canvas.ForceUpdateCanvases();
            scrollRect.verticalNormalizedPosition = 0f;
            Canvas.ForceUpdateCanvases();
        }

    }
}