using UnityEngine;
using UnityEditor;


[System.Serializable]
public class Connection
{
    public ConnectionPoint inPoint;
    public ConnectionPoint outPoint;


    public Connection(ConnectionPoint inPoint, ConnectionPoint outPoint)
    {
        this.inPoint = inPoint;
        this.outPoint = outPoint;
    }


    public Rect GetBounds()
    {
        if (inPoint == null || outPoint == null)
            return new Rect(0, 0, 0, 0);

        Vector2 start = inPoint.rect.center;
        Vector2 end = outPoint.rect.center;
        Vector2 startTangent = start + Vector2.left * 50f;
        Vector2 endTangent = end - Vector2.left * 50f;

        float minX = Mathf.Min(start.x, end.x, startTangent.x, endTangent.x);
        float minY = Mathf.Min(start.y, end.y, startTangent.y, endTangent.y);
        float maxX = Mathf.Max(start.x, end.x, startTangent.x, endTangent.x);
        float maxY = Mathf.Max(start.y, end.y, startTangent.y, endTangent.y);

        return new Rect(minX - 12f, minY - 12f, (maxX - minX) + 24f, (maxY - minY) + 24f);
    }


#if UNITY_EDITOR

    public void Draw(bool isHovered)
    {
        if (inPoint == null || outPoint == null) return;

        Color connectionColor = isHovered ? HexColorUtility.ParseHex("#fdcb6e") : HexColorUtility.ParseHex("#d0ddd7");
        float thickness = isHovered ? 5f : 3f;

        Handles.DrawBezier(
            inPoint.rect.center,
            outPoint.rect.center,
            inPoint.rect.center + Vector2.left * 50f,
            outPoint.rect.center - Vector2.left * 50f,
            connectionColor,
            null,
            thickness
        );

        if (isHovered)
        {
            Handles.DrawBezier(
                inPoint.rect.center,
                outPoint.rect.center,
                inPoint.rect.center + Vector2.left * 50f,
                outPoint.rect.center - Vector2.left * 50f,
                new Color(1, 1, 0, 0.3f),
                null,
                8f
            );
        }

    }

#endif

    private void ProcessEvents(Event e)
    {

    }

    public bool IsMouseOver(Vector2 mousePosition, float maxDistance = 10f)
    {
        if (inPoint == null || outPoint == null) return false;

        Rect bounds = GetBounds();
        if (!bounds.Contains(mousePosition))
            return false;

        Vector2 start = inPoint.rect.center;
        Vector2 end = outPoint.rect.center;
        Vector2 startTangent = start + Vector2.left * 50f;
        Vector2 endTangent = end - Vector2.left * 50f;

        return IsPointNearBezier(mousePosition, start, end, startTangent, endTangent, maxDistance);
    }

    private bool IsPointNearBoundingBox(Vector2 point, Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3, float maxDistance)
    {
        int segments = 6;
        float minDistance = float.MaxValue;

        Vector2 previousPoint = p0;

        for (int i = 1; i <= segments; i++)
        {
            float t = i / (float)segments;
            Vector2 currentPoint = CalculateBezierPoint(t, p0, p1, p2, p3);

            float distance = DistanceToLineSegment(point, previousPoint, currentPoint);
            if (distance < minDistance)
            {
                minDistance = distance;
                if (minDistance <= maxDistance)
                    return true;
            }

            previousPoint = currentPoint;
        }

        return minDistance <= maxDistance;
    }

    private bool IsPointNearBezier(Vector2 point, Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3, float maxDistance)
    {
        int segments = 6;
        float minDistance = float.MaxValue;

        Vector2 previousPoint = p0;

        for (int i = 1; i <= segments; i++)
        {
            float t = i / (float)segments;
            Vector2 currentPoint = CalculateBezierPoint(t, p0, p1, p2, p3);

            float distance = DistanceToLineSegment(point, previousPoint, currentPoint);
            if (distance < minDistance)
            {
                minDistance = distance;
                if (minDistance <= maxDistance)
                    return true;
            }

            previousPoint = currentPoint;
        }

        return minDistance <= maxDistance;
    }

    public Vector2 CalculateBezierPoint(float t, Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;
        float uuu = uu * u;
        float ttt = tt * t;

        Vector2 p = uuu * p0;
        p += 3 * uu * t * p1;
        p += 3 * u * tt * p2;
        p += ttt * p3;

        return p;
    }

    public float DistanceToLineSegment(Vector2 point, Vector2 lineStart, Vector2 lineEnd)
    {
        Vector2 line = lineEnd - lineStart;
        float lineLength = line.magnitude;

        if (lineLength < 0.001f)
            return Vector2.Distance(point, lineStart);

        Vector2 lineNormalized = line / lineLength;
        Vector2 pointToStart = point - lineStart;

        float dot = Vector2.Dot(pointToStart, lineNormalized);

        if (dot <= 0)
            return Vector2.Distance(point, lineStart);
        else if (dot >= lineLength)
            return Vector2.Distance(point, lineEnd);
        else
        {
            Vector2 closestPoint = lineStart + lineNormalized * dot;
            return Vector2.Distance(point, closestPoint);
        }
    }

}
