using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;

[CreateAssetMenu(fileName = "TeaBase", menuName = "Tea/TeaBase")]
public class TeaBase : Commodity
{
    [SerializeField] public bool unlocked;
    public List<TeaFlavorTag> baseFlavorTags = new();
    public int maxWordOfPower;

}


public enum TeaFlavorTag
{
    // Базовые
    Floral,
    Fruity,
    Earthy,
    Grassy,
    Sweet,
    Bitter,
    Smoky,
    Spicy,
    Herbal,
    Woody,

    // Специфичные

    Astringent,
    Malty,
    Creamy,
    Nutty,
    Citrus,
    Berry,
    StoneFruit,
    Tropical,
    Mineral,
    Honey,

    // Дополнительные
    Umami,
    Buttery,
    Vanilla,
    Caramel,
    Chocolate,
    Malt,
    Toast,
    Nutmeg,
    Cinnamon,
    Ginger,

    // Редкие и уникальные
    Orchid,
    Jasmine,
    Rose,
    Lavender,
    Mint,
    Lemon,
    Orange,
    Peach,
    Apricot,
    Melon,
    Mushroom,
    ForestFloor,
    Leather,
    Tobacco,
    Wine,
}