
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class Email
{
    public static List<EmailConfig> LoadEmailConfigs()
    {
        return Resources.LoadAll<EmailConfig>("Configs/Apps/Emails").ToList();
    }
}
