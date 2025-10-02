using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public static class NodeRegistry
{
    private static Dictionary<Type, NodeTypeAttribute> _nodeTypes;

    static NodeRegistry()
    {
        DiscoverNodeTypes();
    }

    private static void DiscoverNodeTypes()
    {
        _nodeTypes = new();

        var nodeTypes = Assembly.GetExecutingAssembly().GetTypes().Where(t => t.IsSubclassOf(typeof(TweenNode)) && !t.IsAbstract);

        foreach (var type in nodeTypes)
        {
            var attribute = type.GetCustomAttribute<NodeTypeAttribute>();
            if (attribute != null)
            {
                _nodeTypes[type] = attribute;
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

    public static float GetDefaultHeight(Type nodeType)
    {
        return _nodeTypes.TryGetValue(nodeType, out var attribute) ? attribute.DefaultHeight : 100f;
    }

    public static float GetDefaultWidth(Type nodeType)
    {
        return _nodeTypes.TryGetValue(nodeType, out var attribute) ? attribute.DefaultWidth : 200f;
    }

    public static NodeTypeAttribute GetAttribute(Type nodeType)
    {
        return _nodeTypes.TryGetValue(nodeType, out var attribute) ? attribute : null;
    }
}
