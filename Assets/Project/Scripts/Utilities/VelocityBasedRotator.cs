using UnityEngine;


public class VelocityBasedRotator : MonoBehaviour
{
    [Header("Tilt Settings")]
    [SerializeField] private float _maxTiltAngle = 25f;
    [SerializeField] private float _tiltStrength = 2f;
    [SerializeField] private float _returnSpeed = 8f;
    [SerializeField] private float _smoothTime = 0.15f;

    private float _originalZRotation;
    private float _targetZRotation;
    private float _currentZVelocity;
    private bool _isDragging;

    private void Awake()
    {
        _originalZRotation = transform.eulerAngles.z;
        _targetZRotation = _originalZRotation;
    }

    public void OnDragStart()
    {
        _isDragging = true;
        _originalZRotation = transform.eulerAngles.z;
    }

    public void OnDragContinue(Vector2 dragVelocity)
    {
        if (!_isDragging) return;

        float velocityX = dragVelocity.x;
        float targetTilt = -velocityX * _tiltStrength;

        _targetZRotation = _originalZRotation + Mathf.Clamp(targetTilt, -_maxTiltAngle, _maxTiltAngle);

        float currentZ = Mathf.SmoothDampAngle(
            transform.eulerAngles.z,
            _targetZRotation,
            ref _currentZVelocity,
            _smoothTime
        );

        transform.rotation = Quaternion.Euler(0, 0, currentZ);
    }

    public void OnDragEnd()
    {
        _isDragging = false;
        _targetZRotation = _originalZRotation;
    }

    private void Update()
    {
        if (!_isDragging && Mathf.Abs(transform.eulerAngles.z - _originalZRotation) > 0.1f)
        {
            float currentZ = Mathf.SmoothDampAngle(
                transform.eulerAngles.z,
                _originalZRotation,
                ref _currentZVelocity,
                1f / _returnSpeed
            );

            transform.rotation = Quaternion.Euler(0, 0, currentZ);
        }
    }
}
