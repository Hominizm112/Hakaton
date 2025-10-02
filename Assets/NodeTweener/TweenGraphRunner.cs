using UnityEngine;
using DG.Tweening;

public class TweenGraphRunner : MonoBehaviour
{
    [Header("Tween Graph")]
    public TweenGraph tweenGraph;

    [Header("Execution Settings")]
    public bool playOnStart = false;
    public bool loop = false;
    public int loopCount = -1;
    public LoopType loopType = LoopType.Restart;

    [Header("Target Object")]
    public GameObject targetObject;

    [Header("Runner Settings")]
    public string runnerName;
    public bool registerAsService;
    public BaseGraphRunnerService graphRunnerService;

    private Sequence _currentSequence;
    private bool _isPlaying = false;

    private void Awake()
    {
        if (registerAsService && graphRunnerService != null)
        {
            graphRunnerService.RegisterAnimator(runnerName, this);
        }

        if (playOnStart)
        {
            PlaySequence();
        }
    }

    private void OnDestroy()
    {
        if (registerAsService && graphRunnerService != null)
        {
            graphRunnerService.UnregisterAnimator(runnerName);
        }

        StopSequence();

    }

    public void PlaySequence(GameObject obj = null)
    {
        if (tweenGraph == null)
        {
            ColorfulDebug.LogError($"No Tween Graph assigned {this}");
            return;
        }


        if (targetObject == null)
        {
            targetObject = gameObject;
        }

        GameObject target = targetObject;


        if (obj != null)
        {
            target = obj;
            if (target == null)
            {
                return;
            }
        }

        if (_isPlaying)
        {
            StopSequence();
        }

        _currentSequence = tweenGraph.BuildSequence(target);

        if (loop)
        {
            _currentSequence.SetLoops(loopCount, loopType);
        }

        _currentSequence.OnStart(() =>
        {
            _isPlaying = true;
        });

        _currentSequence.OnComplete(() =>
        {
            _isPlaying = false;
        });

        _currentSequence.Play();
    }

    public void StopSequence()
    {
        if (_currentSequence != null)
        {
            _currentSequence.Kill();
            _currentSequence = null;
            _isPlaying = false;
        }
    }

    public void PauseSequence()
    {
        if (_currentSequence != null && _currentSequence.IsPlaying())
        {
            _currentSequence.Pause();
        }
    }

    public void ResumeSequence()
    {
        if (_currentSequence != null && !_currentSequence.IsPlaying())
        {
            _currentSequence.Play();
        }
    }

    public bool IsPlaying()
    {
        return _isPlaying;
    }


}
