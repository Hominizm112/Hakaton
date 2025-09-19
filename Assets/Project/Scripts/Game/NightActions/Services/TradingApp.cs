using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using TMPro;
using System;

public class TradingApp : MonoBehaviour, IApp, IStateListener
{
    [Header("UI Elements")]
    // Статичные кнопки
    public Button checkotherstocksButton;
    public Button checkotherbondsButton;
    public Button addcashButton;
    public Button getanalyticsButton;

    // Контейнеры для динамических кнопок
    public Transform stocksContentPanel;
    public Transform bondsContentPanel;
    public Transform buyActive;
    public Transform sellActive;

    // Текстовые поля
    public TextMeshProUGUI stockNameText;
    public TextMeshProUGUI quantityText;
    public TextMeshProUGUI valueText;
    public TextMeshProUGUI totalValueText;
    public TextMeshProUGUI cashBalanceText;

    [Header("Prefabs")]
    public GameObject stockButtonPrefab;
    public GameObject bondButtonPrefab;

    private PortfolioSummary _portfolioSummary;
    private AppController _appController;

    private Dictionary<string, Stock> _availableStocks = new Dictionary<string, Stock>();
    private Dictionary<string, Bond> _availableBonds = new Dictionary<string, Bond>();

    public event Action<IAsset, int> onBuyClicked;
    public event Action<IAsset, int> onSellClicked;
    public event Action<IAsset> onInfoClicked;
    private IAsset _Asset;
    private bool _isActive = false;
    private int _currentQuantity;
    public void Start()
    {
        Mediator.Instance?.SubscribeToState(this, Game.State.Trading);
        if (!_isActive && Mediator.Instance?.CurrentState == Game.State.Trading)
        {
            Open();
        }
    }
    public void Open()
    {

    }

    public void Close()
    {

    }

    private void OnBuyClick()
    {
        if (_Asset != null)
        {
            onBuyClicked?.Invoke(_Asset, 1);
        }
    }

    private void OnSellClick()
    {
        if (_Asset != null)
        {
            onSellClicked?.Invoke(_Asset, 1);
        }
    }

    private void OnInfoClick()
    {
        if (_Asset != null)
        {
            onInfoClicked?.Invoke(_Asset);
        }
    }
    public void OnStateChanged(Game.State newState)
    {if (
        newState == Game.State.Trading)
        {
            gameObject.SetActive(true);
            _isActive = true;
            RefreshUI();
        }
        else
        {
            gameObject.SetActive(false);
            _isActive = false;
        }
        
    }

    private void RefreshUI()
    {
        
    }
    //выбор актива
    private void OnAssetSelected(IAsset asset)
    {
        _Asset = asset;
        //UpdateSelectedAssetInfo();
        //PopulateActionButtons();
    }
    private void OnDestroy()
    {

        Mediator.Instance?.UnsubscribeFromState(this, Game.State.Trading);
    }
    
}
