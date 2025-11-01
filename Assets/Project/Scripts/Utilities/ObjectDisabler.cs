using UnityEngine;

public class ObjectDisabler : MonoBehaviour
{

    private int injectionProgress;
    private void Awake()
    {
        FindInjectables();
        TryDisable();
    }

    private void FindInjectables()
    {
        var list = TransformUtils.SearchForComponents<InjectableBehaviour>(transform);
        foreach (var item in list)
        {
            if (item.Injected) continue;
            item.OnInjected += CollectInjection;
            injectionProgress++;
        }

    }

    private void CollectInjection()
    {
        injectionProgress--;
        TryDisable();
    }

    private void TryDisable()
    {
        if (injectionProgress == 0)
        {
            gameObject.SetActive(false);
        }

    }
}
