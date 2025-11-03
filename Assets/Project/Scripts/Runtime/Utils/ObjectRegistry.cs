

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UniRx;

namespace GameCore.Utils
{
    public class ObjectRegistry
    {
        private readonly Dictionary<Type, object> _objects = new Dictionary<Type, object>();
        private readonly Dictionary<Type, List<object>> _multipleObjects = new Dictionary<Type, List<object>>();

        public bool Register<T>(T instance) where T : class
        {
            if (instance == null)
                throw new ArgumentNullException(nameof(instance));

            Type type = typeof(T);

            if (_objects.ContainsKey(type))
                return false;

            if (instance is IDisposable disposable)
            {
                var wrappedInstance = new DisposableWrapper<T>(instance, () => Unregister(type));
                _objects[type] = wrappedInstance;
            }
            else
            {
                _objects[type] = instance;
            }

            return true;
        }

        private void Unregister(Type type)
        {
            if (_objects.Remove(type, out object instance))
            {
            }
        }

        public void RegisterMultiple<T>(T instance) where T : class
        {
            if (instance == null)
                throw new ArgumentNullException(nameof(instance));

            Type type = typeof(T);

            if (!_multipleObjects.ContainsKey(type))
                _multipleObjects[type] = new List<object>();

            _multipleObjects[type].Add(instance);
        }

        public T Get<T>() where T : class
        {
            Type type = typeof(T);

            if (_objects.TryGetValue(type, out object obj))
            {
                if (obj is DisposableWrapper<T> wrapper)
                    return wrapper.Instance;
                else
                    return (T)obj;

            }

            throw new InvalidOperationException($"No object of type {type.Name} registered");
        }

        public bool TryGet<T>(out T result) where T : class
        {
            result = null;
            Type type = typeof(T);

            if (_objects.TryGetValue(type, out object obj))
            {
                result = (T)obj;
                return true;
            }

            return false;
        }

        public IEnumerable<T> GetAll<T>() where T : class
        {
            Type type = typeof(T);

            if (_objects.TryGetValue(type, out object singleObj))
                yield return (T)singleObj;

            if (_multipleObjects.TryGetValue(type, out List<object> multipleObjs))
            {
                foreach (object obj in multipleObjs)
                {
                    yield return (T)obj;
                }
            }
        }

        public bool IsRegistered<T>() where T : class
        {
            return _objects.ContainsKey(typeof(T)) || _multipleObjects.ContainsKey(typeof(T));
        }

        public void Clear()
        {
            _objects.Clear();
            _multipleObjects.Clear();
        }

        private class DisposableWrapper<T> : IDisposable where T : class
        {
            private readonly Action _onDisposed;
            private bool _disposed = false;

            public T Instance { get; }

            public DisposableWrapper(T instance, Action onDisposed)
            {
                Instance = instance;
                _onDisposed = onDisposed;

                // If the wrapped instance is disposed, trigger our handler
                if (instance is IDisposable disposable)
                {
                    // For reactive disposables
                    if (disposable is IObservable<Unit> observableDisposable)
                    {
                        observableDisposable.Subscribe(_ => Dispose());
                    }
                }
            }

            public void Dispose()
            {
                if (!_disposed)
                {
                    _disposed = true;
                    _onDisposed?.Invoke();

                    // Also dispose the inner instance if it's still valid
                    if (Instance is IDisposable disposable)
                    {
                        disposable.Dispose();
                    }
                }
            }
        }
    }
}