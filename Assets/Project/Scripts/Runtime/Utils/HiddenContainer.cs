using System;
using UnityEngine;

public class HiddenContainer : IDisposable
{
    private GameObject _container;
    public Transform Container => _container.transform;

    private Transform _desiredParent = null;

    public HiddenContainer(Transform desiredParent = null)
    {
        GameObject newContainer = new GameObject("HiddenContainer");
        newContainer.SetActive(false);

        _container = newContainer;
        _desiredParent = desiredParent;
    }

    public void Release(params Transform[] transforms)
    {
        foreach (var transform in transforms)
        {
            transform.SetParent(_desiredParent);
            transform.gameObject.SetActive(false);
        }

    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(_container);
    }
}