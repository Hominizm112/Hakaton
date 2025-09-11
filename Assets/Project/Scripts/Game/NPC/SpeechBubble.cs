using UnityEngine;
using TMPro;
using System;

public class SpeechBubble : MonoBehaviour, IInteractionObject
{
    [SerializeField] private TMP_Text text;
    [SerializeField, Tooltip("Duration per letter")] private float showDuration = 0.1f;
    public Action OnNextLineRequested;
    private bool isPrintingLine;
    private TextAnimator textAnimator = new();
    private string currentLine;

    public void SetText(string str, bool instant = false)
    {
        isPrintingLine = true;
        // OnNextLineRequested = null;


        if (instant)
        {
            text.text = str;
            isPrintingLine = false;
            return;
        }

        currentLine = str;
        textAnimator.AnimateShow(text, currentLine, showDuration, true)
            .OnComplete(() => isPrintingLine = false);
    }

    public void AddToText(string addition)
    {
        string currentText = text.text;
        currentText += addition;
        text.text = currentText;
    }

    private void EndLine()
    {
        textAnimator.StopAnimation();
        isPrintingLine = false;
    }

    public void Interact()
    {
        if (isPrintingLine)
        {
            EndLine();
        }
        else
        {
            OnNextLineRequested?.Invoke();
        }
    }

    private void OnDestroy()
    {
        OnNextLineRequested = null;
    }
}
