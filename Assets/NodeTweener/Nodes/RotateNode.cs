using System;
using DG.Tweening;
using UnityEditor;
using UnityEngine;


[NodeType("Rotate Node", 150f)]
[Serializable]
public class RotateNode : TweenNode
{
    [SerializeField] public Vector3 targetRotation;
    [SerializeField] public float duration = 1f;
    [SerializeField] public Ease easeType = Ease.Linear;
    [SerializeField] public bool relative = false;

    public override DG.Tweening.Tweener Execute(GameObject target)
    {

        DG.Tweening.Tweener tweener;

        if (relative)
        {
            tweener = target.transform.DORotate(target.transform.rotation.eulerAngles + targetRotation, duration);
        }
        else
        {
            tweener = target.transform.DORotate(targetRotation, duration);
        }

        tweener.SetEase(easeType);
        return tweener;
    }

#if UNITY_EDITOR
    public override void DrawNode()
    {
        GUIStyle centeredTitle = new GUIStyle(EditorStyles.boldLabel);
        centeredTitle.alignment = TextAnchor.MiddleCenter;

        GUILayout.Label("Rotate Node", centeredTitle);
        targetRotation = EditorGUILayout.Vector3Field("Rotation:", targetRotation);

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
