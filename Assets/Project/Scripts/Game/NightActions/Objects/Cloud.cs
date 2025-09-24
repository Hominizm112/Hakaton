using UnityEngine;
using DG.Tweening;

public class Cloud : MonoBehaviour, IInteractable, IStateListener
{
    [SerializeField] private CloudAnimationsSettings _cloudAnimationSettings;
    private Tween _movementTweener;
    private bool _isMoving = false;
    private bool _isSubscribed = false;
    private Mediator _mediator;

    public void Start()
    {
        InitializeCloud();
        SubscribeToGameState();
        CheckCurrentState();
    }
    private void SubscribeToGameState()
    {
        if (!_isSubscribed)
        {
            Game.GamesStateChanged += OnStateChanged;
            _isSubscribed = true;
        }
    }

     private void CheckCurrentState()
    {
        if (Game.ActualState == Game.State.NightScene)
        {
            StartCloudMovement();
        }
    }
    private void UnsubscribeFromGameState()
    {
        if (_isSubscribed)
        {
            Game.GamesStateChanged -= OnStateChanged;
            _isSubscribed = false;
        }
    }

    private void InitializeCloud()
    {
        if (_cloudAnimationSettings == null)
        {
            _mediator.GlobalEventBus.Publish<DebugLogErrorEvent>(new ("CloudAnimationsSettings not assigned!"));
            return;
        }
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
        _isMoving = true;
        _movementTweener = transform.DOMoveX(_cloudAnimationSettings.xFinal, _cloudAnimationSettings.movementDuration)
                                    .SetEase(Ease.Linear)
                                    .SetLoops(-1, LoopType.Yoyo)
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
    {
        UnsubscribeFromGameState();
        StopCloudMovement();
    }

}
