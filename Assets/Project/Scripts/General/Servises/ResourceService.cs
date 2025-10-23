using System.Linq;
using UnityEngine;

public class ResourceService : MonoBehaviour
{
    private const string COMMODITY_PATH = "Configs/Commodities";
    public static Commodity GetCommodity(string commodityID)
    {
        var commodities = Resources.LoadAll<Commodity>(COMMODITY_PATH).ToList();
        return commodities.FirstOrDefault((r) => r.id == commodityID);
    }
}
