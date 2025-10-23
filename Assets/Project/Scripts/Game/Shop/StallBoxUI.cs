using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

public class StallBoxUI : MonoBehaviour
{
    [Header("Scene References")]
    [SerializeField] private Transform commodityViewsHolder;
    public LocalizeStringEvent nameLocalizeEvent;
    public LocalizeStringEvent descriptionLocalizeEvent;
    public TMP_Text quantityText;
    [SerializeField] private GameObject descriptionItems;

    [Header("Commodity View Settings")]
    [SerializeField] private GameObject commodityViewPrefab;


    public Commodity selectedCommodity;


    private Dictionary<string, CommodityView> _commodityViews = new();

    private ShopkeeperService _shopkeeperService;


    private void OnEnable()
    {
        RefreshCommodityViews();
    }

    private void OnDisable()
    {
        descriptionItems.SetActive(false);
        selectedCommodity = null;

    }

    public void UnsetCommodity()
    {
        descriptionItems.SetActive(false);
        selectedCommodity = null;
    }

    public void SetCommodity(Commodity commodity)
    {
        descriptionItems.SetActive(true);

        selectedCommodity = commodity;

        nameLocalizeEvent.StringReference = commodity.commodityName;
        descriptionLocalizeEvent.StringReference = commodity.description;

        quantityText.text = _shopkeeperService.GetAvailableCommodities().FirstOrDefault(r => r.commodity == commodity).amount.ToString();

    }

    public void RefreshCommodityViews()
    {
        _shopkeeperService = Mediator.Instance.GetService<ShopkeeperService>();
        var playerCommodities = _shopkeeperService.GetAvailableCommodities();
        foreach (var commodityView_kvp in _commodityViews)
        {
            if (playerCommodities.Find((r) => r.commodity.id == commodityView_kvp.Key) == null)
            {
                DestroyImmediate(commodityView_kvp.Value.gameObject);
            }
        }

        CreateCommodityViews();
    }

    public void CreateCommodityViews()
    {
        _shopkeeperService = Mediator.Instance.GetService<ShopkeeperService>();
        var playerCommodities = _shopkeeperService.GetAvailableCommodities();
        foreach (var commodityEntry in playerCommodities)
        {
            if (!_commodityViews.ContainsKey(commodityEntry.commodity.id))
            {
                var newObject = Instantiate(commodityViewPrefab, commodityViewsHolder);
                newObject.GetComponent<CommodityView>().SetCommodity(commodityEntry);
                newObject.GetComponent<BaseButtonExtended>().OnButtonClick += () => SetCommodity(commodityEntry.commodity);
                _commodityViews.Add(commodityEntry.commodity.id, newObject.GetComponent<CommodityView>());

            }

            _commodityViews[commodityEntry.commodity.id].SetCommodity(commodityEntry);

        }
    }


}
