using System;
using UniRx;
using Zenject;
public interface IService { }

public class Service : IService, IInitializable, IDisposable
{
    [Inject] protected EventBus GlobalEventBus;

    protected string SERVICE_NAME = "default service";
    private string SERVICE_NAME_POSTFIX = " /// ";

    protected CompositeDisposable disposables = new();


    public virtual void Initialize()
    {
    }


    public virtual void SubscribeToEvent<TEvent>(Action<TEvent> handler) where TEvent : IEvent
    {
        GlobalEventBus.Subscribe(handler);
    }

    public void OnDestroy()
    {
        Dispose();
    }

    public virtual void Dispose()
    {
        disposables?.Dispose();
    }

    private string CompileLogMessage(string message)
    {
        return string.Join(SERVICE_NAME, SERVICE_NAME_POSTFIX, message);
    }

    protected void ThrowNullArgument(string message, params object[] args)
    {
        throw new ArgumentNullException(CompileLogMessage(message));
    }
}
