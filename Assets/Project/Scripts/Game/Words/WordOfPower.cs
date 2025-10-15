using System;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Localization;

[CreateAssetMenu(fileName = "New WordOfPower", menuName = "Words/WordOfPower")]
public class WordOfPower : ScriptableObject
{
    [SerializeField, ReadOnly(true)] private string _id;
    public LocalizedString word;
    public LocalizedString description;
    public string category;
    public List<WordToFlavorInfluence> wordToFlavorInfluences;

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
        string baseName = string.IsNullOrEmpty(word.ToString()) ? "word" : word.ToString().ToLower().Replace(" ", "_");
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

[Serializable]
public struct WordToFlavorInfluence
{
    public WordInfuence wordInfuence;
    public TeaFlavorTag teaFlavorTag;
}

public enum WordInfuence
{
    None,
    Add,
    Remove
}