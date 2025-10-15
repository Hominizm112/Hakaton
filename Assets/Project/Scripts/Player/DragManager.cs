using UnityEngine;

public interface IDraggable
{
    bool CanBeDragged { get; }
    void OnDragStart();
    void OnDragContinue(Vector2 worldPosition, Vector2 delta);
    void OnDragEnd();
}

public class DragManager : MonoBehaviour, IInitializable, IStateListener
{

    private Mediator _mediator;
    private InputManager _inputManager;
    private DragVelocityCalculator _velocityCalculator;

    private Vector2 _previousDragPosition;
    private Vector2 _dragStartPosition;
    private bool _isDragging = false;
    private bool _hasValidDrag = false;

    public void Initialize(Mediator mediator)
    {
        _mediator = mediator;
        _inputManager = mediator.GetService<InputManager>();
        _velocityCalculator = new();

        mediator.RegisterService<DragManager>(this);
        mediator.SubscribeToState(this, Game.State.Gameplay);

        mediator.GlobalEventBus.Subscribe<InputActionEvent>(OnInputAction);
    }

    public void OnStateChanged(Game.State state)
    {
        if (state != Game.State.Gameplay && _isDragging)
        {
            EndDrag();
        }
    }

    private void OnInputAction(InputActionEvent inputEvent)
    {
        if (!_mediator.IsCurrentState(Game.State.Gameplay)) return;
        switch (inputEvent.ActionName)
        {
            case "PointerClick" when inputEvent.Context.started:
                StartDrag();
                break;

            case "PointerClick" when inputEvent.Context.canceled:
                EndDrag();
                break;
        }
    }

    private void Update()
    {
        if (_isDragging)
        {
            ContinueDrag();
        }
    }

    private void StartDrag()
    {
        var mousePosition = _inputManager.GetVector2("Point");
        _dragStartPosition = mousePosition;
        _previousDragPosition = mousePosition;
        _isDragging = true;
        _hasValidDrag = false;

        _velocityCalculator.StartRecording(mousePosition);

        _mediator.GlobalEventBus.Publish(new DragStartedEvent(mousePosition));
    }

    private void ContinueDrag()
    {
        var currentPosition = _inputManager.GetVector2("Point");
        var delta = currentPosition - _previousDragPosition;

        _velocityCalculator.UpdatePosition(currentPosition);


        if (!_hasValidDrag)
        {
            var dragDistance = Vector2.Distance(currentPosition, _dragStartPosition);
            _hasValidDrag = dragDistance > 5f;
        }

        if (_hasValidDrag)
        {
            Vector2 currentVelocity = _velocityCalculator.GetSmoothedVelocity();
            Vector2 direction = _velocityCalculator.GetDragDirection();
            _mediator.GlobalEventBus.Publish(new DragContinuedEvent(currentPosition, delta, currentVelocity, direction));
        }

        _previousDragPosition = currentPosition;
    }

    private void EndDrag()
    {
        if (!_isDragging) return;

        var endPosition = _inputManager.GetVector2("Point");

        _mediator.GlobalEventBus.Publish(new DragEndedEvent(endPosition));


        _velocityCalculator.StopRecording();
        _isDragging = false;
        _hasValidDrag = false;
    }

    public bool IsDragging() => _isDragging;
    public bool HasValidDrag() => _hasValidDrag;
    public Vector2 GetDragStartPosition() => _dragStartPosition;
    public Vector2 GetCurrentDragPosition() => _inputManager.GetVector2("Point");
    public Vector2 GetDragDelta() => _inputManager.GetVector2("Point") - _previousDragPosition;
    public Vector2 GetCurrentVelocity => _velocityCalculator.GetSmoothedVelocity();

    private void OnDestroy()
    {
        _mediator?.GlobalEventBus?.Unsubscribe<InputActionEvent>(OnInputAction);
    }
}