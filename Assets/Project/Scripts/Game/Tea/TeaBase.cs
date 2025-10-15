using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TeaBase", menuName = "Scriptable Objects/TeaBase")]
public class TeaBase : ScriptableObject
{
    public string teaBaseName;
    [SerializeField] public bool unlocked;
    public List<TeaFlavorTag> baseFlavorTags = new();
}
