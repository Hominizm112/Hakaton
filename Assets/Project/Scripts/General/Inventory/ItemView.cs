using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class ItemView : MonoBehaviour
{
    [SerializeField] private Image _icon;
    [SerializeField] private TMP_Text _quantityText;
    [SerializeField] private TMP_Text _nameText;
    [SerializeField] private Button _useButton;
    [SerializeField] private Button _sellButton;
    [SerializeField] private Image _rarityBackground;

    private ItemModel _itemData;
    private InventoryViewModel _viewModel;
    private CompositeDisposable _disposables = new CompositeDisposable();

    public void Initialize(ItemModel itemData, InventoryViewModel viewModel)
    {
        _itemData = itemData;
        _viewModel = viewModel;

        UpdateDisplay();
        SetupButtons();
    }

    private void UpdateDisplay()
    {
        _nameText.text = _itemData.Id;
        _quantityText.text = _itemData.Quantity.Value > 1 ? _itemData.Quantity.ToString() : "";

        _rarityBackground.color = GetRarityColor(_itemData.Rarity);

        // TODO: Load actual icon
        // _icon.sprite = LoadIcon(_itemData.ItemId);
    }

    private void SetupButtons()
    {
        // _sellButton.OnClickAsObservable()
        //     .Subscribe(_ => _viewModel.SellItemCommand.Execute(_itemData))
        //     .AddTo(_disposables);
    }

    private Color GetRarityColor(ItemRarity rarity)
    {
        return rarity switch
        {
            ItemRarity.Common => Color.gray,
            ItemRarity.Rare => Color.blue,
            ItemRarity.Epic => Color.magenta,
            ItemRarity.Legendary => Color.yellow,
            _ => Color.white
        };
    }

    private void OnDestroy()
    {
        _disposables?.Dispose();
    }
}
