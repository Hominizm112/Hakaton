using System;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class ItemView : MonoBehaviour, IDisposable
{
    [SerializeField] private Image _icon;
    [SerializeField] private TMP_Text _quantityText;
    [SerializeField] private TMP_Text _nameText;
    [SerializeField] public ButtonExtended SelectButton;

    private ItemData _itemData;
    public ItemData ItemData => _itemData;
    private CompositeDisposable _disposables = new CompositeDisposable();

    public void Initialize(ItemData itemData)
    {
        _itemData = itemData;

        _itemData.Quantity.Subscribe(val => _quantityText.text = val.ToString()).AddTo(_disposables);

        UpdateDisplay();
    }

    private void UpdateDisplay()
    {
        _nameText.text = _itemData.Id;
        _quantityText.text = _itemData.Quantity.Value.ToString();


    }

    public void Dispose()
    {
        _disposables?.Dispose();
    }


}
