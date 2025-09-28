using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public enum NodeType
{
    None,
    Move,
    Scale,
    Wait
}
public abstract class TweenNode : ScriptableObject
{
    public Rect nodeRect;
    public string nodeName = "Node";
    public bool isDragged;
    public bool isSelected;

    public List<TweenNode> inputs = new();
    public List<TweenNode> outputs = new();

    public abstract DG.Tweening.Tweener Execute(GameObject target);
    public virtual void DrawNode() { }

    public virtual bool ProcessEvents(Event e) { return false; }
}
