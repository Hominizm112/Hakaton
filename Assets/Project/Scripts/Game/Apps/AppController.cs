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
    [SerializeField] public UnityEvent OnAppOpen;
    [SerializeField] public UnityEvent OnAppClose;
    [SerializeField] private BaseApp KeypadApp;

    [SerializeReference] private List<IApp> apps = new();
    private IApp _activeApp;
    private bool _keypadOpen => KeypadApp.IsOpen;
    public KeypadApp KeypadAppInstance => KeypadApp as KeypadApp;

    private void Awake()
    {
        Mediator.Instance.RegisterService(this);
        print(Mediator.Instance.GetService<AppController>());
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

    public T GetApp<T>() where T : IApp
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
        if ((_activeApp is BaseApp baseApp) && baseApp.requireKeypad)
        {
            KeypadApp.Open();
        }
    }

    public void DeselectApp(IApp app)
    {
        if (_activeApp != app) return;
        _activeApp = null;

        if (_keypadOpen)
        {
            KeypadApp.Close();
        }
    }

    public void OnDestroy()
    {
        apps.Clear();
        Mediator.Instance?.UnregisterService(this);
    }
}
