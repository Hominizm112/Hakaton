using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;


[Serializable]
public abstract class TweenNode : ScriptableObject
{

    [SerializeField, HideInInspector]
    private bool _isInitialized = false;

    protected virtual void OnEnable()
    {
        if (!_isInitialized)
        {
            Initialize();
            _isInitialized = true;
        }
    }


    [SerializeField] public Rect nodeRect;
    [SerializeField] public string nodeName = "Node";
    [SerializeField] public bool isDragged;
    [SerializeField] public bool isSelected;

    [SerializeField, HideInInspector]
    public string guid;

    [SerializeField] public virtual float nodeHeight => NodeRegistry.GetDefaultHeight(GetType());
    [SerializeField] public virtual float nodeWidth => NodeRegistry.GetDefaultWidth(GetType());

    [SerializeField] public List<TweenNode> inputs = new();
    [SerializeField] public List<TweenNode> outputs = new();

    public abstract DG.Tweening.Tweener Execute(GameObject target);
    public virtual void DrawNode() { }
    public virtual bool ProcessEvents(Event e) { return false; }
    protected virtual void Initialize() { }

}
