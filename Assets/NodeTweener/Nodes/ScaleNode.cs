using System;
using DG.Tweening;
using UnityEditor;
using UnityEngine;


[NodeType("Scale Node", 150f)]
[Serializable]
public class ScaleNode : TweenNode
{

    [SerializeField] public Vector3 targetScale = Vector3.one;
    [SerializeField] public float duration = 1f;
    [SerializeField] public Ease easeType = Ease.Linear;
    [SerializeField] public bool relative = false;

    public override DG.Tweening.Tweener Execute(GameObject target)
    {

        DG.Tweening.Tweener tweener;

        if (relative)
        {
            tweener = target.transform.DOScale(target.transform.localScale + targetScale, duration);
        }
        else
        {
            tweener = target.transform.DOScale(targetScale, duration);
        }

        tweener.SetEase(easeType);
        return tweener;
    }

    public override void DrawNode()
    {
        GUIStyle centeredTitle = new GUIStyle(EditorStyles.boldLabel);
        centeredTitle.alignment = TextAnchor.MiddleCenter;

        GUILayout.Label("Scale Node", centeredTitle);
        targetScale = EditorGUILayout.Vector3Field("Scale:", targetScale);

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
}
