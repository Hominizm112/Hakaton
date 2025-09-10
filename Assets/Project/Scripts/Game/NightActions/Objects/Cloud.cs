using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;
using JetBrains.Annotations;
public class Cloud : MonoBehaviour, IInteractable,IGameStateListener
{
    [SerializeField] private float _movementDuration = 10f;
    [SerializeField] private float _x_final = 5f;
    [SerializeField] private Vector2 _initial_point = new Vector2(0, 5f);
    private Tween _movementTweener;
    private bool _isMoving = false;
    public void Awake()
    {
        InitializeCloud();
    }
    private void InitializeCloud()
    {
        transform.position = _initial_point;
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
        _movementTweener = transform.DOMoveX(_x_final, _movementDuration)
                                    .SetEase(Ease.Linear)
                                    .SetLoops(-1, LoopType.Restart)
                                    .OnStart(() => transform.position = _initial_point);
    }

    public void StopCloudMovement()
    {
        _movementTweener?.Kill();
        _movementTweener = null;
    }
    public void CheckGameMode(Game.State newState)
    {
        if (newState != Game.State.NightScene)
        {
            StopCloudMovement();
        }
        else if (newState == Game.State.NightScene)
        {
            StartCloudMovement();
        }
    }

    void OnDestroy()
    {
        StopCloudMovement();
    }
    
}
