using DG.Tweening;
using UnityEditor;
using UnityEngine;


[NodeType("Start Node", 60f, 150f)]
public class StartNode : TweenNode
{
    public override DG.Tweening.Tweener Execute(GameObject target)
    {
        return DOTween.To(() => 0, x => { }, 0, 0);
    }

#if UNITY_EDITOR

    public override void DrawNode()
    {
        GUIStyle centeredStyle = new GUIStyle(EditorStyles.boldLabel);
        centeredStyle.alignment = TextAnchor.MiddleCenter;

        GUIStyle miniCenteredStyle = new GUIStyle(EditorStyles.miniLabel);
        miniCenteredStyle.alignment = TextAnchor.MiddleCenter;

        GUILayout.Label("START", centeredStyle);
        GUILayout.Label("Sequence Entry Point", miniCenteredStyle);
    }

#endif
}
