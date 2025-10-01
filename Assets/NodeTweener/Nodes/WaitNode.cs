using DG.Tweening;
using UnityEditor;
using UnityEngine;



[CreateAssetMenu(fileName = "WaitNode", menuName = "DOTween/Nodes/Wait")]
[NodeType("Wait Node", NodeType.Wait, 80f)]
public class WaitNode : TweenNode
{
    public float waitTime = 1f;

    public override DG.Tweening.Tweener Execute(GameObject target)
    {
        var tweener = DOTween.To(() => 0f, x => { }, 1f, waitTime);
        return tweener;
    }

    public override void DrawNode()
    {
        GUILayout.Label("Wait Node", EditorStyles.boldLabel);
        GUILayout.Label("Wait Time");
        waitTime = EditorGUILayout.FloatField(waitTime);
    }
}
