using UnityEngine;
using TMPro;
using UnityEngine.UI;
using MyGame.Enums;
public class TradeWindow : MonoBehaviour
{
    [SerializeField] private TMP_Text _headerText; 
    [SerializeField] private TMP_Text _assetNameText;
    [SerializeField] private TMP_Text _assetPriceText;
    [SerializeField] private TMP_InputField _quantityInput;
    [SerializeField] private TMP_Text _totalText;


    private string _currentTicker;
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
        _assetNameText.text = _currentTicker;
        _assetPriceText.text = _currentPrice.ToString("C");

        if (_currentTradeType == TradeType.Buy)
        {
            _headerText.text = "Цена покупки";
        }
        else
        {
            _headerText.text = "Цена продажи";
        }
        _quantityInput.onValueChanged.AddListener(UpdateTotal);
    }

    private void UpdateTotal(string newQuantity)//обновление стоимости сделки
    {
        if (int.TryParse(newQuantity, out int quantity) && quantity > 0)
        {
            float total = _currentPrice * quantity;
            _totalText.text = total.ToString("C");
        }
        else
        {
            _totalText.text = "0";
        }
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
