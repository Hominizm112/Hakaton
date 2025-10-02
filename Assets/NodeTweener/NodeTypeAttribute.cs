using System;


[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class NodeTypeAttribute : Attribute
{
    public string MenuName { get; }
    public float DefaultHeight { get; }
    public float DefaultWidth { get; }

    public NodeTypeAttribute(string menuName, float defaultHeight = 100f, float defaultWidth = 200f)
    {
        MenuName = menuName;
        DefaultHeight = defaultHeight;
        DefaultWidth = defaultWidth;
    }
}
