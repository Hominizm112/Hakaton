using UnityEngine;
using TMPro;
using System.Collections.Generic;
using MyGame.Enums;
public class AssetPageManager : MonoBehaviour
{
    [SerializeField] private GameObject _stockPageUI;
    [SerializeField] private GameObject _bondPageUI;

    // Элементы для страницы акций
    [Header("Stock Page ")]
    [SerializeField] private TMP_Text _gainLossText;
    [SerializeField] private TMP_Text _gainLossPercentText;
    [SerializeField] private TMP_Text _openPriceText;
    [SerializeField] private TMP_Text _closePriceText;
    [SerializeField] private TMP_Text _dividendDateText;
    [SerializeField] private TMP_Text _dividendSizeText;
    [SerializeField] private TMP_Text _dividendPercentText;
    [SerializeField] private TMP_Text _descriptionText;
    [SerializeField] private TMP_Text _sectorText;
    [SerializeField] private TMP_Text _countryText;
    [SerializeField] private TMP_Text AverageDivYield;
    [SerializeField] private TMP_Text LevelStability;

    [Header("Bond Page ")]
    [SerializeField] private TMP_Text _redemptionDateText;
    [SerializeField] private TMP_Text _couponSizeText;
    [SerializeField] private TMP_Text _couponPaymentDateText;
    [SerializeField] private TMP_Text _couponCountText;
    [SerializeField] private TMP_Text _nominalText;
    [SerializeField] private TMP_Text _issuerRatingText;
    [SerializeField] private TMP_Text _issuerInfoText;

    //общие кнопки
    [SerializeField] private TMP_Text _tickerText;
    [SerializeField] private TMP_Text _priceText;
    [SerializeField] private TMP_Text _inPortfolioText;
    [SerializeField] private QuickTradeButton _buyButton;
    [SerializeField] private QuickTradeButton _sellButton;


    private PortfollioService _portfolioService;

    private void Awake()
    {
        _portfolioService = Mediator.Instance.GetService<PortfollioService>();
        Mediator.Instance.GlobalEventBus.Subscribe<OpenAssetPageEvent>(HandleOpenAssetPage);
    }

    private void HandleOpenAssetPage(OpenAssetPageEvent e)
    {
        _stockPageUI.SetActive(false);
        _bondPageUI.SetActive(false);

        if (e.Type == AssetType.Stock)
        {
        }
    }


}
