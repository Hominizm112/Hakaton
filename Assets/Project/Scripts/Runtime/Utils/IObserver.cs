using System;
using UniRx;

namespace GameCore.Utils
{
    public interface IObserver : IObserver<Unit>
    {
        void OnNext();
    }
}