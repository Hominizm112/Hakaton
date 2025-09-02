using UnityEngine;
using DG.Tweening;

[CreateAssetMenu(fileName = "AnimationSettings", menuName = "Animations/AnimationSettings")]
public class AnimationSettings : ScriptableObject
{
    public float duration;
    public Ease ease;
}
