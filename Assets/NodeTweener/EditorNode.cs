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
    public NodeType nodeType;
    public string guid;

    public ConnectionPoint inPoint;
    public ConnectionPoint outPoint;

    public Action<EditorNode> OnRemoveNode;
    public Action<EditorNode> OnSelectNode;
    public Action<ConnectionPoint> OnClickConnectionPoint;


    private GUIStyle style;
    private GUIStyle defaultNodeStyle;
    private GUIStyle selectedNodeStyle;

    public EditorNode(Vector2 position, float width, float height, NodeType type, GUIStyle nodeStyle, GUIStyle selectedStyle
        , GUIStyle inPointStyle, GUIStyle outPointStyle, Action<ConnectionPoint> OnClickInPoint, Action<ConnectionPoint> OnClickOutPoint
        , Action<EditorNode> OnClickRemoveNode, Action<EditorNode> OnNodeSelected, System.Type nodeType, TweenNode nodeAsset)
    {
        rect = new Rect(position.x, position.y, width, height);
        this.nodeType = type;
        this.guid = System.Guid.NewGuid().ToString();
        style = nodeStyle;
        defaultNodeStyle = nodeStyle;
        selectedNodeStyle = selectedStyle;

        inPoint = new ConnectionPoint(this, ConnectionPointType.In, inPointStyle, OnClickInPoint);
        outPoint = new ConnectionPoint(this, ConnectionPointType.Out, outPointStyle, OnClickOutPoint);

        OnRemoveNode = OnClickRemoveNode;
        OnSelectNode = OnNodeSelected;

        node = ScriptableObject.CreateInstance(nodeType.Name) as TweenNode;
        node.nodeRect = rect;
        node.name = nodeType.Name;
        this.node = nodeAsset;
    }

    public void Drag(Vector2 delta)
    {
        rect.position += delta;
        node.nodeRect = rect;

        inPoint.rect.position += delta;
        outPoint.rect.position += delta;
    }

    public void Draw()
    {

        inPoint.Draw();
        outPoint.Draw();

        Color originalBgColor = GUI.backgroundColor;

        if (isSelected)
        {
            GUI.backgroundColor = HexColorUtility.ParseHex("#534b52"); // Selected color
        }
        else
        {
            GUI.backgroundColor = HexColorUtility.ParseHex("#474448"); // Normal color
        }

        GUIStyle style = isSelected ? selectedNodeStyle : defaultNodeStyle;
        GUI.Box(rect, node.name, style);

        // Restore background color
        GUI.backgroundColor = originalBgColor;

        // Draw node content
        DrawNodeContent();

    }

    private void DrawNodeContent()
    {
        Color originalColor = GUI.color;
        Color originalContentColor = GUI.contentColor;

        Color textColor = NodeBasedTweenEditor.GetContrastColor(isSelected ? HexColorUtility.ParseHex("#534b52") : HexColorUtility.ParseHex("#474448"));
        GUI.color = textColor;
        GUI.contentColor = textColor;

        if (node is MoveNode moveNode)
        {
            moveNode.targetPosition = EditorGUI.Vector3Field(new Rect(rect.x + 5, rect.y + 20, rect.width - 10, 40), "Position", moveNode.targetPosition);
            moveNode.duration = EditorGUI.FloatField(new Rect(rect.x + 5, rect.y + 65, rect.width - 10, 15), "Duration", moveNode.duration);
        }
        else if (node is ScaleNode scaleNode)
        {
            scaleNode.targetScale = EditorGUI.Vector3Field(new Rect(rect.x + 5, rect.y + 20, rect.width - 10, 40), "Scale", scaleNode.targetScale);
            scaleNode.duration = EditorGUI.FloatField(new Rect(rect.x + 5, rect.y + 65, rect.width - 10, 15), "Duration", scaleNode.duration);
        }
        else if (node is WaitNode waitNode)
        {
            waitNode.waitTime = EditorGUI.FloatField(new Rect(rect.x + 5, rect.y + 20, rect.width - 10, 15), "Wait Time", waitNode.waitTime);
        }

        GUI.color = originalColor;
        GUI.contentColor = originalContentColor;
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
