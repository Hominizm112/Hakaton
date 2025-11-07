using Cysharp.Threading.Tasks;
using DG.Tweening;
using GameCore.UI;
using UniRx;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

namespace TeaGame.UI.View
{
    public class CustomerFlowView : View<CustomerFlowViewModel>
    {
        [SerializeField] private AssetReference customerRef;
        [SerializeField] private Transform customerSpawnPosition;
        [SerializeField] private Transform customerAtStallPosition;
        [SerializeField] private Transform customerDespawnPosition;
        [SerializeField] private Transform customersParent;

        [Header("Animation Settings")]
        [SerializeField] private float customerSpawnStallDuration = 1f;
        [SerializeField] private Ease customerSpawnStallEase = Ease.Linear;
        [SerializeField] private float customerStallDespawnDuration = 1f;
        [SerializeField] private Ease customerStallDespawnEase = Ease.Linear;

        [SerializeField] private Button button;


        public override void Initialize()
        {
            Bind();

            ViewModel.Initialize(customerRef, customersParent);
            button.onClick.AddListener(() => ViewModel.RequestCustomer());

            ViewModel.customerRequested
                .Subscribe(HandleCustomerSpawn)
                .AddTo(disposables);


            ViewModel.customerReleased
                .Subscribe(HandleCustomerRelease)
                .AddTo(disposables);
        }

        private void HandleCustomerSpawn(Customer newCustomer)
        {
            newCustomer.gameObject.SetActive(true);
            newCustomer.transform.position = customerSpawnPosition.position;
            newCustomer.transform.DOMove(customerAtStallPosition.position, customerSpawnStallDuration).SetEase(customerSpawnStallEase).OnComplete(() =>
            {
                ViewModel.SetCustomerAtStall(true);
                newCustomer.SetAnimating(false);

            });
            newCustomer.SetAnimating(true);
        }

        private void HandleCustomerRelease(Customer oldCustomer)
        {
            oldCustomer?.transform.DOMove(customerDespawnPosition.position, customerStallDespawnDuration).SetEase(customerStallDespawnEase).OnComplete(() =>
            {
                oldCustomer.gameObject.SetActive(false);
                oldCustomer.SetAnimating(false);
            });

        }

        private void HandleCustomerDespawn()
        {

        }
    }
}