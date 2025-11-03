
using GameCore.UI;
using UnityEngine;

namespace TeaGame.Views
{
    public class SellBellView : View<SellBellViewModel>
    {
        [SerializeField] private ButtonExtendedViewBinder sellButton = new("sellButton");
        [SerializeField] private AreaDetectorViewBinder sellArea = new("sellArea");
        public override void Initialize()
        {
            Bind(sellButton, sellArea);
        }
    }

    public interface IRuntimeView
    {
        public virtual void Initialize() { }
    }
}