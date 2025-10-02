using System;
using DG.Tweening;
using UnityEditor;
using UnityEngine;


[NodeType("Branch Node", 35f, 35f)]
[Serializable]
public class BranchNode : TweenNode
{

    public override DG.Tweening.Tweener Execute(GameObject target)
    {
        return DOTween.To(() => 0, x => { }, 0, 0);
    }

    public override void DrawNode()
    {



    }
}
