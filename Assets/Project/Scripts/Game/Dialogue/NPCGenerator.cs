using System.Collections.Generic;
using UnityEngine;

public class NPCGenerator : MonoBehaviour
{
    private NPC activeNpc;

    public void Start()
    {
    }

    public void GenerateNpc()
    {
        if (activeNpc == null)
        {
            activeNpc = new NPC();
        }

    }
}

// public class NPC
// {
//     List<TeaFlavorTag> flavorPreferences;


//     public NPC()
//     {
//         int flavorPreferencesCount = Random.Range(1, 6);
//         for (int i = 0; i < flavorPreferencesCount; i++)
//         {
//             flavorPreferences.Add(RandomUtils.GetRandomItemInEnum<TeaFlavorTag>());
//         }
//     }
// }
