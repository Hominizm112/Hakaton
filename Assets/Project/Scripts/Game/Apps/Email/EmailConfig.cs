using UnityEngine;

[CreateAssetMenu(fileName = "EmailConfig", menuName = "MiniApps/EmailApp/EmailConfig")]
public class EmailConfig : ScriptableObject
{
    public int id;
    public string Sender;
    public string Contents;
}
