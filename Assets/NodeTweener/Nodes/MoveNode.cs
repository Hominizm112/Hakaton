using DG.Tweening;
using UnityEditor;
using UnityEngine;


[NodeType("Move Node", NodeType.Move, 170f)]
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

    public override void DrawNode()
    {
        GUIStyle centeredTitleStyle = new GUIStyle(EditorStyles.boldLabel);
        centeredTitleStyle.alignment = TextAnchor.MiddleCenter;

        GUILayout.Label("Move Node", centeredTitleStyle);

        targetPosition = EditorGUILayout.Vector3Field("Position", targetPosition);

        GUILayout.Space(5);
        GUILayout.Label("Duration");
        duration = EditorGUILayout.FloatField(duration);

        GUILayout.Space(5);
        GUILayout.Label("Ease");
        easeType = (Ease)EditorGUILayout.EnumPopup(easeType);


    }
}
