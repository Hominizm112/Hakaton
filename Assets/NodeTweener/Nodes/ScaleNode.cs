using DG.Tweening;
using UnityEditor;
using UnityEngine;


[CreateAssetMenu(fileName = "ScaleNode", menuName = "DOTween/Nodes/Scale")]
[NodeType("Scale Node", NodeType.Scale, 170f)]
public class ScaleNode : TweenNode
{

    public Vector3 targetScale = Vector3.one;
    public float duration = 1f;
    public Ease easeType = Ease.Linear;

    public override DG.Tweening.Tweener Execute(GameObject target)
    {
        if (target == null)
        {
            Debug.LogError("ScaleNode: No target specified!");
            return null;
        }

        var tweener = target.transform.DOScale(targetScale, duration);
        tweener.SetEase(easeType);
        return tweener;
    }

    public override void DrawNode()
    {
        GUILayout.Label("Scale Node", EditorStyles.boldLabel);
        targetScale = EditorGUILayout.Vector3Field("Scale", targetScale);

        GUILayout.Space(5);
        GUILayout.Label("Duration");
        duration = EditorGUILayout.FloatField(duration);
    }
}
