
using Cysharp.Threading.Tasks;
using GameCore.UI;
using GameCore.Utils;
using TeaGame.States;
using UniRx;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Zenject;

public class CustomerFlowViewModel : ViewModel
{
    public ReactiveCommand<Customer> customerRequested = new();
    public ReactiveCommand<Customer> customerReleased = new();

    private AsyncPool<Customer> _customersPool;
    public AsyncPool<Customer> CustomersPool => _customersPool;

    private Customer _activeCustomer;
    private ReactiveProperty<bool> _isCustomerAtStall = new();

    private bool _canRequestCustomer = true;

    [Inject] private StallState _stallState;
    [Inject] private EventBus _eventBus;

    public override void Initialize()
    {
        _isCustomerAtStall
            .Subscribe(val => _stallState.SetCustomerAtStall(val))
            .AddTo(disposables);
    }

    public async void Initialize(AssetReference customerRef, Transform customersParent)
    {
        await CreateCustomersPool(customerRef, customersParent);
        RequestCustomer();

        _stallState.ItemSoldCommand
            .Subscribe(_ => RequestCustomer())
            .AddTo(disposables);

        disposables.Add(
            _eventBus.Subscribe<TimeTrackCompletedEvent>(_ => _canRequestCustomer = false));
    }

    public async UniTask CreateCustomersPool(AssetReference customerRef, Transform customersParent)
    {
        _customersPool = await AsyncPool<Customer>.CreateAsync(
            createFuncAsync: async (cancellationToken) =>
            {
                var obj = await Addressables.InstantiateAsync(customerRef, customersParent);
                obj.SetActive(false);
                obj.transform.localScale = Vector3.one;
                return obj.GetComponent<Customer>();
            },
            initialCapacity: 2,
            maxSize: 5
        );
    }


    public void RequestCustomer()
    {
        if (!_canRequestCustomer)
        {
            ReleaseCustomer();
            return;
        }

        if (_activeCustomer != null && _activeCustomer.Animating)
            return;

        var newCustomer = _customersPool.Get();
        ReleaseCustomer();
        _activeCustomer = newCustomer;
        customerRequested.Execute(_activeCustomer);


    }


    public void ReleaseCustomer()
    {
        if (_activeCustomer == null) return;

        var oldCustomer = _activeCustomer;
        _isCustomerAtStall.Value = false;
        _customersPool.Release(_activeCustomer);
        _activeCustomer = null;
        customerReleased.Execute(oldCustomer);


    }

    public void SetCustomerAtStall(bool atStall)
    {
        _isCustomerAtStall.Value = atStall;
    }

}