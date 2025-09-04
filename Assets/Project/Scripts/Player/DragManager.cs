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
    [SerializeField] private LayerMask _draggableLayer = 1;
    [SerializeField] private float _zDepth = 10f;

    private Mediator _mediator;
    private InputManager _inputManager;
    private Camera _mainCamera;

    private GameObject _currentDraggedObject;
    private Vector2 _previousDragPosition;
    private bool _isDragging = false;

    public void Initialize(Mediator mediator)
    {
        _mediator = mediator;
        _inputManager = mediator.GetService<InputManager>();
        _mainCamera = Camera.main;

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
            case "Click" when inputEvent.Context.started:
                StartDrag();
                break;

            case "Click" when inputEvent.Context.canceled:
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
        var hit = Physics2D.Raycast(_mainCamera.ScreenToWorldPoint(mousePosition), Vector2.zero,
                                   Mathf.Infinity, _draggableLayer);

        if (hit.collider != null && hit.collider.TryGetComponent<IDraggable>(out var draggable))
        {
            if (draggable.CanBeDragged)
            {
                _currentDraggedObject = hit.collider.gameObject;
                _previousDragPosition = mousePosition;
                _isDragging = true;

                draggable.OnDragStart();

                _mediator.GlobalEventBus.Publish(new DragStartedEvent(
                    mousePosition,
                    _currentDraggedObject
                ));
            }
        }
    }

    private void ContinueDrag()
    {
        var currentPosition = _inputManager.GetVector2("Point");
        var delta = currentPosition - _previousDragPosition;

        if (_currentDraggedObject != null &&
            _currentDraggedObject.TryGetComponent<IDraggable>(out var draggable))
        {
            var worldPosition = _mainCamera.ScreenToWorldPoint(
                new Vector3(currentPosition.x, currentPosition.y, _zDepth)
            );

            draggable.OnDragContinue(worldPosition, delta);

            _mediator.GlobalEventBus.Publish(new DragContinuedEvent(
                currentPosition,
                delta,
                _currentDraggedObject
            ));
        }

        _previousDragPosition = currentPosition;
    }

    private void EndDrag()
    {
        if (!_isDragging) return;

        var endPosition = _inputManager.GetVector2("Point");

        if (_currentDraggedObject != null &&
            _currentDraggedObject.TryGetComponent<IDraggable>(out var draggable))
        {
            draggable.OnDragEnd();

            _mediator.GlobalEventBus.Publish(new DragEndedEvent(
                endPosition,
                _currentDraggedObject
            ));
        }

        _currentDraggedObject = null;
        _isDragging = false;
    }

    public bool IsDragging() => _isDragging;
    public GameObject GetCurrentDraggedObject() => _currentDraggedObject;

    private void OnDestroy()
    {
        _mediator?.GlobalEventBus.Unsubscribe<InputActionEvent>(OnInputAction);
    }
}
