using System.Collections.Generic;
using UnityEngine;

public class BaseGraphRunnerService : MonoService, IGraphRunnerService
{
    [Header("Settings")]
    [SerializeField] private bool registerToMediator;
    

    protected Dictionary<string, TweenGraphRunner> _registeredRunners = new();


    private void Awake()
    {
        if (registerToMediator)
        {
            TryRegisterAsService();
        }
    }

    private void TryRegisterAsService()
    {
        if (Mediator.Instance == null) return;
        Mediator.Instance.RegisterService(this);
    }

    private void OnDestroy()
    {
        if (registerToMediator)
        {
            if (Mediator.Instance == null) return;
            Mediator.Instance.UnregisterService(this);

        }
    }

    public void RegisterAnimator(string runnerName, TweenGraphRunner animator)
    {
        if (!_registeredRunners.ContainsKey(runnerName))
        {
            _registeredRunners[runnerName] = animator;
        }
        else
        {
            ColorfulDebug.LogError($"Runner with name: {runnerName} already exists in the dictionary.");
        }

    }

    public void UnregisterAnimator(string runnerName)
    {
        if (TryGetRunner(runnerName, out _))
        {
            _registeredRunners.Remove(runnerName);
        }
    }

    public TweenGraphRunner GetRunner(string runnerName)
    {
        _registeredRunners.TryGetValue(runnerName, out TweenGraphRunner runner);
        if (runner == null)
        {
            ColorfulDebug.LogError($"Runner with name: {runnerName} not found in Dictionary");
        }
        return runner;
    }

    public bool TryGetRunner(string runnerName, out TweenGraphRunner runner)
    {
        runner = GetRunner(runnerName);
        return runner != null;
    }
}
