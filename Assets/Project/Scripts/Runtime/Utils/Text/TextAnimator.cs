using System;
using System.Threading;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class TextAnimationHandle
{
    private Action _onComplete;
    private Action _onStop;
    private TMP_Text _tmpText;
    private string _fullText;

    public TextAnimationHandle(TMP_Text tmpText, string fullText)
    {
        _tmpText = tmpText;
        _fullText = fullText;
    }

    public TextAnimationHandle OnComplete(Action onComplete)
    {
        _onComplete = onComplete;
        return this;
    }

    public TextAnimationHandle OnStop(Action onStop)
    {
        _onStop = onStop;
        return this;
    }

    public void InvokeComplete()
    {
        _onComplete?.Invoke();
    }

    public void InvokeStop()
    {
        if (_tmpText != null)
        {
            _tmpText.text = _fullText;
        }
        _onStop?.Invoke();
    }
}

public class TextAnimator
{
    public bool IsRunning { get; private set; }
    private CancellationTokenSource _cancellationTokenSource;
    private TextAnimationHandle _currentHandle;

    public TextAnimationHandle AnimateShow(TMP_Text tmpText, string text, float duration, bool letterDuration = false, Action OnUpdateLetter = null)
    {
        StopAnimation();

        IsRunning = true;
        _cancellationTokenSource = new CancellationTokenSource();
        _currentHandle = new TextAnimationHandle(tmpText, text);

        _ = RunAnimationAsync(tmpText, text, duration, letterDuration, OnUpdateLetter, _currentHandle);

        return _currentHandle;
    }

    private async Task RunAnimationAsync(TMP_Text tmpText, string text, float duration, bool letterDuration, Action OnUpdateLetter, TextAnimationHandle handle)
    {
        try
        {
            await AnimateAsync(tmpText, text, letterDuration ? duration : CalculatePerLetterDuration(text, duration), _cancellationTokenSource.Token, OnUpdateLetter);

            if (!_cancellationTokenSource.IsCancellationRequested)
            {
                handle.InvokeComplete();
            }
        }
        catch (TaskCanceledException)
        {
        }
        finally
        {
            IsRunning = false;
            _cancellationTokenSource?.Dispose();
            _cancellationTokenSource = null;
            _currentHandle = null;
        }
    }

    public void StopAnimation()
    {
        if (_cancellationTokenSource != null && !_cancellationTokenSource.IsCancellationRequested)
        {
            _currentHandle?.InvokeStop();

            _cancellationTokenSource.Cancel();
        }

        Cleanup();
    }

    private void Cleanup()
    {
        _cancellationTokenSource?.Dispose();
        _cancellationTokenSource = null;
        IsRunning = false;
        _currentHandle = null;
    }

    private float CalculatePerLetterDuration(string text, float duration)
    {
        var cleanText = text.Replace(" ", "");
        return duration / Mathf.Max(1, cleanText.Length);
    }

    private async Task AnimateAsync(TMP_Text tmpText, string text, float letterDuration, CancellationToken cancellationToken, Action OnUpdateLetter)
    {
        tmpText.text = "";

        foreach (var ch in text)
        {
            if (cancellationToken.IsCancellationRequested)
                return;

            string currentText = tmpText.text;
            currentText += ch;
            tmpText.text = currentText;

            if (ch != ' ')
            {
                OnUpdateLetter?.Invoke();

                float elapsed = 0f;
                while (elapsed < letterDuration)
                {
                    if (cancellationToken.IsCancellationRequested)
                        return;

                    elapsed += Time.deltaTime;
                    await Task.Yield();
                }
            }
        }
    }
}