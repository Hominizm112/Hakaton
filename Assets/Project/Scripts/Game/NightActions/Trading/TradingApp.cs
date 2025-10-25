using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using TMPro;
using System;
using UnityEngine.Events;
using System.Runtime.CompilerServices;
using MyGame.Enums;
using Zenject;


public class TradingApp : BaseApp
{
    //динамические кнопки
    [Header("UI Elements - Containers")]
    [SerializeField] private Transform _assetListContainer;// Контейнер, где будут создаваться кликабельные кнопки
    [SerializeField] private GameObject _assetUIPrefab;

    [Inject] private PortfollioService _portfolioService;



    private PortfolioSummary _portfolioSummary;
    private Dictionary<Ticker, GameObject> _activeUIElements = new();

    protected void Awake()
    {
        // _portfolioService.OnPortfolioUpdated += UpdateUI;
    }

    private void UpdateUI(PortfolioSummary summary)
    {
        // Очищаем старые UI-элементы, чтобы избежать дублирования
        foreach (var element in _activeUIElements.Values)
        {
            Destroy(element);
        }
        _activeUIElements.Clear();

        //полные списки доступных активов



    }

    private void CreateAssetUI(Ticker ticker, int quantity, TradeType tradeType)
    {
        GameObject assetUI = Instantiate(_assetUIPrefab, _assetListContainer);
        _activeUIElements[ticker] = assetUI;

        TMP_Text assetNameText = assetUI.GetComponentInChildren<TMP_Text>();
        if (assetNameText != null)
        {
            assetNameText.text = $"{ticker} (x{quantity})";
        }

        // QuickTradeButton sellButton = assetUI.GetComponentInChildren<QuickTradeButton>();
        // if (sellButton != null)
        // {
        // sellButton.Ticker = ticker;
        // sellButton.Price = _portfolioService.GetAssetPrice(ticker);
        // sellButton.TradeType = tradeType;
        // sellButton.gameObject.SetActive(true);
        // }
    }

}