using System;


[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class NodeTypeAttribute : Attribute
{
    public string MenuName { get; }
    public NodeType Type { get; }
    public float DefaultHeight { get; }

    public NodeTypeAttribute(string menuName, NodeType type, float defaultHeight = 100f)
    {
        MenuName = menuName;
        Type = type;
        DefaultHeight = defaultHeight;
    }
}
