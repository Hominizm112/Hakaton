using UnityEngine;
using DG.Tweening;

[CreateAssetMenu(fileName = "CloudAnimationSettings", menuName = "Settings/Cloud Animation Settings")]
public class CloudAnimationsSettings : ScriptableObject
{
    [Header("Movement Settings")]
    public Vector2 initial_point = new Vector2(0, 5f);
    public float xFinal = 5f;
     public float movementDuration = 10f;

}
