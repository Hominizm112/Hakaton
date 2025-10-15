using UnityEngine;
using TMPro;
using MyGame.Enums;
public class TradeWindow : MonoBehaviour
{
<<<<<<< Updated upstream
    [SerializeField] private TMP_Text _headerText;
    [SerializeField] private TMP_Text _assetNameText;
    [SerializeField] private TMP_Text _assetPriceText;

    //TODO: убрать _quantityInput
    [SerializeField] private TMP_InputField _quantityInput;
    [SerializeField] private TMP_Text _totalText;


=======
    [SerializeField] private Button _confirmButton;// кнопка подтвердить в окне торговли
    //[SerializeField] private InputField _quantityInput;//ввод количества в окне торговли
    private TMP_Text _tickerText;
    private TMP_Text _activeNameText;
>>>>>>> Stashed changes
    private Ticker _currentTicker;
    private float _currentPrice;
    private TradeType _currentTradeType;


    private void Awake()
    {
        Mediator.Instance.GlobalEventBus.Subscribe<OpenTradeWindowEvent>(HandleOpenEvent);
    }

    private void HandleOpenEvent(OpenTradeWindowEvent e)
    {
        // открытие и заполнение данных в окне
        gameObject.SetActive(true);
        _currentTicker = e.Ticker;
        _currentPrice = e.Price;
        _currentTradeType = e.TradeType;
        _assetNameText.text = _currentTicker.ToString();
        _assetPriceText.text = _currentPrice.ToString();

        switch (_currentTradeType)
        {
<<<<<<< Updated upstream
            case TradeType.Buy:
                _headerText.text = "Цена покупки";
                break;

            case TradeType.Sell:
                _headerText.text = "Цена продажи";
                break;
        }

        _quantityInput.onValueChanged.AddListener(UpdateTotal);
=======
            //localizestringevent
            //int quantity;
            //if (quantity > 0)
            //{
            //   OnTradeConfirmed?.Invoke(_currentTradeType, _currentTicker, quantity);
            // }

            // else
            //{
            //    Debug.Log("Некорректный ввод количества");
            //  return;
            //}
        });
>>>>>>> Stashed changes
    }

    private void UpdateTotal(string newQuantity)//обновление стоимости сделки
    {
<<<<<<< Updated upstream
        if (int.TryParse(newQuantity, out int quantity) && quantity > 0)
        {
            float total = _currentPrice * quantity;
            _totalText.text = total.ToString("C");
        }
        else
        {
            _totalText.text = "0";
        }
=======
        _currentTradeType = type;
        _currentTicker = ticker;
        string action = (type == TradeType.Buy) ? "Покупка" : "Продажа";
        // string companyName = CompanyInfo.ActiveName.ContainsKey(ticker)
        //? CompanyInfo.ActiveName[ticker]
        // ticker.ToString();
        //_activeNameText.text = companyName;
        _tickerText.text = $"{ticker}";
        _currentPrice.text = $"Текущая цена: {price}";
        _confirmButton.gameObject.SetActive(true);
        this.gameObject.SetActive(true);
>>>>>>> Stashed changes
    }
    public void OnTradeButtonClick()
    {
        if (int.TryParse(_quantityInput.text, out int quantity))
        {

            Mediator.Instance.GlobalEventBus.Publish(new TradeRequestEvent(_currentTradeType, _currentTicker, _currentPrice, quantity));
            gameObject.SetActive(false);
        }
    }
}
