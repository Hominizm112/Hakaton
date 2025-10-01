using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public enum NodeType
{
    None,
    Move,
    Scale,
    Wait,
    Start,
    End,
    Branch,
    Rotate
}
public abstract class TweenNode : ScriptableObject
{
    public Rect nodeRect;
    public string nodeName = "Node";
    public bool isDragged;
    public bool isSelected;

    [HideInInspector]
    public string guid;

    public virtual float nodeHeight => NodeRegistry.GetDefaultHeight(GetType());

    public List<TweenNode> inputs = new();
    public List<TweenNode> outputs = new();

    public abstract DG.Tweening.Tweener Execute(GameObject target);
    public virtual void DrawNode() { }

    public virtual bool ProcessEvents(Event e) { return false; }
}
