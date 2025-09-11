using System.Collections.Generic;
using UnityEngine;

public class NPCDialogueController : MonoBehaviour
{
    [Header("NPC Details")]
    public string npcName;
    public Sprite portrait;
    public Dialogue npcDialogue;

    [Header("Trade Settings")]
    public bool canBuy = true;
    public float priceModifier = 1.0f;

    [Header("Behavior")]
    public float smallTalkCooldown = 30f;

    private float lastTalkTime = -999f;
    private int conversationDepth = 0;


    void Start()
    {
        if (npcDialogue == null)
        {
            npcDialogue = new Dialogue();
            npcDialogue.npcName = npcName;
        }
    }


    public DialogueLine GetGreeting()
    {
        conversationDepth = 0;
        lastTalkTime = Time.time;

        if (npcDialogue.greetingLines.Count == 0)
            return CreateDefaultGreeting();

        return npcDialogue.greetingLines[Random.Range(0, npcDialogue.greetingLines.Count)];
    }

    public DialogueLine GetSmallTalk()
    {
        conversationDepth++;
        lastTalkTime = Time.time;

        if (npcDialogue.smallTalkLines.Count == 0)
            return CreateDefaultSmallTalk();

        return npcDialogue.smallTalkLines[Random.Range(0, npcDialogue.smallTalkLines.Count)];
    }

    public DialogueLine GetFarewell()
    {
        conversationDepth = 0;
        lastTalkTime = Time.time;

        if (npcDialogue.farewellLines.Count == 0)
            return CreateDefaultFarewell();

        return npcDialogue.farewellLines[Random.Range(0, npcDialogue.farewellLines.Count)];
    }

    public List<CommodityRequest> GetBuyRequests()
    {
        return npcDialogue.buyRequests;
    }


    public int GetBuyPrice(Commodity commodity, int quantity)
    {
        float basePrice = commodity.CurrentPrice * quantity;
        float relationshipModifier = npcDialogue.friendliness / 100f;
        float finalPrice = basePrice * priceModifier * relationshipModifier;

        return Mathf.RoundToInt(finalPrice);
    }

    public int GetSellPrice(Commodity commodity, int quantity)
    {
        float basePrice = commodity.CurrentPrice * quantity;
        float relationshipModifier = (npcDialogue.friendliness / 100f) * 1.2f; // NPCs pay less when buying
        float finalPrice = basePrice * priceModifier * relationshipModifier;

        return Mathf.RoundToInt(finalPrice);
    }

    public void ModifyFriendliness(int amount)
    {
        npcDialogue.friendliness = Mathf.Clamp(npcDialogue.friendliness + amount, 0, 100);
    }

    private DialogueLine CreateDefaultGreeting()
    {
        return new DialogueLine
        {
            text = $"Hello there! What can I do for you?",
            mood = DialogueMood.Neutral
        };
    }

    private DialogueLine CreateDefaultSmallTalk()
    {
        string[] defaultLines =
        {
            "The weather is quite nice today, don't you think?",
            "Business has been good lately.",
            "I've been hearing interesting things about the tea market.",
            "There's something special about a good cup of tea, isn't there?",
            "I wonder what new teas will come into season soon."
        };

        return new DialogueLine
        {
            text = defaultLines[Random.Range(0, defaultLines.Length)],
            mood = DialogueMood.Neutral
        };
    }

    private DialogueLine CreateDefaultFarewell()
    {
        return new DialogueLine
        {
            text = "Goodbye! Come back anytime.",
            mood = DialogueMood.Happy
        };
    }
}
