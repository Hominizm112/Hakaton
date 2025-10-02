using UnityEngine;

public class TweenGraphRedirect : MonoBehaviour
{
    [SerializeField] private TweenGraphRunner tweenGraphRunner;

    public void PlaySequence()
    {
        tweenGraphRunner.PlaySequence(gameObject);
    }

    public void StopSequence()
    {
        tweenGraphRunner.StopSequence();
    }
}
