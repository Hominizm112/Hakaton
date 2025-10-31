using UniRx;
using UnityEngine;


[CreateAssetMenu(fileName = "New Item Template", menuName = "Inventory/Item Template")]
public class ItemTemplate : ScriptableObject
{
    public string ItemId;
    public string Name;
    public ItemRarity Rarity;
    public int BaseSellPrice;
    public Sprite Icon;
    public string Description;


}
