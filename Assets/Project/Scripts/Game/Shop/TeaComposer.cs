using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Rendering.LookDev;
using UnityEngine;
using UnityEngine.Events;

public class TeaComposer : MonoService
{
    [SerializeField] private UnityEvent OnTeaSet;
    [SerializeField] private GameObject teaFlavorPrefab;
    [SerializeField] private Transform teaFlavorScrollView;
    [SerializeField] private List<FlavorComponent> flavorComponents;

    private TeaCommodity _selectedTea;
    private List<TeaFlavorTag> _selectedFlavors = new();
    private List<TeaFlavorView> _teaFlavorViews = new();

    private void Awake()
    {
        Mediator.Instance.RegisterService(this);
    }


    public override void Initialize(Mediator mediator)
    {
        base.Initialize();

        foreach (TeaFlavorTag flavor in Enum.GetValues(typeof(TeaFlavorTag)))
        {
            var view = Instantiate(teaFlavorPrefab, Vector3.zero, Quaternion.identity, teaFlavorScrollView).GetComponent<TeaFlavorView>();
            _teaFlavorViews.Add(view);
            view.OnButtonClick += (r) => AddFlavor(r.TeaFlavor);
            view.SetFlavor(flavor);
        }

        foreach (var flavorComponent in flavorComponents)
        {
            flavorComponent.OnUnset += RemoveFlavor;
        }

    }

    public void SetTea(TeaCommodity tea)
    {
        if (tea == null) return;

        if (_selectedTea != null)
        {
            UnsetTea();
        }

        _selectedTea = tea;

        OnTeaSet?.Invoke();
    }

    public void UnsetTea()
    {
        _selectedTea = null;
        foreach (var flavorComp in flavorComponents)
        {
            flavorComp.UnsetFlavor();
        }
        _selectedFlavors.Clear();



    }

    private void AddFlavor(TeaFlavorTag flavor)
    {
        if (_selectedFlavors.Contains(flavor))
        {
            return;
        }
        var freeComponent = flavorComponents.Find(r => !r.IsFlavorSet);
        if (freeComponent == null) return;

        _selectedFlavors.Add(flavor);
        freeComponent.SetFlavor(flavor);

        Debug.Log($"Added flavor: {flavor}");
    }

    private void RemoveFlavor(TeaFlavorTag flavor)
    {
        if (_selectedFlavors.Contains(flavor))
        {
            _selectedFlavors.Remove(flavor);
            flavorComponents.Find(r => r.TeaFlavorTag == flavor).UnsetFlavor();

            Debug.Log($"Removed flavor: {flavor}");
            return;
        }

    }

    public void SellTea()
    {
        if (_selectedTea == null || !Mediator.Instance.GetService<NPCService>().NpcReadyToBuy)
        {
            return;
        }
        TeaCommodity teaComposed = Instantiate(_selectedTea);
        foreach (var flavor in _selectedFlavors)
        {
            teaComposed.flavorTags.Add(flavor);
        }

        Mediator.Instance.GetService<NPCService>().BuyTea(teaComposed);
    }

}

public struct TeaCompositeForSale
{
    public TeaCommodity tea;
    public List<TeaFlavorTag> flavors;

    public TeaCompositeForSale(TeaCommodity teaCommodity)
    {
        tea = teaCommodity;
        flavors = new();
    }
}
