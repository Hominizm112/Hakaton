using System;
using GameCore.UI;
using UniRx;
using Zenject;

namespace GameCore.Factories
{
    public class ViewModelFactory : Factory, IDisposable
    {
        [Inject] private DiContainer _objectResolver;
        private CompositeDisposable _disposables = new();

        public T Create<T>(params ViewBinder[] viewBinders) where T : ViewModel, new()
        {
            var viewModel = new T();
            _disposables.Add(viewModel);
            _objectResolver.Inject(viewModel);
            viewModel.Initialize();

            foreach (var viewBinder in viewBinders)
            {
                if (viewModel.ViewModelBinders.TryGetValue(viewBinder.Id, out var viewModelBinder))
                {
                    viewBinder.ViewModelBinder = viewModelBinder;
                }
            }

            return viewModel;
        }

        public void Dispose()
        {
            _disposables?.Dispose();
        }

    }
}
