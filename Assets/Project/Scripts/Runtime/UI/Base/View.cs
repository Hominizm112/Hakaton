using System;
using System.Collections.Generic;
using GameCore.Factories;
using UniRx;
using UnityEngine;
using Zenject;

namespace GameCore.UI
{
    public abstract class View<TViewModel> : View
        where TViewModel : ViewModel, new()
    {
        [Inject] private ViewModelFactory _viewModelFactory;

        private ViewBinder[] _viewBinders;
        public TViewModel ViewModel { get; private set; }



        protected void Bind(params ViewBinder[] viewBinders)
        {
            ViewModel = _viewModelFactory.Create<TViewModel>(viewBinders);
            foreach (var viewBinder in viewBinders)
            {
                viewBinder.Initialize();
            }
        }

        public override void Dispose()
        {
            foreach (var viewBinder in _viewBinders)
            {
                viewBinder.Dispose();
            }


        }
    }

    public abstract class View : MonoBehaviour, IDisposable
    {
        protected CompositeDisposable disposables = new();
        protected List<Action> onDisposeActions = new();
        public abstract void Initialize();

        public virtual void Open()
        {
            gameObject.SetActive(true);
        }

        public virtual void Close()
        {
            gameObject.SetActive(false);
        }

        public virtual void Dispose()
        {
            disposables.Dispose();
            foreach (var disposeEvent in onDisposeActions)
            {
                disposeEvent?.Invoke();
            }
        }
    }
}