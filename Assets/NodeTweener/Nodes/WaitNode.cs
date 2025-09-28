using DG.Tweening;
using UnityEngine;



[CreateAssetMenu(fileName = "WaitNode", menuName = "DOTween/Nodes/Wait")]
public class WaitNode : TweenNode
{
    public float waitTime = 1f;

    public override DG.Tweening.Tweener Execute(GameObject target)
    {
        var tweener = DOTween.To(() => 0f, x => { }, 1f, waitTime);
        return tweener;
    }
}
