using UnityEngine;
using DG.Tweening;

public class LeafController : MonoBehaviour
{
    public float DropDistance;
    public float DropDuration;
    public void StartLeafAnimation()
    {
        if (DropDistance <= 0) return;

        transform.DOMoveY(transform.position.y - DropDistance, DropDuration)
            .SetEase(Ease.InCubic)
            .OnComplete(() => Destroy(gameObject));
    }
}
