using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using Zenject;

public class CustomerService : Service
{
    [Header("Customer settings")]
    [SerializeField] private GameObject customerPrefab;
    [SerializeField] private Transform customersHolder;
    [SerializeField] private Vector2 customerStartPosition;
    [SerializeField] private Vector2 customerAtStallPosition;
    [SerializeField] private Vector2 customerEndPosition;

    [Header("Customer animation settings")]

    [SerializeField] private float customerShowDuration;
    [SerializeField] private Ease customerShowEase;
    [SerializeField] private float customerHideDuration;
    [SerializeField] private Ease customerHideEase;

    private Customer _customerAtStall;
    private Customer _lastCustomerAtStall;
    public Customer CustomerAtStall => _customerAtStall;

    /// <summary>
    /// Object pool for managing Customer instances with their activation states.
    /// Key (Customer): The Customer instance associated with the activation state
    /// Value (bool): Represents whether the customer is currently active (true) or available for reuse (false)
    /// </summary>
    private Dictionary<Customer, bool> _customersPool = new();

    [Inject]
    public void Awake()
    {
        InitializeCustomers();
    }

    private void InitializeCustomers()
    {
        if (_customersPool.Count != 0) return;
        for (int i = 0; i < 2; i++)
        {
            // _customersPool.Add(Instantiate(customerPrefab, customersHolder).GetComponent<Customer>(), false);
            _customersPool.Last().Key.gameObject.SetActive(false);
        }
    }


    private Customer GetFreeCustomer()
    {

        return _customersPool.FirstOrDefault(r => !r.Value).Key;
    }

    private void SetCustomerActive(Customer customer, bool active)
    {
        _customersPool[customer] = active;
        customer.gameObject.SetActive(active);
    }



    public void RequestCustomer()
    {
        SpawnCustomer();
    }

    private void SpawnCustomer()
    {
        Customer customer = GetFreeCustomer();

        if (customer == null)
        {
            InitializeCustomers();
            customer = GetFreeCustomer();
        }

        if (customer == null)
        {
            ColorfulDebug.LogError("Free customer not found.");
            return;
        }

        SetCustomerActive(customer, true);
        customer.gameObject.transform.position = customerStartPosition;
        customer.gameObject.transform.DOMove(customerAtStallPosition, customerShowDuration).SetEase(customerShowEase).OnComplete(() =>
        {
            _customerAtStall = customer;
            _lastCustomerAtStall = customer;
        });

    }

    public void DespawnCustomer()
    {
        if (_customerAtStall == null)
        {
            ColorfulDebug.LogError("Tried to despawn customer, but none is at stall.");
            return;
        }

        _customerAtStall.gameObject.transform.DOMove(customerEndPosition, customerHideDuration).SetEase(customerHideEase).OnComplete(() =>
        {
            SetCustomerActive(_lastCustomerAtStall, false);
        });

        _customerAtStall = null;

    }

    public override void Dispose()
    {
    }
}

