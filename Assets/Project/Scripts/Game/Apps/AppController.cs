using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public interface IApp
{
    public void Open();
    public void Close();
}
public class AppController : MonoService
{
    [SerializeField] private List<IApp> apps = new();
    [SerializeField] public UnityEvent OnAppOpen;
    [SerializeField] public UnityEvent OnAppClose;

    private IApp _activeApp;

    private void Awake()
    {
        Mediator.Instance.RegisterService(this);
    }

    public void RegisterApp(IApp app)
    {
        if (CheckApp(app)) return;
        apps.Add(app);
    }

    public bool CheckApp(IApp app)
    {
        return apps.Contains(app);
    }

    public IApp GetApp<T>() where T : IApp
    {
        return apps.OfType<T>().FirstOrDefault();
    }

    public void CloseCurrentApp()
    {
        _activeApp.Close();
        DeselectApp(_activeApp);
    }

    public void SelectApp(IApp app)
    {
        _activeApp = app;
    }

    public void DeselectApp(IApp app)
    {
        if (_activeApp != app) return;
        _activeApp = null;
    }
}
