
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using GameCore.UI;
using GameCore.Utils;
using UniRx;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class CustomerFlowViewModel : ViewModel
{
    public ReactiveCommand<Customer> customerRequested = new();
    public ReactiveCommand<Customer> customerReleased = new();

    private AsyncPool<Customer> _customersPool;
    public AsyncPool<Customer> CustomersPool => _customersPool;

    private Customer _activeCustomer;
    private bool _isCustomerAtStall;

    public override void Initialize()
    {
    }

    public void Initialize(AssetReference customerRef, Transform customersParent)
    {
        CreateCustomersPool(customerRef, customersParent);
    }

    public async void CreateCustomersPool(AssetReference customerRef, Transform customersParent)
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
        _isCustomerAtStall = false;
        _customersPool.Release(_activeCustomer);
        _activeCustomer = null;
        customerReleased.Execute(oldCustomer);


    }

    public void SetCustomerAtStall(bool atStall)
    {
        _isCustomerAtStall = atStall;
    }

}