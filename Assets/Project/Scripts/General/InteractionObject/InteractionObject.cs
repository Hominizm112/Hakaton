using UnityEngine;
using UnityEngine.InputSystem.Interactions;

public abstract class InteractionObject : MonoBehaviour, IInteractionObject
{
    public virtual void Interact()
    {
        HandleInteract();
    }
    protected abstract void HandleInteract();
}
