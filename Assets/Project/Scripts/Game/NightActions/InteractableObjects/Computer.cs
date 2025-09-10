using UnityEngine;
using DG.Tweening;

public class Computer : InteractionObject
{
    [SerializeField] private AnimationSettings _animationSettings;
    private Mediator _mediator;
    private void Awake()
    {
        _mediator = Mediator.Instance;
    }
    protected override void HandleInteract()
    {
        _mediator.SetState(Game.State.Paused);
    }
}

