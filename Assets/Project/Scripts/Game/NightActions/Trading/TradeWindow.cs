using UnityEngine;
using TMPro;
using MyGame.Enums;
using UnityEngine.UI;
using System;
public class TradingWindowView : MonoBehaviour
{
    [SerializeField] private Button _confirmButton;// кнопка подтвердить в окне торговли
    [SerializeField] private InputField _quantityInput;//ввод количества в окне торговли
    private TMP_Text _tickerText;
    private TMP_Text _activeNameText;
    private Ticker _currentTicker;
    private TradeType _currentTradeType;
    private TMP_Text _currentPrice;
    public event Action<TradeType, Ticker, int> OnTradeConfirmed;//событие подтвреждения сделки

    private void Awake()
    {
        _confirmButton.onClick.AddListener(() =>
        {
            int quantity;
            if (int.TryParse(_quantityInput.text, out quantity) && quantity > 0)
            {
                OnTradeConfirmed?.Invoke(_currentTradeType, _currentTicker, quantity);
            }

            else
            {
                Debug.Log("Некорректный ввод количества");
                return;
            }
        });
    }

    public void Show(TradeType type, Ticker ticker, int price)
    {
        _currentTradeType = type;
        _currentTicker = ticker;
        string action = (type == TradeType.Buy) ? "Покупка" : "Продажа";
        string companyName = CompanyInfo.ActiveName.ContainsKey(ticker)
        ? CompanyInfo.ActiveName[ticker]
        : ticker.ToString();
        _activeNameText.text = companyName;
        _tickerText.text = $"{ticker}";
        _currentPrice.text = $"Текущая цена: {price}";
        this.gameObject.SetActive(true);
    }

    public void TradeWindowClose()
    {
        this.gameObject.SetActive(false);
        ColorfulDebug.LogGreen("Успешная операция");


    }


    // public void UpdateAssetPrice(float price)//<--заменить на int
    // {
    // _currentPrice.text = $"Текущая цена: {price}";
    // }
}
   // private void UpdateTotal(string newQuantity)//обновление стоимости сделки
   // {
     //   if (int.TryParse(newQuantity, out int quantity) && quantity > 0)
      //  {
      //      float total = _currentPrice * quantity;
       //     _totalText.text = total.ToString("C");
       // }
      //  else
      //  {
       //     _totalText.text = "0";
      //  }
  //  }
    //public void OnTradeButtonClick()
  //  {
      //  if (int.TryParse(_quantityInput.text, out int quantity))
     //   {

    //        Mediator.Instance.GlobalEventBus.Publish(new TradeRequestEvent(_currentTradeType, _currentTicker, _currentPrice, quantity));
    //        gameObject.SetActive(false);
      ////  }
   // }

