using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.Events;
using Zenject;

public abstract class BaseApp : MonoBehaviour
{
    /*
    [SerializeField] private List<GameObject> appObjects;
    [SerializeField] protected UnityEvent OnOpen;
    [SerializeField] protected UnityEvent OnClose;
    [SerializeField] public bool requireKeypad;
    [SerializeField] public bool requireAppLoad = true;

    public bool IsOpen { get; private set; }

    [Inject] protected Mediator _mediator;
    [Inject] protected AppController _appController;

    protected bool oppenable = true;

    [Inject]
    public void Construct()
    {
        _appController.RegisterApp(this);
    }

    public void Open()
    {
        OnOpen?.Invoke();
        SetObjects(true);
        IsOpen = true;

        if (oppenable)
        {
            _appController?.SelectApp(this);
            _appController?.OnAppOpen?.Invoke();
        }

        HandleAppOpen();

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

        HandleAppClose();

    }


    private void SetObjects(bool active)
    {
        foreach (var obj in appObjects)
        {
            obj.SetActive(active);
        }
    }

    protected virtual void HandleAppOpen() { }
    protected virtual void HandleAppClose() { }
    */
}
