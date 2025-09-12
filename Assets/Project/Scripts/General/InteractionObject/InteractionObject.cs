using System;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider2D))]
public abstract class InteractionObject : MonoBehaviour, IInteractionObject
{
    public Action<InteractionObject> OnInteract;
    [SerializeField] private UnityEvent OnInteractEvent;
    public virtual void Interact()
    {
        OnInteract?.Invoke(this);
        OnInteractEvent?.Invoke();
        HandleInteract();
    }
    protected abstract void HandleInteract();

    public void OnDestroy()
    {
        OnInteract = null;
    }
}
