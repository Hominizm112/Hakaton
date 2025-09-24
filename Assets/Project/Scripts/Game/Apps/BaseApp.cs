using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.Events;

public abstract class BaseApp : MonoBehaviour, IApp
{
    [SerializeField] private List<GameObject> appObjects;
    [SerializeField] protected UnityEvent OnOpen;
    [SerializeField] protected UnityEvent OnClose;
    [SerializeField] public bool requireKeypad;

    public bool IsOpen { get; private set; }

    protected Mediator _mediator;
    protected AppController _appController;

    protected bool oppenable = true;

    protected virtual void Awake()
    {
        _mediator = Mediator.Instance;
        _mediator.GlobalEventBus.Subscribe<ServiceRegisterEvent>(Initialize);
    }

    protected virtual void Initialize(ServiceRegisterEvent @event)
    {
        if (@event.Service == typeof(AppController) as IService)
        {
            _appController = _mediator.GetService<AppController>();
            _appController.RegisterApp(this);

        }
    }
    public void Open()
    {
        OnOpen?.Invoke();
        SetObjects(true);
        IsOpen = true;

        if (oppenable)
        {
            _appController?.SelectApp(this as IApp);
            _appController?.OnAppOpen?.Invoke();
        }
    }

    public void Close()
    {
        OnClose?.Invoke();
        SetObjects(false);
        IsOpen = true;

        if (oppenable)
        {
            _appController?.DeselectApp(this);
            _appController?.OnAppClose?.Invoke();

        }
    }


    private void SetObjects(bool active)
    {
        foreach (var obj in appObjects)
        {
            obj.SetActive(active);
        }
    }
}
