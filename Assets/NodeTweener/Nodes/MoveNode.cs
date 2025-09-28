using DG.Tweening;
using UnityEngine;


[CreateAssetMenu(fileName = "MoveNode", menuName = "DOTween/Nodes/Move")]
public class MoveNode : TweenNode
{
    public Vector3 targetPosition;
    public float duration = 1f;
    public Ease easeType = Ease.Linear;
    public bool relative = false;

    public override DG.Tweening.Tweener Execute(GameObject target)
    {
        if (target == null)
        {
            Debug.LogError("MoveNode: No target specified!");
            return null;
        }

        DG.Tweening.Tweener tweener;

        if (relative)
        {
            tweener = target.transform.DOMove(target.transform.position + targetPosition, duration);
        }
        else
        {
            tweener = target.transform.DOMove(targetPosition, duration);
        }

        tweener.SetEase(easeType);
        return tweener;
    }
}
