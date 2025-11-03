
using System;
using System.Collections.Generic;
using UniRx;
using Zenject;

namespace GameCore.Behaviours
{
    public abstract class Behaviour : IDisposable
    {
        protected CompositeDisposable _disposables = new();
        protected List<Action> _onDispose;

        [Inject]
        public void Construct()
        {
            Initialize();
        }

        public abstract void Initialize();

        public virtual void Dispose()
        {
            _disposables.Dispose();

            foreach (var @event in _onDispose)
            {
                @event?.Invoke();
            }
        }
    }
}