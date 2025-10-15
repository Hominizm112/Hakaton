using UnityEngine;

public class DragVelocityCalculator
{
    private Vector2 _previousPosition;
    private Vector2 _currentPosition;
    private float _lastUpdateTime;
    private bool _isRecording;

    private readonly System.Collections.Generic.Queue<MovementSample> _movementSamples = new();
    private const int SAMPLE_BUFFER_SIZE = 5;
    private const float SAMPLE_MAX_AGE = 0.2f;

    private struct MovementSample
    {
        public Vector2 Position;
        public float Time;
        public Vector2 Velocity;
    }

    public void StartRecording(Vector2 startPosition)
    {
        _previousPosition = startPosition;
        _currentPosition = startPosition;
        _lastUpdateTime = Time.time;
        _isRecording = true;
        _movementSamples.Clear();
    }

    public void StopRecording()
    {
        _isRecording = false;
        _movementSamples.Clear();
    }

    public void UpdatePosition(Vector2 newPosition)
    {
        if (!_isRecording) return;

        _previousPosition = _currentPosition;
        _currentPosition = newPosition;

        float currentTime = Time.time;
        float deltaTime = currentTime - _lastUpdateTime;

        // Calculate instant velocity
        Vector2 instantVelocity = deltaTime > 0 ? (_currentPosition - _previousPosition) / deltaTime : Vector2.zero;

        // Add to sample buffer
        _movementSamples.Enqueue(new MovementSample
        {
            Position = newPosition,
            Time = currentTime,
            Velocity = instantVelocity
        });

        // Remove old samples
        while (_movementSamples.Count > 0 &&
               (currentTime - _movementSamples.Peek().Time > SAMPLE_MAX_AGE ||
                _movementSamples.Count > SAMPLE_BUFFER_SIZE))
        {
            _movementSamples.Dequeue();
        }

        _lastUpdateTime = currentTime;
    }

    /// <summary>
    /// Gets the current smoothed velocity based on recent movement samples
    /// </summary>
    public Vector2 GetSmoothedVelocity()
    {
        if (_movementSamples.Count == 0) return Vector2.zero;

        Vector2 totalVelocity = Vector2.zero;
        int count = 0;

        foreach (var sample in _movementSamples)
        {
            totalVelocity += sample.Velocity;
            count++;
        }

        return count > 0 ? totalVelocity / count : Vector2.zero;
    }

    /// <summary>
    /// Gets the instant velocity (current frame only)
    /// </summary>
    public Vector2 GetInstantVelocity()
    {
        if (_movementSamples.Count == 0) return Vector2.zero;

        // Get the most recent sample
        var recentSamples = _movementSamples.ToArray();
        return recentSamples[recentSamples.Length - 1].Velocity;
    }

    /// <summary>
    /// Gets the current speed (magnitude of velocity)
    /// </summary>
    public float GetSpeed()
    {
        return GetSmoothedVelocity().magnitude;
    }

    /// <summary>
    /// Gets the total distance dragged from start position
    /// </summary>
    public float GetTotalDragDistance(Vector2 startPosition)
    {
        return Vector2.Distance(startPosition, _currentPosition);
    }

    /// <summary>
    /// Gets the direction of drag (normalized)
    /// </summary>
    public Vector2 GetDragDirection()
    {
        Vector2 velocity = GetSmoothedVelocity();
        return velocity.magnitude > 0.01f ? velocity.normalized : Vector2.zero;
    }

    public bool IsRecording => _isRecording;
}
