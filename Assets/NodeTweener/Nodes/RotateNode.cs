using DG.Tweening;
using UnityEditor;
using UnityEngine;


[NodeType("Rotate Node", NodeType.Rotate, 165f)]
public class RotateNode : TweenNode
{
    public Vector3 targetRotation;
    public float duration = 1f;
    public Ease easeType = Ease.Linear;

    public override DG.Tweening.Tweener Execute(GameObject target)
    {
        return target.transform.DORotate(targetRotation, duration).SetEase(easeType);
    }

    public override void DrawNode()
    {
        GUIStyle centeredTitle = new GUIStyle(EditorStyles.boldLabel);
        centeredTitle.alignment = TextAnchor.MiddleCenter;

        GUILayout.Label("Rotate Node", centeredTitle);

        targetRotation = EditorGUILayout.Vector3Field("Rotation", targetRotation);

        GUILayout.Space(5);
        GUILayout.Label("Duration");
        duration = EditorGUILayout.FloatField(duration);

        GUILayout.Space(5);
        easeType = (Ease)EditorGUILayout.EnumPopup("Ease", easeType);
    }

}
