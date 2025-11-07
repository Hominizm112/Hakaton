using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace GameCore.Utils
{
    public sealed class AsyncPool<T> where T : class
    {
        private readonly Stack<T> _available;
        private readonly HashSet<T> _inUse;
        private readonly Action<T> _onGet;
        private readonly Action<T> _onRelease;
        private readonly int _maxSize;

        public int CountAll { get; private set; }
        public int CountActive => CountAll - CountInactive;
        public int CountInactive => _available.Count;

        private AsyncPool(
            Action<T> onGet = null,
            Action<T> onRelease = null,
            int maxSize = 10000)
        {
            _onGet = onGet;
            _onRelease = onRelease;
            _maxSize = maxSize;
            _available = new Stack<T>();
            _inUse = new HashSet<T>();
        }

        public static async UniTask<AsyncPool<T>> CreateAsync(
            Func<CancellationToken, UniTask<T>> createFuncAsync,
            Action<T> onGet = null,
            Action<T> onRelease = null,
            int initialCapacity = 10,
            int maxSize = 10000,
            CancellationToken cancellationToken = default)
        {
            var pool = new AsyncPool<T>(onGet, onRelease, maxSize);
            await pool.InitializeAsync(createFuncAsync, initialCapacity, cancellationToken);
            return pool;
        }

        private async UniTask InitializeAsync(
            Func<CancellationToken, UniTask<T>> createFuncAsync,
            int initialCapacity,
            CancellationToken cancellationToken)
        {
            var tasks = new List<UniTask<T>>();

            for (int i = 0; i < initialCapacity; i++)
            {
                tasks.Add(createFuncAsync(cancellationToken));
            }

            var results = await UniTask.WhenAll(tasks);

            foreach (var result in results)
            {
                _available.Push(result);
                CountAll++;
            }
        }

        public T Get()
        {
            if (_available.Count == 0)
            {
                throw new InvalidOperationException("No objects available in pool. Consider increasing initial capacity.");
            }

            var obj = _available.Pop();
            _onGet?.Invoke(obj);
            _inUse.Add(obj);
            return obj;
        }

        public void Release(T element)
        {
            if (element == null)
                throw new ArgumentNullException(nameof(element));

            _onRelease?.Invoke(element);
            _inUse.Remove(element);

            if (CountInactive < _maxSize)
            {
                _available.Push(element);
            }
        }

    }
}