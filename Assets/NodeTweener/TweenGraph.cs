using UnityEngine;
using DG.Tweening;
using System.IO;
using System.Collections.Generic;
using System;


[CreateAssetMenu(fileName = "TweenGraph", menuName = "DOTween/Node Graph")]
public class TweenGraph : ScriptableObject
{
    public List<TweenNode> nodes = new();
    public TweenNode selectedNode;
    public NodeGraphSaveData saveData;

    public void SyncWithEditorNodes(List<EditorNode> editorNodes)
    {
        nodes.Clear();

        foreach (var editorNode in editorNodes)
        {
            if (editorNode.node != null)
            {
                nodes.Add(editorNode.node);
            }
        }
    }


    public Sequence BuildSequence(GameObject target)
    {
        Sequence sequence = DOTween.Sequence();

        if (nodes == null || nodes.Count == 0)
        {
            Debug.LogWarning("TweenGraph: No nodes in graph!");
            return sequence;
        }

        List<TweenNode> startNodes = nodes.FindAll(node => node.inputs.Count == 0);

        if (startNodes.Count == 0)
        {
            Debug.LogWarning("TweenGraph: No start nodes found (nodes without inputs)!");
            return sequence;
        }

        HashSet<TweenNode> visited = new HashSet<TweenNode>();
        foreach (var startNode in startNodes)
        {
            AddNodeToSequence(startNode, sequence, visited, target);
        }

        return sequence;
    }

    private void AddNodeToSequence(TweenNode node, Sequence sequence, HashSet<TweenNode> visited, GameObject target)
    {
        if (visited.Contains(node))
        {
            Debug.LogWarning($"TweenGraph: Circular dependency detected at node {node.name}!");
            return;
        }

        visited.Add(node);

        DG.Tweening.Tweener tweener = node.Execute(target);
        if (tweener != null)
        {
            sequence.Append(tweener);
        }
        else
        {
            Debug.LogWarning($"TweenGraph: Node {node.name} returned null tweener!");
        }

        // Process outputs (next nodes in sequence)
        foreach (var outputNode in node.outputs)
        {
            AddNodeToSequence(outputNode, sequence, visited, target);
        }
    }

    public bool ValidateGraph(out string errorMessage)
    {
        errorMessage = "";

        if (nodes.Count == 0)
        {
            errorMessage = "Graph has no nodes";
            return false;
        }

        // Check for circular dependencies
        foreach (var node in nodes)
        {
            if (HasCircularDependency(node, new HashSet<TweenNode>()))
            {
                errorMessage = $"Circular dependency detected starting from node: {node.name}";
                return false;
            }
        }

        // Check for unreachable nodes
        var reachable = GetReachableNodes();
        var unreachable = nodes.FindAll(n => !reachable.Contains(n));

        if (unreachable.Count > 0)
        {
            errorMessage = $"Graph has {unreachable.Count} unreachable nodes";
            return false;
        }

        return true;
    }

    private bool HasCircularDependency(TweenNode node, HashSet<TweenNode> visited)
    {
        if (visited.Contains(node))
            return true;

        visited.Add(node);

        foreach (var output in node.outputs)
        {
            if (HasCircularDependency(output, new HashSet<TweenNode>(visited)))
                return true;
        }

        return false;
    }

    private HashSet<TweenNode> GetReachableNodes()
    {
        HashSet<TweenNode> reachable = new HashSet<TweenNode>();
        var startNodes = nodes.FindAll(node => node.inputs.Count == 0);

        foreach (var startNode in startNodes)
        {
            TraverseReachable(startNode, reachable);
        }

        return reachable;
    }

    private void TraverseReachable(TweenNode node, HashSet<TweenNode> reachable)
    {
        if (reachable.Contains(node))
            return;

        reachable.Add(node);

        foreach (var output in node.outputs)
        {
            TraverseReachable(output, reachable);
        }
    }


}


