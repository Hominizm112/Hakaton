using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class Timer
{
    public event Action OnTimerStarted;
    public event Action OnTimerCompleted;
    public event Action OnTimerStopped;
    public event Action<float> OnTimerUpdate;

    private CancellationTokenSource _cts;
    private bool _isRunning;

    public bool IsRunning => _isRunning;
    public float CurrentTime { get; private set; }
    public float Duration { get; private set; }
    public float Progress => Duration > 0 ? CurrentTime / Duration : 0;

    public async void Start(float duration, bool loop = false)
    {
        if (_isRunning) Stop();

        _isRunning = true;
        _cts = new CancellationTokenSource();
        Duration = duration;
        CurrentTime = 0f;

        OnTimerStarted?.Invoke();

        try
        {
            do
            {
                CurrentTime = 0f;

                while (CurrentTime < duration)
                {
                    if (_cts.Token.IsCancellationRequested)
                        return;

                    CurrentTime += Time.deltaTime;
                    OnTimerUpdate?.Invoke(CurrentTime);

                    await Task.Yield();
                }

                OnTimerCompleted?.Invoke();

            } while (loop && !_cts.Token.IsCancellationRequested);
        }
        catch (OperationCanceledException)
        {
        }
        finally
        {
            _isRunning = false;
        }
    }

    public void Stop()
    {
        _cts?.Cancel();
        _isRunning = false;
        OnTimerStopped?.Invoke();
    }

    public void Pause()
    {
        _isRunning = false;
        _cts?.Cancel();
    }

    public void Resume()
    {
        if (CurrentTime > 0 && !_isRunning)
        {
            Start(Duration - CurrentTime);
        }
    }

    public void ResetTimerCompletedAction()
    {
        OnTimerCompleted = null;
    }

    public void Dispose()
    {
        OnTimerStarted = null;
        OnTimerCompleted = null;
        OnTimerStopped = null;
        OnTimerUpdate = null;
    }
}