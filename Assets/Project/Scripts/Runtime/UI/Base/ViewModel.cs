using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;

namespace GameCore.UI
{
    public abstract class ViewModel : IDisposable
    {
        private Dictionary<string, ViewModelBinder> _viewModelBinders = new();
        public IReadOnlyDictionary<string, ViewModelBinder> ViewModelBinders => _viewModelBinders;
        protected CompositeDisposable disposables = new();
        protected List<Action> onDisposeActions = new();

        protected void Bind(params ViewModelBinder[] binders)
        {
            _viewModelBinders = binders.ToDictionary(b => b.Id);
        }

        public abstract void Initialize();

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