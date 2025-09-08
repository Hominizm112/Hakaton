using Unity.Mathematics;
using UnityEngine;

public class ScreenBounds
{
    private static Camera _mainCamera;
    private static Bounds _screenBounds;

    public static void Initialize(Camera camera = null)
    {
        _mainCamera = camera ?? Camera.main;
        CalculateScreenBounds();
    }

    public static void CalculateScreenBounds()
    {
        if (_mainCamera == null)
        {
            Debug.LogError("Main camera not found!");
            return;
        }

        Vector3 bottomLeft = _mainCamera.ScreenToWorldPoint(Vector3.zero);
        Vector3 topRight = _mainCamera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));

        _screenBounds = new Bounds();
        _screenBounds.SetMinMax(bottomLeft, topRight);
    }

    public static Vector3 ClampToScreen(Vector3 worldPosition, Vector2 objectSize)
    {
        if (_mainCamera == null) Initialize();

        Vector2 halfSize = objectSize * 0.5f;

        float clampedX = Mathf.Clamp(worldPosition.x, _screenBounds.min.x + halfSize.x, _screenBounds.max.x - halfSize.x);

        float clampedY = Mathf.Clamp(worldPosition.y, _screenBounds.min.y + halfSize.y, _screenBounds.max.y - halfSize.y);

        return new Vector3(clampedX, clampedY, worldPosition.z);
    }

    public static bool IsWithinScreen(Vector3 worldPosition, Vector2 objectsize)
    {
        if (_mainCamera == null) Initialize();

        Vector2 halfSize = objectsize * 0.5f;

        return worldPosition.x >= _screenBounds.min.x + halfSize.x &&
                       worldPosition.x <= _screenBounds.max.x - halfSize.x &&
                       worldPosition.y >= _screenBounds.min.y + halfSize.y &&
                       worldPosition.y <= _screenBounds.max.y - halfSize.y;
    }

    public static Bounds GetScreenBounds() => _screenBounds;

    public static void DrawDebugBounds(Color color, float duration = 1f)
    {
        Debug.DrawLine(
            new Vector3(_screenBounds.min.x, _screenBounds.min.y, 0),
            new Vector3(_screenBounds.max.x, _screenBounds.min.y, 0),
            color, duration
        );

        Debug.DrawLine(
            new Vector3(_screenBounds.max.x, _screenBounds.min.y, 0),
            new Vector3(_screenBounds.max.x, _screenBounds.max.y, 0),
            color, duration
        );

        Debug.DrawLine(
            new Vector3(_screenBounds.max.x, _screenBounds.max.y, 0),
            new Vector3(_screenBounds.min.x, _screenBounds.max.y, 0),
            color, duration
        );

        Debug.DrawLine(
            new Vector3(_screenBounds.min.x, _screenBounds.max.y, 0),
            new Vector3(_screenBounds.min.x, _screenBounds.min.y, 0),
            color, duration
        );
    }
}
