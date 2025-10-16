using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using MyGame.Enums;
using System.Linq;
using Unity.Mathematics;
public class PortfolioApp : MonoBehaviour, IApp
{
    [SerializeField] private Transform _assetListContainer; //для динамического добавления активов
                                                            // [SerializeField] private GameObject _assetUIPrefab;



    // private Dictionary<Ticker, GameObject> _activeUIElements = new Dictionary<Ticker, GameObject>();
    private PortfollioService _portfolioService;
    Mediator _mediator;
    private AppController _appController;
    private void Start()
    {

    }
    private void Awake()
    {
        _portfolioService = Mediator.Instance.GetService<PortfollioService>();//?? 
        if (_portfolioService != null)
        {
            _portfolioService.OnPortfolioUpdated += UpdateUI;
            UpdateUI(_portfolioService.GetPortfolioSummary());
        }
        else
        {
            _mediator.GlobalEventBus.Publish<DebugLogErrorEvent>(new("PortfollioService  не найден"));
        }

        _appController = Mediator.Instance.GetService<AppController>();
        // Регистрируем этот экран в AppController
        _appController?.RegisterApp(this);
    }
    public void Open()
    {
        gameObject.SetActive(true);
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }
    private void OnDestroy()
    {
        if (_portfolioService != null)
        {
            _portfolioService.OnPortfolioUpdated -= UpdateUI;
        }
    }
    private void UpdateUI(PortfolioSummary summary)
    {

        foreach (var entry in summary.MyActives)
        {
            Ticker ticker = entry.Key;
            int quantity = entry.Value.Quantity;
            // if (quantity > 0)
            //{
            //  else//создание элемента
            //{
            //  CreateAssetUI(ticker, quantity);
            //}
            //}
            // else
            //{
            ///  _mediator.GlobalEventBus.Publish<DebugLogErrorEvent>(new("Информация об активе отсутствует"));
            //}
        }

    }
    private void CreateAssetUI(Ticker ticker, int quantity)
    {

        float currentPrice = _portfolioService.GetAssetPrice(ticker);
        //кнопка инфо об активе
        //  Button assetButton = assetUI.GetComponent<Button>();
        // if (assetButton != null)
        // {
        // assetButton.onClick.AddListener(() =>
        //   {
        // object assetData = _portfolioService.GetAssetByTicker(ticker);
        //  Mediator.Instance.GlobalEventBus.Publish(new OpenAssetPageEvent(assetData));
        //    });
        // }
        //действие кнопки sell(buy)

    }
    private void OnAddCashClick()
    {
        //_portfolioService.AddCash();
    }

    private void OnAnalyticsClick()
    {
        
    }
}


