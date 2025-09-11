using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Dialogue
{
    public string npcName;
    public List<DialogueLine> greetingLines = new();
    public List<DialogueLine> smallTalkLines = new();
    public List<DialogueLine> farewellLines = new();
    public List<CommodityRequest> buyRequests = new();

    [Range(0, 100)]
    public int friendliness = 50;
}

[Serializable]
public class DialogueLine
{
    [TextArea(2, 4)]
    public string text;
    public DialogueMood mood = DialogueMood.Neutral;

}

[Serializable]
public class CommodityRequest
{
    public string commodityId;
    public TeaFlavorTag preferredFlavor;
    public TeaType preferredType;
    public int desiredQuantity = 1;
    public int maxPrice = 100;
    public int urgency = 50;

    [TextArea(1, 3)]
    public string requestLine;
}

public enum DialogueMood
{
    Happy,
    Neutral,
    Sad,
    Angry,
    Excited,
    Thoudhtful
}

public enum DialogueAction
{
    Continue,
    OpenShop,
    CompleteQuest,
    ChangeRelationship
}