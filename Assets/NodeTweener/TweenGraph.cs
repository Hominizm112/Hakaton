using UnityEngine;
using DG.Tweening;
using System.IO;
using System.Collections.Generic;
using System;
using System.Linq;


[CreateAssetMenu(fileName = "TweenGraph", menuName = "DOTween/Tween Node Graph")]
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

    #region Build

    public Sequence BuildSequence(GameObject target)
    {

        if (nodes == null || nodes.Count == 0)
        {
            Debug.LogWarning("No nodes in graph");
            return null;
        }

        StartNode startNode = nodes.Find(n => n is StartNode) as StartNode;
        if (startNode == null)
        {
            Debug.LogError("No Start node found in graph");
            return null;
        }

        Sequence sequence = DOTween.Sequence();
        BuildSequenceRecursive(startNode, sequence, target, new HashSet<TweenNode>());

        return sequence;
    }


    private void BuildSequenceRecursive(TweenNode currentNode, Sequence sequence, GameObject target, HashSet<TweenNode> visited)
    {
        if (currentNode == null || visited.Contains(currentNode))
            return;

        visited.Add(currentNode);

        switch (currentNode)
        {
            case StartNode start:
                ProcessNodeOutputs(currentNode, sequence, target, visited);
                break;

            case EndNode end:
                return;

            default:
                DG.Tweening.Tweener tweener = currentNode.Execute(target);
                if (tweener != null)
                {
                    sequence.Append(tweener);
                }

                ProcessNodeOutputs(currentNode, sequence, target, visited);
                break;
        }
    }

    private void ProcessNodeOutputs(TweenNode currentNode, Sequence sequence, GameObject target, HashSet<TweenNode> visited)
    {
        if (currentNode.outputs.Count == 0) return;

        if (currentNode.outputs.Count == 1)
        {
            BuildSequenceRecursive(currentNode.outputs[0], sequence, target, visited);
        }
        else
        {
            Sequence parallelSection = DOTween.Sequence();

            List<Tween> parallelTweens = new List<Tween>();
            foreach (var outputNode in currentNode.outputs)
            {
                Sequence branchSequence = DOTween.Sequence();
                BuildSequenceRecursive(outputNode, branchSequence, target, new HashSet<TweenNode>(visited));

                if (branchSequence != null)
                {
                    var nestedTweens = GetNestedTweens(branchSequence);
                    parallelTweens.AddRange(nestedTweens);
                }
            }

            if (parallelTweens.Count > 0)
            {
                parallelSection.Append(parallelTweens[0]);

                for (int i = 1; i < parallelTweens.Count; i++)
                {
                    parallelSection.Join(parallelTweens[i]);
                }
            }

            sequence.Append(parallelSection);
        }
    }

    private List<Tween> GetNestedTweens(Sequence sequence)
    {
        List<Tween> tweens = new List<Tween>();

        tweens.Add(sequence);

        return tweens;
    }

    private void BuildSequenceFromNode(TweenNode currentNode, Sequence sequence, GameObject target, HashSet<TweenNode> visited)
    {
        if (currentNode == null || visited.Contains(currentNode))
            return;

        visited.Add(currentNode);

        if (currentNode is StartNode)
        {
            if (currentNode.outputs.Count > 1)
            {
                Sequence parallelSection = DOTween.Sequence();

                foreach (var outputNode in currentNode.outputs)
                {
                    BuildSequenceFromNode(outputNode, parallelSection, target, new HashSet<TweenNode>(visited));
                }

                sequence.Append(parallelSection);
            }
            else if (currentNode.outputs.Count == 1)
            {
                BuildSequenceFromNode(currentNode.outputs[0], sequence, target, visited);
            }
        }
        else if (currentNode is EndNode)
        {
            return;
        }
        else
        {
            DG.Tweening.Tweener tweener = currentNode.Execute(target);
            if (tweener != null)
            {
                sequence.Append(tweener);
            }

            if (currentNode.outputs.Count > 1)
            {
                Sequence parallelSection = DOTween.Sequence();

                foreach (var outputNode in currentNode.outputs)
                {
                    BuildSequenceFromNode(outputNode, parallelSection, target, new HashSet<TweenNode>(visited));
                }

                sequence.Append(parallelSection);
            }
            else if (currentNode.outputs.Count == 1)
            {
                BuildSequenceFromNode(currentNode.outputs[0], sequence, target, visited);
            }
        }
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

        foreach (var outputNode in node.outputs)
        {
            AddNodeToSequence(outputNode, sequence, visited, target);
        }
    }


    public bool ValidateGraph(out string errorMessage)
    {
        errorMessage = string.Empty;

        if (nodes == null || nodes.Count == 0)
        {
            errorMessage = "Graph has no nodes";
            return false;
        }

        int startNodeCount = nodes.Count(n => n is StartNode);
        if (startNodeCount == 0)
        {
            errorMessage = "Graph must contain exactly one Start node";
            return false;
        }
        else if (startNodeCount > 1)
        {
            errorMessage = "Graph can only contain one Start node";
            return false;
        }

        int endNodeCount = nodes.Count(n => n is EndNode);
        if (endNodeCount == 0)
        {
            errorMessage = "Graph must contain exactly one End node";
            return false;
        }
        else if (endNodeCount > 1)
        {
            errorMessage = "Graph can only contain one End node";
            return false;
        }

        StartNode startNode = nodes.Find(n => n is StartNode) as StartNode;
        if (startNode.outputs.Count == 0)
        {
            errorMessage = "Start node must be connected to other nodes";
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

    #endregion


}


