using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Draggable2D : MonoBehaviour, IDraggable
{
    [SerializeField] private bool _isDraggable = true;
    [SerializeField] private bool _snapToCursor = true;
    [SerializeField] private float _dragSpeed = 1f;

    public bool CanBeDragged => _isDraggable;

    private Vector3 _offset;
    private Rigidbody2D _rb;
    private Collider2D _collider;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _collider = GetComponent<Collider2D>();
    }

    public void OnDragStart()
    {
        var mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        _offset = transform.position - mouseWorldPos;

        if (_rb != null)
        {
            _rb.bodyType = RigidbodyType2D.Kinematic;
        }

    }

    public void OnDragContinue(Vector2 worldPosition, Vector2 delta)
    {
        if (_snapToCursor)
        {
            transform.position = worldPosition + (Vector2)_offset;
        }
        else
        {
            var targetPosition = worldPosition + (Vector2)_offset;
            transform.position = Vector3.Lerp(transform.position, targetPosition, _dragSpeed * Time.deltaTime);
        }
    }

    public void OnDragEnd()
    {
        if (_rb != null)
        {
            _rb.bodyType = RigidbodyType2D.Static;
        }

        _offset = Vector3.zero;
    }

    public void SetDraggable(bool draggable)
    {
        _isDraggable = draggable;
    }
}
