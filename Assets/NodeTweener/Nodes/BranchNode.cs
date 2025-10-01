using DG.Tweening;
using UnityEditor;
using UnityEngine;


[NodeType("Branch Node", NodeType.Branch, 100f)]
public class BranchNode : TweenNode
{
    public string branchName = "Branch";

    public override DG.Tweening.Tweener Execute(GameObject target)
    {
        return DOTween.To(() => 0, x => { }, 0, 0);
    }

    public override float nodeHeight => 80f;

    public override void DrawNode()
    {
        GUIStyle centeredTitle = new GUIStyle(EditorStyles.boldLabel);
        centeredTitle.alignment = TextAnchor.MiddleCenter;

        GUILayout.Label("BRANCH", centeredTitle);

        GUILayout.Space(5);
        branchName = EditorGUILayout.TextField("Name", branchName);
    }
}
