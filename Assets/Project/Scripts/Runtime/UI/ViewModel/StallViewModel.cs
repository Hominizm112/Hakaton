using GameCore.UI;
using TriInspector;
using UniRx;
using UnityEngine;

public class StallViewModel : ViewModel
{
    private ReactiveCollection<ItemData> _itemsInStall = new();
    public IReadOnlyReactiveCollection<ItemData> ItemsInStall => _itemsInStall;

    private ReactiveDictionary<string, ItemData> _itemsInBoxes = new();
    public IReadOnlyReactiveDictionary<string, ItemData> ItemsInBoxes => _itemsInBoxes;


    private string _selectedBoxId;

    private CompositeDisposable _disposables = new();



    public override void Initialize()
    {
    }


    public void SelectStallBox(string id)
    {
        _selectedBoxId = id;
    }

    public bool CanSpawnNewItem()
    {
        _itemsInBoxes.TryGetValue(_selectedBoxId, out var value);
        return value != null;
    }

    public void PlaceItem(ItemData itemData)
    {
        _itemsInBoxes[_selectedBoxId] = itemData;

    }



}
