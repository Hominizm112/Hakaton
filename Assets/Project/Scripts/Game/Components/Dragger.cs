using System;
using System.Collections;
using System.ComponentModel;
using UnityEditor.Localization.Plugins.XLIFF.V12;
using UnityEngine;
using Zenject;

public class Dragger : MonoComponent
{
    private enum DragMode
    {
        Simple,
        WithRotation
    }

    [Header("Settings")]
    [SerializeField] private DragMode dragMode;

    [Inject] private InputManager _inputService;

    private bool _dragging;
    public bool IsDragging => _dragging;

    private Coroutine _moveRoutine;
    private VelocityBasedRotator _velocityBasedRotator;
    private DragVelocityCalculator _velocityCalculator;

    private void Awake()
    {
        SubscribeToEvent<InputActionEvent>(HandleInput);
        if (dragMode == DragMode.WithRotation)
        {
            _velocityBasedRotator = GetComponent<VelocityBasedRotator>();
            if (_velocityBasedRotator == null)
            {
                _velocityBasedRotator = gameObject.AddComponent<VelocityBasedRotator>();
            }
            _velocityCalculator = new();
        }
    }

    private void HandleInput(InputActionEvent @event)
    {
        switch (@event.ActionName)
        {
            case "PointerClick" when @event.Context.started:
                StartDrag();
                break;

            case "PointerClick" when @event.Context.canceled:
                EndDrag();
                break;
        }
    }

    private void StartDrag()
    {
        if (CheckDrag())
        {
            _dragging = true;
            _moveRoutine = StartCoroutine(Move());
            _eventBus.Publish<DragStartedEvent>(new(this));
            if (dragMode == DragMode.WithRotation)
            {
                _velocityBasedRotator.OnDragStart();
                _velocityCalculator.StartRecording(_inputService.GetVector2("Point"));
            }
        }
    }

    private IEnumerator Move()
    {
        while (_dragging)
        {
            var currentPosition = Camera.main.ScreenToWorldPoint(_inputService.GetVector2("Point"));
            currentPosition.z = transform.position.z;
            transform.position = currentPosition;
            if (dragMode == DragMode.WithRotation)
            {
                _velocityCalculator.UpdatePosition(currentPosition);
                _velocityBasedRotator.OnDragContinue(_velocityCalculator.GetSmoothedVelocity());
            }
            yield return null;
        }
    }

    private void EndDrag()
    {
        _eventBus?.Publish<DragEndedEvent>(new(this));
        _dragging = false;

        if (_moveRoutine != null)
        {
            StopCoroutine(_moveRoutine);
            _moveRoutine = null;
        }

        if (dragMode == DragMode.WithRotation)
        {
            _velocityBasedRotator.OnDragEnd();
            _velocityCalculator.StopRecording();
        }

    }

    private bool CheckDrag()
    {
        if (gameObject == null) return false;
        return InputManager.GetObjectUnderMouse() == gameObject;
    }

    public override void Dispose()
    {
        EndDrag();
    }

}
