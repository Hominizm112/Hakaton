using UnityEngine;

public class ConnectionPair
{
    public ConnectionPoint start;
    public ConnectionPoint end;

    public ConnectionPair(ConnectionPoint start, ConnectionPoint end)
    {
        this.start = start;
        this.end = end;
    }

    public bool IsComplete()
    {
        return start != null && end != null;
    }
}
