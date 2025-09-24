using UnityEngine;

[CreateAssetMenu(fileName = "PlantAnimationSettings", menuName = "Settings/Plant Animation Settings")]
public class PlantsAnimationSettings : ScriptableObject
{
    [Header("Prefab Settings")]
    public GameObject leafPrefab;

    [Header("Position Settings")]
    public Vector2 dropLeavesPoint = Vector2.zero;
    public float dropDistance = 4f;
    
    [Header("Timing Settings")]
    public float dropDuration = 5f;
    public float actionInterval = 10f;
}
