using System.Collections.Generic;
using UnityEngine;

public class EmailApp : BaseApp
{
    [SerializeReference] private Transform emailHolder;

    public List<EmailConfig> emailConfigs;

    public void UnlockEmail(int emailId)
    {
        EmailConfig email = emailConfigs.Find(r => r.id == emailId);
        print(email.Sender);
        print(email.Contents);
    }
}
