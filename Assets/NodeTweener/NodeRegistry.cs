using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public static class NodeRegistry
{
    private static Dictionary<Type, NodeTypeAttribute> _nodeTypes;
    private static Dictionary<NodeType, Type> _typeByNodeType;

    static NodeRegistry()
    {
        DiscoverNodeTypes();
    }

    private static void DiscoverNodeTypes()
    {
        _nodeTypes = new();
        _typeByNodeType = new();

        var nodeTypes = Assembly.GetExecutingAssembly().GetTypes().Where(t => t.IsSubclassOf(typeof(TweenNode)) && !t.IsAbstract);

        foreach (var type in nodeTypes)
        {
            var attribute = type.GetCustomAttribute<NodeTypeAttribute>();
            if (attribute != null)
            {
                _nodeTypes[type] = attribute;
                _typeByNodeType[attribute.Type] = type;
            }
        }
    }

    public static IEnumerable<Type> GetAllNodeTypes()
    {
        return _nodeTypes.Keys;
    }

    public static string GetMenuName(Type nodeType)
    {
        return _nodeTypes.TryGetValue(nodeType, out var attribute) ? attribute.MenuName : nodeType.Name;
    }

    public static NodeType GetNodeType(Type nodeType)
    {
        return _nodeTypes.TryGetValue(nodeType, out var attribute) ? attribute.Type : NodeType.None;
    }

    public static float GetDefaultHeight(Type nodeType)
    {
        return _nodeTypes.TryGetValue(nodeType, out var attribute) ? attribute.DefaultHeight : 100f;
    }

    public static Type GetTypeFromNodeType(NodeType nodeType)
    {
        return _typeByNodeType.TryGetValue(nodeType, out var type) ? type : null;
    }

    public static NodeTypeAttribute GetAttribute(Type nodeType)
    {
        return _nodeTypes.TryGetValue(nodeType, out var attribute) ? attribute : null;
    }
}
