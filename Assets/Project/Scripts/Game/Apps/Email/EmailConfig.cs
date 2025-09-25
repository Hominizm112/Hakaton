using UnityEngine;

[CreateAssetMenu(fileName = "EmailConfig", menuName = "MiniApps/EmailApp/EmailConfig")]
public class EmailConfig : ScriptableObject
{
    public NPC npc;
    public int friendLevelToUnlock;
    public string Sender => npc.npcName;
    public string EmailName;
    public string Contents;
}
