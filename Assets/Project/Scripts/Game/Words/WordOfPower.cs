using System.ComponentModel;
using UnityEngine;

[CreateAssetMenu(fileName = "New WordOfPower", menuName = "Words/WordOfPower")]
public class WordOfPower : ScriptableObject
{
    [SerializeField, ReadOnly(true)] private string _id;
    public string word;
    public string description;
    public string category;

    public string id
    {
        get
        {
            _id = GenerateID();
            return _id;
        }
    }

    private string GenerateID()
    {
        string baseName = string.IsNullOrEmpty(word) ? "word" : word.ToLower().Replace(" ", "_");
        return $"{baseName}";
    }

    private void Reset()
    {
        _id = GenerateID();
    }

    public string[] tags;
    [SerializeField] public bool isUnlocked = true;

    public override bool Equals(object obj)
    {
        if (obj is WordOfPower other)
        {
            return id == other.id;
        }
        return false;
    }

    public override int GetHashCode()
    {
        return id?.GetHashCode() ?? 0;
    }
}
