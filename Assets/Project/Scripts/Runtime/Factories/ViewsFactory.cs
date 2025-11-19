using System;
using GameCore.UI;
using UniRx;
using UnityEngine;
using Zenject;

namespace GameCore.Factories
{
    public class ViewsFactory : Factory
    {
        [Inject] private DiContainer _objectResolver;


        public TView Create<TView>(TView prefab, Transform parent)
            where TView : View
        {
            var view = UnityEngine.Object.Instantiate(prefab, parent);
            InitializeView(view);
            return view;
        }

        public void InitializeView(View view)
        {
            _objectResolver.Inject(view);
            view.Initialize();
        }

    }
}