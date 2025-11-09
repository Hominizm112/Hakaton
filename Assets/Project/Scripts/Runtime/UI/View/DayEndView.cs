using GameCore.UI;
using UniRx;
using UnityEngine;

namespace TeaGame.Views
{
    public class DayEndView : View<DayEndViewModel>
    {
        [SerializeField] private GameObject dayEndUI;

        public override void Initialize()
        {
            Bind();

            ViewModel.DayEndedCommand
                .Subscribe(_ => HandleDayEnd())
                .AddTo(disposables);

            dayEndUI.SetActive(false);

        }

        private void HandleDayEnd()
        {
            dayEndUI.SetActive(true);
        }
    }
}