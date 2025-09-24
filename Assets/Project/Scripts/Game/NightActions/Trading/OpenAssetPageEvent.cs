using UnityEngine;
public class OpenAssetPageEvent : IEvent
{
    public object Asset { get; }

    public OpenAssetPageEvent(object asset)
    {
        Asset = asset;
    }
}