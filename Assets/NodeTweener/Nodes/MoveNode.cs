using System;
using DG.Tweening;
using UnityEditor;
using UnityEngine;


[NodeType("Move Node", 150f)]
[Serializable]
public class MoveNode : TweenNode
{
    [SerializeField] public Vector3 targetPosition;
    [SerializeField] public float duration = 1f;
    [SerializeField] public Ease easeType = Ease.Linear;
    [SerializeField] public bool relative = false;

    public override DG.Tweening.Tweener Execute(GameObject target)
    {

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

#if UNITY_EDITOR

    public override void DrawNode()
    {
        GUIStyle centeredTitleStyle = new GUIStyle(EditorStyles.boldLabel);
        centeredTitleStyle.alignment = TextAnchor.MiddleCenter;

        GUILayout.Label("Move Node", centeredTitleStyle);

        targetPosition = EditorGUILayout.Vector3Field("Position:", targetPosition);

        GUILayout.Space(5);

        GUILayout.BeginHorizontal();
        GUILayout.Label("Duration:");
        duration = EditorGUILayout.FloatField(duration);
        GUILayout.EndHorizontal();

        GUILayout.Space(5);

        GUILayout.BeginHorizontal();
        GUILayout.Label("Ease:");
        easeType = (Ease)EditorGUILayout.EnumPopup(easeType);
        GUILayout.EndHorizontal();

        GUILayout.Space(5);

        GUILayout.BeginHorizontal();
        GUILayout.Label("Relative:");
        relative = EditorGUILayout.Toggle(relative);
        GUILayout.EndHorizontal();

    }

#endif

}
