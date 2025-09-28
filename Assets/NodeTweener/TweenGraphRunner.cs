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

    private Sequence currentSequence;
    private bool isPlaying = false;

    private void Start()
    {
        if (playOnStart)
        {
            PlaySequence();
        }
    }

    public void PlaySequence()
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

        if (isPlaying)
        {
            StopSequence();
        }

        currentSequence = tweenGraph.BuildSequence(targetObject);

        if (loop)
        {
            currentSequence.SetLoops(loopCount, loopType);
        }

        currentSequence.OnStart(() =>
        {
            isPlaying = true;
        });

        currentSequence.OnComplete(() =>
        {
            isPlaying = false;
        });

        currentSequence.Play();
    }

    public void StopSequence()
    {
        if (currentSequence != null && currentSequence.IsPlaying())
        {
            currentSequence.Kill();
            isPlaying = false;
        }
    }

    public void PauseSequence()
    {
        if (currentSequence != null && currentSequence.IsPlaying())
        {
            currentSequence.Pause();
        }
    }

    public void ResumeSequence()
    {
        if (currentSequence != null && !currentSequence.IsPlaying())
        {
            currentSequence.Play();
        }
    }

    public bool IsPlaying()
    {
        return isPlaying;
    }

    private void OnDestroy()
    {
        StopSequence();
    }
}
