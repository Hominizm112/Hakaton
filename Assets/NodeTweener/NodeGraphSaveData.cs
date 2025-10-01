using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

[Serializable]
public class NodeGraphSaveData
{
    public List<NodeSaveData> nodes = new();
    public List<ConnectionSaveData> connections = new();
    public Vector2 canvasOffset;
}

[Serializable]
public class NodeSaveData
{
    public string nodeType;
    public string nodeName;
    public Vector2 position;
    public string guid;

    public Vector3 targetPosition;
    public Vector3 targetScale;
    public float duration;
    public float waitTime;
    public Ease easeType;
    public string branchName;

    // New generic field storage
    public Dictionary<string, string> customFields;
}

[Serializable]
public class ConnectionSaveData
{
    public string fromNodeGuid;
    public string toNodeGuid;
}

[Serializable]
public class EditorNodeData
{
    public string nodeType;
    public string guid;
    public Vector2 position;
    public NodeType visualNodeType;
    public string nodeAssetGuid;
}

[Serializable]
public class ConnectionData
{
    public string fromNodeGuid;
    public string toNodeGuid;
}