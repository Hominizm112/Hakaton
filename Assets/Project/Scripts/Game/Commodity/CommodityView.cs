using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;

public class CommodityView : MonoBehaviour
{
    public LocalizeStringEvent nameText;
    public LocalizeStringEvent descriptionText;
    public TMP_Text quantityText;


    public void SetCommodity(CommodityEntry commodityEntry)
    {
        nameText.StringReference = commodityEntry.commodity.commodityName;
        descriptionText.StringReference = commodityEntry.commodity.description;
        quantityText.text = commodityEntry.amount.ToString();
    }
}
