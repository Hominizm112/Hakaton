using UnityEngine;
using UnityEditor;
using System;
using UnityEditor.MemoryProfiler;
using System.Collections.Generic;

public class EditorNode
{
    public Rect rect;
    public TweenNode node;
    public bool isDragged;
    public bool isSelected;
    public string guid;

    public ConnectionPoint inPoint;
    public ConnectionPoint outPoint;

    public Action<EditorNode> OnRemoveNode;
    public Action<EditorNode> OnSelectNode;
    public Action<ConnectionPoint> OnClickConnectionPoint;


    private GUIStyle style;
    private GUIStyle defaultNodeStyle;
    private GUIStyle selectedNodeStyle;

    private bool showInPoint = true;
    private bool showOutPoint = true;

    public EditorNode(Vector2 position, GUIStyle nodeStyle, GUIStyle selectedStyle
       , GUIStyle inPointStyle, GUIStyle outPointStyle, Action<ConnectionPoint> OnClickInPoint, Action<ConnectionPoint> OnClickOutPoint
       , Action<EditorNode> OnClickRemoveNode, Action<EditorNode> OnNodeSelected, TweenNode nodeAsset)
    {
        rect = new Rect(position.x, position.y, nodeAsset.nodeWidth, nodeAsset.nodeHeight);
        this.guid = System.Guid.NewGuid().ToString();
        style = nodeStyle;
        defaultNodeStyle = nodeStyle;
        selectedNodeStyle = selectedStyle;
        this.node = nodeAsset;

        showInPoint = !(nodeAsset is StartNode);
        showOutPoint = !(nodeAsset is EndNode);

        if (showInPoint)
        {
            inPoint = new ConnectionPoint(this, ConnectionPointType.In, inPointStyle, OnClickInPoint);
        }

        if (showOutPoint)
        {
            outPoint = new ConnectionPoint(this, ConnectionPointType.Out, outPointStyle, OnClickOutPoint);
        }

        OnRemoveNode = OnClickRemoveNode;
        OnSelectNode = OnNodeSelected;

        node.nodeRect = rect;
    }
    public void Drag(Vector2 delta)
    {
        rect.position += delta;
        node.nodeRect = rect;

        if (showInPoint)
        {
            inPoint.rect.position += delta;

        }

        if (showOutPoint)
        {
            outPoint.rect.position += delta;

        }
    }

    public void Draw()
    {
        if (node == null) return;

        if (showInPoint) inPoint?.Draw();
        if (showOutPoint) outPoint?.Draw();


        GUIStyle style = isSelected ? selectedNodeStyle : defaultNodeStyle;

        GUILayout.BeginArea(rect, style);
        {
            node.DrawNode();
        }
        GUILayout.EndArea();




    }

    public bool ProcessEvents(Event e)
    {
        switch (e.type)
        {
            case EventType.MouseDown:
                if (e.button == 0 || e.button == 1)
                {
                    if (rect.Contains(e.mousePosition))
                    {
                        isDragged = true;
                        GUI.changed = true;
                        isSelected = true;
                        style = selectedNodeStyle;
                        OnSelectNode?.Invoke(this);

                        if (e.button == 1)
                            ProcessContextMenu(e.mousePosition);

                    }
                    else
                    {
                        GUI.changed = true;
                        isSelected = false;
                        style = defaultNodeStyle;
                    }


                }
                break;

            case EventType.MouseUp:
                isDragged = false;
                break;

            case EventType.MouseDrag:
                if (e.button == 0 && isDragged)
                {
                    Drag(e.delta);
                    e.Use();
                    return true;
                }
                break;
        }

        return false;
    }

    private void ProcessContextMenu(Vector2 mousePosition)
    {
        GenericMenu genericMenu = new();
        genericMenu.AddItem(new GUIContent("Remove node"), false, OnClickRemoveNode);
        genericMenu.ShowAsContext();
    }

    public void RemoveNode()
    {
        OnClickRemoveNode();
    }

    private void OnClickRemoveNode()
    {
        if (OnRemoveNode != null)
        {
            OnRemoveNode?.Invoke(this);
        }
    }
}
