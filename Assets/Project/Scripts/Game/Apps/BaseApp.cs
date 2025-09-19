using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class BaseApp : MonoBehaviour, IApp
{
    [SerializeField] private List<GameObject> appObjects;
    [SerializeField] private UnityEvent OnOpen;
    [SerializeField] private UnityEvent OnClose;

    private Mediator _mediator;
    private AppController _appController;

    private void Awake()
    {
        _mediator = Mediator.Instance;
        _mediator.GlobalEventBus.Subscribe<ServiceRegisterEvent>(Initialize);

    }

    private void Initialize(ServiceRegisterEvent @event)
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
        _appController.SelectApp(this);
        _appController.OnAppOpen?.Invoke();
    }

    public void Close()
    {
        OnClose?.Invoke();
        SetObjects(false);
        _appController.DeselectApp(this);
        _appController.OnAppClose?.Invoke();
    }

    private void SetObjects(bool active)
    {
        foreach (var obj in appObjects)
        {
            obj.SetActive(active);
        }
    }
}
