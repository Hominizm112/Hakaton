using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TeaComposer : MonoService
{
    [SerializeField] private UnityEvent OnTeaSet;

    private TeaCommodity _selectedTea;
    private List<TeaFlavorTag> _selectedFlavors = new();

    private void Awake()
    {
        Mediator.Instance.RegisterService(this);
    }


    public override void Initialize(Mediator mediator)
    {
        base.Initialize();
    }

    public void SetTea(TeaCommodity tea)
    {
        if (tea == null) return;

        _selectedTea = tea;

        OnTeaSet?.Invoke();
    }

    public void UnsetTea()
    {
        _selectedTea = null;
        _selectedFlavors.Clear();
    }

}
