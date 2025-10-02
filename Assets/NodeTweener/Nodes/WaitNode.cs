using DG.Tweening;
using UnityEditor;
using UnityEngine;



[NodeType("Wait Node", 65f)]
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
        GUIStyle centeredTitle = new GUIStyle(EditorStyles.boldLabel);
        centeredTitle.alignment = TextAnchor.MiddleCenter;

        GUILayout.Label("Wait Node", centeredTitle);

        GUILayout.BeginHorizontal();
        GUILayout.Label("Wait Time:");
        waitTime = EditorGUILayout.FloatField(waitTime);
        GUILayout.EndHorizontal();

    }
}
