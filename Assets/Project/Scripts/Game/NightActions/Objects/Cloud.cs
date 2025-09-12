using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine.Rendering;

public class Cloud : MonoBehaviour, IInteractable, IStateListener
{
    [SerializeField] private CloudAnimationsSettings _cloudAnimationSettings;
    private Tween _movementTweener;
    private bool _isMoving = false;
    private bool _isSubscribed = false;
    

    public void Awake()
    {
        InitializeCloud();
    }

    public void Start()
    {
        if (!_isSubscribed)
        {
            Mediator.Instance?.SubscribeToState(this, Game.State.NightScene);
            _isSubscribed = true;
        }
    }
    private void InitializeCloud()
    {
        transform.position = _cloudAnimationSettings.initial_point;
        _isMoving = false;
    }
    public void Interact()
    {
        ToggleMovement();
    }
    private void ToggleMovement()
    {
        if (_isMoving)
            StopCloudMovement();
        else
            StartCloudMovement();
    }
    private void StartCloudMovement()
    {
        if (_isMoving) return;

        _isMoving = true;
        _movementTweener = transform.DOMoveX(_cloudAnimationSettings.xFinal, _cloudAnimationSettings.movementDuration)
                                    .SetEase(Ease.Linear)
                                    .SetLoops(-1, LoopType.Restart)
                                    .OnStart(() => transform.position = _cloudAnimationSettings.initial_point);
    }
    public void StopCloudMovement()
    {
        _movementTweener?.Kill();
        _movementTweener = null;
        _isMoving = false;
    }
    public void OnStateChanged(Game.State newState)
    {
        if (newState == Game.State.NightScene)
        {
            StartCloudMovement();
        }
        else
        {
            StopCloudMovement();
            transform.position = _cloudAnimationSettings.initial_point;
        }
    }

    void OnDestroy()
    {  if (_isSubscribed)
        {
            Mediator.Instance?.UnsubscribeFromState(this, Game.State.NightScene);
        }
        StopCloudMovement();
    }

}
