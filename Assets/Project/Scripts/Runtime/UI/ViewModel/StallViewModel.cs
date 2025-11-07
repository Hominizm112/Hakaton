using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using GameCore.UI;
using GameCore.Utils;
using TeaGame.Services;
using TeaGame.States;
using TeaGame.Views;
using UniRx;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Zenject;

public class StallViewModel : ViewModel
{
    [Inject] private EventBus _eventBus;
    [Inject] private DiContainer _diContainer;
    [Inject] private StallState _stallState;
    [Inject] private TeaMixerService _teaMixService;
    [Inject] private ObjectRegistry _objectRegistry;

    private ReactiveDictionary<string, ItemData> _itemsInBoxes = new();
    public IReadOnlyReactiveDictionary<string, ItemData> ItemsInBoxes => _itemsInBoxes;

    public ReactiveCommand SpawnItem = new();

    private ReactiveProperty<string> _selectedBoxId = new();
    private CompositeDisposable _disposables = new();
    private List<Action> _onDispose = new();
    private StallBoxSelectMode _stallBoxSelectMode = StallBoxSelectMode.TakeItemFromBox;
    private List<GameObject> _instances = new();

    private Placer _placer;

    public override void Initialize()
    {
        _diContainer.BindInterfacesAndSelfTo(GetType()).FromInstance(this).AsSingle().NonLazy();
        _disposables.Add(_eventBus.Subscribe<ScreenOpenEvent>(e =>
        {
            if (e.ScreenView is StallChangeItemsView)
            {
                SwitchStallBoxSelectMode(StallBoxSelectMode.SetItemInBox);
            }
        }));

        _disposables.Add(_eventBus.Subscribe<ScreenCloseEvent>(e =>
        {
            if (e.ScreenView is StallChangeItemsView)
            {
                SwitchStallBoxSelectMode(StallBoxSelectMode.TakeItemFromBox);
            }
        }));


        _stallState.onLoad += SetItemInBoxes;
        _stallState.onSaveStarted += SaveItemsInBoxes;

    }

    public async UniTask<(Dragger, Placer)> InitializeDragger(AssetReference dragObjectRef)
    {
        HiddenContainer hiddenContainer = new();
        Dragger dragger;

        var handle = Addressables.InstantiateAsync(dragObjectRef, hiddenContainer.Container);
        await handle.Task;

        if (handle.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
        {
            var createdObject = handle.Result.gameObject;
            _instances.Add(createdObject);

            dragger = createdObject.GetComponent<Dragger>();
            _placer = createdObject.GetComponent<Placer>();

            _objectRegistry.Register(_placer);

            _teaMixService.SetPlacer(_placer);

            _disposables.Add(dragger);
            _disposables.Add(_placer);
            hiddenContainer.Release(createdObject.transform);
            hiddenContainer.Dispose();

            return (dragger, _placer);
        }

        throw new Exception($"Error while creating dragger for Stall");
    }

    private void SetItemInBoxes(Dictionary<string, ItemData> data)
    {
        foreach (var kvp in data)
        {
            _itemsInBoxes.Add(kvp.Key, kvp.Value);
        }
    }

    private void SaveItemsInBoxes()
    {
        _stallState.SaveData(_itemsInBoxes.ToDictionary(pair => pair.Key, pair => pair.Value));
    }

    private void SwitchStallBoxSelectMode(StallBoxSelectMode mode)
    {
        _stallBoxSelectMode = mode;
    }


    public void SelectStallBox(string id)
    {
        _selectedBoxId.Value = id;
        if (CanSpawnNewItem())
        {
            SpawnItem.Execute();
        }
    }

    public bool CanSpawnNewItem()
    {
        _itemsInBoxes.TryGetValue(_selectedBoxId.Value, out var value);
        return value != null && _stallBoxSelectMode == StallBoxSelectMode.TakeItemFromBox && !_placer.IsActive;
    }

    public ItemData GetItemInSelectedBox()
    {
        return _itemsInBoxes[_selectedBoxId.Value];
    }

    public void PlaceItem(ItemData itemData)
    {
        _itemsInBoxes[_selectedBoxId.Value] = itemData;
    }

    public bool TryPlaceItem(ItemData itemData)
    {

        if (_stallBoxSelectMode == StallBoxSelectMode.SetItemInBox)
        {
            if (itemData == null || string.IsNullOrEmpty(_selectedBoxId.Value))
            {
                return false;
            }
            _itemsInBoxes[_selectedBoxId.Value] = itemData;
            return true;
        }

        return false;
    }

    public override void Dispose()
    {
        base.Dispose();
        _disposables.Dispose();

        foreach (var @event in _onDispose)
        {
            @event?.Invoke();
        }

        foreach (var instance in _instances)
        {
            Addressables.ReleaseInstance(instance);
        }



        _stallState.onLoad -= SetItemInBoxes;
        _stallState.onSaveStarted -= SaveItemsInBoxes;
    }


    private enum StallBoxSelectMode
    {
        SetItemInBox,
        TakeItemFromBox
    }



}
