using UnityEngine;

public class HiddenContainer
{
    private GameObject _container;
    public Transform Container => _container.transform;


    public HiddenContainer()
    {
        GameObject newContainer = new GameObject("HiddenContainer");
        newContainer.SetActive(false);

        _container = newContainer;
    }

    public void Release(params Transform[] transforms)
    {
        foreach (var transform in transforms)
        {
            transform.localScale = Vector3.one;
        }
        Object.Destroy(_container);
    }

}