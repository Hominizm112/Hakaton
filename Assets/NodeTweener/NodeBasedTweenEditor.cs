#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;
using System.IO;
using DG.Tweening;
using System.Linq;

public class NodeBasedTweenEditor : EditorWindow
{
    [SerializeField] private GameObject previewTarget;
    private Sequence previewSequence;
    private bool isPreviewPlaying = false;

    private TweenGraph currentGraph;
    private string currentSavePath;

    private new bool hasUnsavedChanges = false;

    private List<EditorNode> editorNodes = new();
    private List<Connection> connections = new();

    private GUIStyle nodeStyle;
    private GUIStyle selectedNodeStyle;
    private GUIStyle inPointStyle;
    private GUIStyle outPointStyle;

    private Vector2 offset;
    private Vector2 drag;
    private ConnectionPoint selectedInPoint;
    private ConnectionPoint selectedOutPoint;
    private EditorNode selectedNode;
    private Connection hoveredConnection;

    private bool stylesInitialized = false;

    private double lastAutoSaveTime;
    private const double AUTO_SAVE_INTERVAL = 300;

    [SerializeField] private TweenGraph activeGraphAsset;
    private bool isCompiling = false;

    private Texture2D defaultNodeTexture;
    private Texture2D selectedNodeTexture;

    private bool debugMode = false;


    #region Debug

    private void DebugAllConnections()
    {

        ColorfulDebug.LogBlue($"=== CONNECTION DEBUG INFO ===");
        ColorfulDebug.LogGreen($"Total connections in list: {connections.Count}");

        if (connections != null && connections.Count > 0)
        {
            for (int i = 0; i < connections.Count; i++)
            {
                var conn = connections[i];
                string inNodeName = conn.inPoint?.node?.node?.name ?? "NULL";
                string outNodeName = conn.outPoint?.node?.node?.name ?? "NULL";
                ColorfulDebug.LogGreen($"Connection {i}: {outNodeName} -> {inNodeName}");
            }
        }
        else
        {
            ColorfulDebug.LogRed("No connections in list");
        }
        ColorfulDebug.LogBlue($"=== END DEBUG INFO ===");
    }

    private void DrawDebugInfo()
    {
        GUILayout.BeginArea(new Rect(10, position.height - 100, 300, 90));
        GUILayout.Label($"Nodes: {editorNodes?.Count ?? 0}", EditorStyles.miniLabel);
        GUILayout.Label($"Connections: {connections?.Count ?? 0}", EditorStyles.miniLabel);
        GUILayout.Label($"Hovered Connection: {hoveredConnection != null}", EditorStyles.miniLabel);
        GUILayout.Label($"Selected Node: {selectedNode != null}", EditorStyles.miniLabel);

        if (GUILayout.Button("Debug Info", GUILayout.Width(100)))
        {
            DebugAllConnections();
        }
        GUILayout.EndArea();
    }

    #endregion

    #region Draw

    private void OnGUI()
    {
        if (!stylesInitialized)
        {
            InitializeStyles();
        }

        if (editorNodes == null) editorNodes = new List<EditorNode>();
        if (connections == null) connections = new List<Connection>();

        Rect backgroundRect = new Rect(0, 0, position.width, position.height);
        EditorGUI.DrawRect(backgroundRect, HexColorUtility.ParseHex("#2d232e"));

        DrawGrid(20, 0.05f, HexColorUtility.ParseHex("#594e36"));
        DrawGrid(100, 0.1f, HexColorUtility.ParseHex("#594e36"));



        DrawConnections();
        DrawConnectionLine(Event.current);
        DrawNodes();


        DrawToolbar();
        DrawExecutionToolbar();
        DrawDebugInfo();


        CheckCompilationHover(Event.current.mousePosition);

        ProcessNodeEvents(Event.current);
        ProcessEvents(Event.current);




        if (GUI.changed) Repaint();
    }



    private void DrawGrid(float gridSpacing, float gridOpacity, Color gridColor)
    {
        int widthDivs = Mathf.CeilToInt(position.width / gridSpacing);
        int heightDivs = Mathf.CeilToInt(position.height / gridSpacing);

        Handles.BeginGUI();
        Handles.color = new Color(gridColor.r, gridColor.g, gridColor.b, gridColor.a * gridOpacity);

        offset += drag * 0.5f;
        Vector3 newOffset = new Vector3(offset.x % gridSpacing, offset.y % gridSpacing, 0);

        for (int i = 0; i < widthDivs; i++)
        {
            Handles.DrawLine(new Vector3(gridSpacing * i, -gridSpacing, 0) + newOffset,
                            new Vector3(gridSpacing * i, position.height, 0f) + newOffset);
        }

        for (int j = 0; j < heightDivs; j++)
        {
            Handles.DrawLine(new Vector3(-gridSpacing, gridSpacing * j, 0) + newOffset,
                            new Vector3(position.width, gridSpacing * j, 0f) + newOffset);
        }

        Handles.color = Color.white;
        Handles.EndGUI();
    }

    private void DrawNodes()
    {
        if (editorNodes == null || editorNodes.Count == 0)
        {
            if (currentGraph?.nodes?.Count > 0)
            {
                DrawNoNodesMessage();
            }
            return;
        }

        // Draw all valid nodes
        for (int i = 0; i < editorNodes.Count; i++)
        {
            if (editorNodes[i]?.node != null)
            {
                editorNodes[i].Draw();
            }
            else
            {
                // Remove null nodes
                editorNodes.RemoveAt(i);
                i--;
            }
        }
    }

    private void DrawNoNodesMessage()
    {
        GUIStyle messageStyle = new GUIStyle(EditorStyles.label);
        messageStyle.alignment = TextAnchor.MiddleCenter;
        messageStyle.normal.textColor = Color.yellow;
        messageStyle.fontSize = 14;

        Rect messageRect = new Rect(position.width / 2 - 200, position.height / 2 - 20, 400, 40);
        EditorGUI.DrawRect(messageRect, new Color(0.2f, 0.2f, 0.2f, 0.8f));
        GUI.Label(messageRect, "Graph data exists but nodes are not visible.\nClick 'Restore Graph' in context menu.", messageStyle);
    }

    private void DrawConnections()
    {
        if (connections == null || connections.Count == 0) return;

        bool needsRepaint = false;

        for (int i = 0; i < connections.Count; i++)
        {
            var conn = connections[i];
            if (conn == null)
            {
                connections.RemoveAt(i);
                i--;
                needsRepaint = true;
                continue;
            }

            bool isValid = conn.inPoint != null &&
                          conn.outPoint != null &&
                          conn.inPoint.node != null &&
                          conn.outPoint.node != null &&
                          conn.inPoint.node.node != null &&
                          conn.outPoint.node.node != null;

            if (!isValid)
            {
                connections.RemoveAt(i);
                i--;
                needsRepaint = true;
                continue;
            }

            bool isHovered = conn == hoveredConnection;
            conn.Draw(isHovered);

            DrawConnectionBounds(conn, isHovered);

        }

        if (needsRepaint)
        {
            Repaint();
        }
    }

    private void DrawConnectionLine(Event e)
    {
        if (selectedInPoint != null && selectedOutPoint == null)
        {
            Handles.DrawBezier(
                selectedInPoint.rect.center,
                e.mousePosition,
                selectedInPoint.rect.center + Vector2.left * 50f,
                e.mousePosition - Vector2.left * 50f,
                Color.white,
                null,
                2f
            );

            GUI.changed = true;

        }


        if (selectedOutPoint != null && selectedInPoint == null)
        {
            Handles.DrawBezier(
                selectedOutPoint.rect.center,
                e.mousePosition,
                selectedOutPoint.rect.center - Vector2.left * 50f,
                e.mousePosition + Vector2.left * 50f,
                Color.white,
                null,
                2f
            );

            GUI.changed = true;
        }
    }

    private void DrawConnectionBounds(Connection conn, bool isHovered)
    {
        if (!debugMode) return;


        // Rect bounds = conn.GetBounds();
        // Handles.color = isHovered ? Color.green : Color.red;
        // Handles.DrawWireCube(bounds.center, bounds.size);
        // Handles.color = Color.white;
    }

    private void CreateNodeStyles()
    {
        if (defaultNodeTexture == null || selectedNodeTexture == null)
        {
            LoadNodeTextures();
        }

        nodeStyle = new GUIStyle();
        nodeStyle.normal.background = defaultNodeTexture;
        nodeStyle.normal.textColor = GetContrastColor(HexColorUtility.ParseHex("#474448"));
        nodeStyle.border = new RectOffset(12, 12, 12, 12);
        nodeStyle.padding = new RectOffset(10, 10, 10, 10);
        nodeStyle.alignment = TextAnchor.UpperCenter;
        nodeStyle.fontStyle = FontStyle.Bold;

        selectedNodeStyle = new GUIStyle();
        selectedNodeStyle.normal.background = selectedNodeTexture;
        selectedNodeStyle.normal.textColor = GetContrastColor(HexColorUtility.ParseHex("#534b52"));
        selectedNodeStyle.border = new RectOffset(12, 12, 12, 12);
        selectedNodeStyle.padding = new RectOffset(10, 10, 10, 10);
        selectedNodeStyle.alignment = TextAnchor.UpperCenter;
        selectedNodeStyle.fontStyle = FontStyle.Bold;

        inPointStyle = new GUIStyle();
        inPointStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn left.png") as Texture2D;
        inPointStyle.active.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn left on.png") as Texture2D;
        inPointStyle.border = new RectOffset(4, 4, 12, 12);

        outPointStyle = new GUIStyle();
        outPointStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn right.png") as Texture2D;
        outPointStyle.active.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn right on.png") as Texture2D;
        outPointStyle.border = new RectOffset(4, 4, 12, 12);
    }


    public void SetNodeColor(Color newColor)
    {
        CreateNodeStyles();
        Repaint();
    }

    public void SetNodeColor(string hexColor)
    {
        SetNodeColor(HexColorUtility.ParseHex(hexColor));
    }

    public static Color GetContrastColor(Color backgroundColor)
    {
        float luminance = 0.299f * backgroundColor.r + 0.587f * backgroundColor.g + 0.114f * backgroundColor.b;
        return luminance > 0.5f ? Color.black : Color.white;
    }

    private void InitializeStyles()
    {
        if (stylesInitialized) return;

        CreateNodeStyles();
        stylesInitialized = true;
    }

    #endregion


    #region Processing

    private void ProcessEvents(Event e)
    {
        drag = Vector2.zero;

        switch (e.type)
        {
            case EventType.MouseDown:
                if (e.button == 1)
                {
                    ClearConnectionSelection();
                    EditorNode clickedNode = GetNodeAtPosition(e.mousePosition);
                    if (clickedNode == null)
                    {
                        ProcessContextMenu(e.mousePosition);

                    }
                }

                if (e.button == 0)
                {
                    Connection clickedConnection = GetConnectionAtPosition(e.mousePosition);
                    if (clickedConnection != null)
                    {
                        RemoveConnection(clickedConnection);
                        e.Use();
                    }
                    else
                    {
                        hoveredConnection = null;
                    }
                }
                break;

            case EventType.MouseDrag:
                if (e.button == 0)
                {
                    OnDrag(e.delta);


                }
                break;

            case EventType.MouseMove:
                Repaint();
                e.Use();
                break;


            case EventType.KeyDown:
                if (e.keyCode == KeyCode.S && e.control)
                {
                    if (e.shift)
                    {
                        SaveGraph(null);
                    }
                    else
                    {
                        SaveGraph();
                    }
                    e.Use();
                }
                else if (e.keyCode == KeyCode.O && e.control)
                {
                    LoadGraphFromFile();
                    e.Use();
                }
                else if (e.keyCode == KeyCode.N && e.control)
                {
                    NewGraph();
                    e.Use();
                }
                else if (e.keyCode == KeyCode.Delete || e.keyCode == KeyCode.X)
                {
                    if (selectedNode != null)
                    {
                        if (!selectedNode.isSelected)
                        {
                            selectedNode = null;
                            return;
                        }

                        ColorfulDebug.LogRed($"Node deleted");
                        selectedNode?.RemoveNode();
                    }
                }
                else if (e.keyCode == KeyCode.P)
                {
                    ColorfulDebug.LogRed("Lists cleared");
                    editorNodes.Clear();
                    connections.Clear();
                }
                break;



        }
    }

    private EditorNode GetNodeAtPosition(Vector2 position)
    {
        if (editorNodes == null) return null;

        for (int i = editorNodes.Count - 1; i >= 0; i--)
        {
            if (editorNodes[i].rect.Contains(position))
            {
                return editorNodes[i];
            }
        }

        return null;
    }

    private void ProcessNodeEvents(Event e)
    {
        if (editorNodes != null)
        {
            for (int i = editorNodes.Count - 1; i >= 0; i--)
            {
                if (editorNodes[i]?.node != null)
                {
                    bool guiChanged = editorNodes[i].ProcessEvents(e);
                    if (guiChanged)
                    {
                        GUI.changed = true;
                    }
                }
            }
        }
    }

    private void ProcessContextMenu(Vector2 mousePosition)
    {
        GenericMenu genericMenu = new GenericMenu();

        foreach (var nodeType in NodeRegistry.GetAllNodeTypes())
        {
            string menuName = NodeRegistry.GetMenuName(nodeType);
            genericMenu.AddItem(new GUIContent($"Add {menuName}"), false, () => OnClickAddNode(mousePosition, nodeType));
        }

        genericMenu.AddSeparator("");
        genericMenu.AddItem(new GUIContent("Restore Graph"), false, AutoRestoreGraph);
        genericMenu.AddItem(new GUIContent("Force New Graph"), false, () =>
        {
            NewGraph();
            Repaint();
        });

        genericMenu.ShowAsContext();
    }

    #endregion


    #region Save/Load

    private void SaveGraph(string path = null)
    {
        try
        {
            if (EditorApplication.isCompiling || EditorApplication.isPlaying)
            {
                Debug.LogWarning("Cannot save during compilation or play mode");
                return;
            }

            string savePath = path ?? currentSavePath;

            if (activeGraphAsset != null && string.IsNullOrEmpty(savePath))
            {
                savePath = AssetDatabase.GetAssetPath(activeGraphAsset);
            }

            if (string.IsNullOrEmpty(savePath))
            {
                savePath = EditorUtility.SaveFilePanel("Save Node Graph", "Assets", "NewTweenSequence", "asset");
                if (string.IsNullOrEmpty(savePath)) return;
                savePath = "Assets" + savePath.Substring(Application.dataPath.Length);
            }

            Debug.Log($"Saving to: {savePath}");

            TweenGraph graphToSave;
            bool isNewAsset = false;

            if (AssetDatabase.LoadAssetAtPath<TweenGraph>(savePath) != null)
            {
                graphToSave = AssetDatabase.LoadAssetAtPath<TweenGraph>(savePath);
            }
            else
            {
                graphToSave = ScriptableObject.CreateInstance<TweenGraph>();
                isNewAsset = true;
            }

            // Update graph data
            graphToSave.saveData = CreateSaveData();
            graphToSave.name = Path.GetFileNameWithoutExtension(savePath);

            // Save the asset
            if (isNewAsset)
            {
                AssetDatabase.CreateAsset(graphToSave, savePath);
            }

            // Add nodes as sub-assets
            foreach (var editorNode in editorNodes)
            {
                if (editorNode?.node != null)
                {
                    editorNode.node.hideFlags = HideFlags.None;
                    if (!AssetDatabase.Contains(editorNode.node))
                    {
                        AssetDatabase.AddObjectToAsset(editorNode.node, graphToSave);
                    }
                    EditorUtility.SetDirty(editorNode.node);
                }
            }

            // Clean up destroyed sub-assets
            var existingAssets = AssetDatabase.LoadAllAssetsAtPath(savePath);
            foreach (var asset in existingAssets)
            {
                if (asset is TweenNode node && !editorNodes.Any(en => en.node == node))
                {
                    AssetDatabase.RemoveObjectFromAsset(node);
                    DestroyImmediate(node, true);
                }
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            currentGraph = graphToSave;
            activeGraphAsset = graphToSave;
            currentSavePath = savePath;
            hasUnsavedChanges = false;


            titleContent = new GUIContent($"DOTween Node Editor - {Path.GetFileName(savePath)}");
            Debug.Log($"✅ Saved: {editorNodes.Count} nodes, {connections.Count} connections");
            ShowNotification(new GUIContent("Graph Saved"));
        }
        catch (Exception e)
        {
            Debug.LogError($"❌ Save failed: {e.Message}");
            ShowNotification(new GUIContent("Save Failed!"));
        }
    }

    private void LoadGraphFromFile()
    {
        try
        {
            string loadPath = EditorUtility.OpenFilePanel("Load Node Graph", "Assets", "asset");
            if (string.IsNullOrEmpty(loadPath)) return;

            loadPath = "Assets" + loadPath.Substring(Application.dataPath.Length);
            TweenGraph loadedGraph = AssetDatabase.LoadAssetAtPath<TweenGraph>(loadPath);

            if (loadedGraph != null)
            {
                LoadGraphAsset(loadedGraph);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Load failed: {e.Message}");
        }
    }

    private void NewGraph()
    {
        if (hasUnsavedChanges && !EditorUtility.DisplayDialog("Unsaved Changes",
            "You have unsaved changes. Create new graph anyway?", "Yes", "No"))
        {
            return;
        }

        currentGraph = ScriptableObject.CreateInstance<TweenGraph>();
        activeGraphAsset = null;
        editorNodes.Clear();
        connections.Clear();
        currentSavePath = null;
        hasUnsavedChanges = false;

        CreateDefaultNodes();

        titleContent = new GUIContent("DOTween Node Editor - New Graph");
        Repaint();
    }

    private NodeGraphSaveData CreateSaveData()
    {
        var saveData = new NodeGraphSaveData();
        saveData.canvasOffset = offset;

        foreach (var editorNode in editorNodes)
        {
            if (editorNode?.node != null)
            {
                var nodeData = new NodeSaveData
                {
                    nodeType = editorNode.node.GetType().AssemblyQualifiedName,
                    nodeName = editorNode.node.name,
                    position = editorNode.rect.position,
                    guid = editorNode.guid ?? Guid.NewGuid().ToString()
                };

                saveData.nodes.Add(nodeData);
            }
        }

        foreach (var connection in connections)
        {
            if (connection?.outPoint?.node != null && connection?.inPoint?.node != null)
            {
                saveData.connections.Add(new ConnectionSaveData
                {
                    fromNodeGuid = connection.outPoint.node.guid,
                    toNodeGuid = connection.inPoint.node.guid
                });
            }
        }

        return saveData;
    }



    private void LoadGraphAsset(TweenGraph graphAsset)
    {
        if (graphAsset == null) return;

        try
        {
            StopPreview();

            // Clear everything
            editorNodes.Clear();
            connections.Clear();

            activeGraphAsset = graphAsset;
            currentGraph = graphAsset;
            currentSavePath = AssetDatabase.GetAssetPath(graphAsset);

            if (graphAsset.saveData != null)
            {
                LoadFromSaveData(graphAsset.saveData);
            }
            else
            {
                // If no save data, create default nodes
                CreateDefaultNodes();
            }

            hasUnsavedChanges = false;
            titleContent = new GUIContent($"DOTween Node Editor - {graphAsset.name}");

            Repaint();
            Debug.Log($"Loaded: {editorNodes.Count} nodes");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Load failed: {e.Message}");
        }
    }

    private void LoadFromSaveData(NodeGraphSaveData saveData)
    {
        if (saveData == null) return;

        Debug.Log($"Loading: {saveData.nodes.Count} nodes, {saveData.connections.Count} connections");

        editorNodes.Clear();
        connections.Clear();
        offset = saveData.canvasOffset;

        var guidToNodeMap = new Dictionary<string, EditorNode>();

        foreach (var nodeData in saveData.nodes)
        {
            try
            {
                var nodeType = Type.GetType(nodeData.nodeType);
                if (nodeType == null)
                {
                    Debug.LogError($"Failed to resolve node type: {nodeData.nodeType}");
                    continue;
                }

                TweenNode nodeAsset = FindNodeInAsset(nodeData.guid, nodeType) ?? CreateNodeAsset(nodeType);

                nodeAsset.guid = nodeData.guid;
                nodeAsset.name = nodeData.nodeName;
                nodeAsset.nodeRect.position = nodeData.position;

                var editorNode = new EditorNode(
                    nodeData.position,
                    nodeStyle,
                    selectedNodeStyle,
                    inPointStyle,
                    outPointStyle,
                    OnClickInPoint,
                    OnClickOutPoint,
                    OnClickRemoveNode,
                    OnClickSelectNode,
                    nodeAsset
                );

                editorNode.guid = nodeData.guid;
                editorNodes.Add(editorNode);
                guidToNodeMap[nodeData.guid] = editorNode;

                Debug.Log($"Loaded node: {nodeAsset.name} ({nodeType.Name})");
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to load node {nodeData.nodeName}: {e.Message}");
            }
        }

        foreach (var connectionData in saveData.connections)
        {
            try
            {
                if (guidToNodeMap.TryGetValue(connectionData.fromNodeGuid, out var fromNode) &&
                    guidToNodeMap.TryGetValue(connectionData.toNodeGuid, out var toNode))
                {
                    if (fromNode?.outPoint != null && toNode?.inPoint != null)
                    {
                        var connection = new Connection(toNode.inPoint, fromNode.outPoint);
                        connections.Add(connection);

                        if (!fromNode.node.outputs.Contains(toNode.node))
                            fromNode.node.outputs.Add(toNode.node);

                        if (!toNode.node.inputs.Contains(fromNode.node))
                            toNode.node.inputs.Add(fromNode.node);

                        Debug.Log($"Restored connection: {fromNode.node.name} -> {toNode.node.name}");
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to restore connection: {e.Message}");
            }
        }

        SyncGraphWithEditorNodes();
        Repaint();
    }

    private TweenNode FindNodeInAsset(string guid, System.Type nodeType)
    {
        if (currentGraph == null) return null;

        var assetPath = AssetDatabase.GetAssetPath(currentGraph);
        if (string.IsNullOrEmpty(assetPath)) return null;

        var assets = AssetDatabase.LoadAllAssetsAtPath(assetPath);
        foreach (var asset in assets)
        {
            if (asset is TweenNode node && node.guid == guid && node.GetType() == nodeType)
            {
                return node;
            }
        }

        return null;
    }

    private void AutoRestoreGraph()
    {
        Debug.Log("Attempting auto-restore of graph...");

        // Don't auto-restore during play mode as assets might be in temporary state
        if (EditorApplication.isPlaying)
        {
            Debug.Log("Skipping auto-restore during Play Mode");
            return;
        }

        // Priority 1: Active graph asset
        if (activeGraphAsset != null)
        {
            Debug.Log($"Auto-restoring from active graph: {activeGraphAsset.name}");
            LoadGraphAsset(activeGraphAsset);
            return;
        }

        // Priority 2: Current save path
        if (!string.IsNullOrEmpty(currentSavePath))
        {
            var graph = AssetDatabase.LoadAssetAtPath<TweenGraph>(currentSavePath);
            if (graph != null)
            {
                Debug.Log($"Auto-restoring from save path: {currentSavePath}");
                LoadGraphAsset(graph);
                return;
            }
        }

        // Priority 3: Create default nodes if empty
        if (editorNodes.Count == 0)
        {
            Debug.Log("No saved data found, creating default nodes");
            CreateDefaultNodes();
        }
    }

    #endregion

    #region Event Handlers

    private void OnEnable()
    {




        if (editorNodes == null) editorNodes = new List<EditorNode>();
        if (connections == null) connections = new List<Connection>();

        LoadNodeTextures();
        CreateNodeStyles();
        stylesInitialized = true;

        EditorApplication.update += OnEditorUpdate;
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        EditorApplication.update += CheckCompilationStatus;

        EditorApplication.projectChanged += OnProjectChanged;

        AutoRestoreGraph();
        Repaint();
    }



    private void OnDisable()
    {


        EditorApplication.update -= OnEditorUpdate;
        EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
        EditorApplication.update -= CheckCompilationStatus;
        EditorApplication.projectChanged -= OnProjectChanged;

        StopPreview();
    }

    private void OnProjectChanged()
    {
        if (activeGraphAsset != null)
        {
            EditorApplication.delayCall += () =>
            {
                if (activeGraphAsset != null)
                {
                    LoadGraphAsset(activeGraphAsset);
                }
            };
        }
    }

    private void OnDestroy()
    {
        if (hasUnsavedChanges)
        {
            if (EditorUtility.DisplayDialog("Unsaved Changes",
                "You have unsaved changes. Save before closing?", "Save", "Don't Save"))
            {
                SaveGraph();
            }
        }

    }


    private void OnSelectionChange()
    {
        if (Selection.activeObject is TweenGraph selectedGraph)
        {
            Debug.Log($"Graph asset selected: {selectedGraph.name}");
            LoadGraphAsset(selectedGraph);
        }
        Repaint();
    }

    private void OnPlayModeStateChanged(PlayModeStateChange state)
    {
        Debug.Log($"PlayMode state changed: {state}");

        switch (state)
        {
            case PlayModeStateChange.ExitingEditMode:
                if (hasUnsavedChanges && activeGraphAsset != null)
                {
                    Debug.Log("Auto-saving before Play Mode");
                    SaveGraph();
                }
                StopPreview();
                AutoRestoreGraph();

                break;

            case PlayModeStateChange.EnteredPlayMode:
                isPreviewPlaying = false;
                previewSequence = null;
                EditorApplication.delayCall += () =>
                {
                    AutoRestoreGraph();
                    Repaint();
                };
                break;

            case PlayModeStateChange.ExitingPlayMode:
                StopPreview();
                break;

            case PlayModeStateChange.EnteredEditMode:
                EditorApplication.delayCall += () =>
                {
                    Debug.Log("Restoring graph after Play Mode");
                    if (activeGraphAsset != null)
                    {
                        LoadGraphAsset(activeGraphAsset);
                    }
                    else
                    {
                        AutoRestoreGraph();
                    }
                    Repaint();
                };
                break;
        }
    }

    private void Update()
    {
        if (hasUnsavedChanges && EditorApplication.timeSinceStartup - lastAutoSaveTime > AUTO_SAVE_INTERVAL)
        {
            if (!string.IsNullOrEmpty(currentSavePath))
            {
                SaveGraph(currentSavePath);
                lastAutoSaveTime = EditorApplication.timeSinceStartup;
            }
        }

        if (EditorApplication.isPlayingOrWillChangePlaymode && !EditorApplication.isPlaying)
        {
            if (hasUnsavedChanges && activeGraphAsset != null)
            {
                Debug.Log("Auto-saving before entering Play Mode");
                SaveGraph();
            }
        }
    }

    private void OnEditorUpdate()
    {

    }

    private void OnDrag(Vector2 delta)
    {
        drag = delta;

        if (editorNodes != null)
        {
            for (int i = 0; i < editorNodes.Count; i++)
            {
                editorNodes[i].Drag(delta);
            }
        }


        GUI.changed = true;
    }

    private void OnClickAddNode(Vector2 mousePosition, System.Type nodeType)
    {


        if (editorNodes == null) editorNodes = new List<EditorNode>();
        if (connections == null) connections = new List<Connection>();

        TweenNode nodeAsset = CreateNodeAsset(nodeType);

        EditorNode editorNode = new EditorNode(
            mousePosition,
            nodeStyle,
            selectedNodeStyle,
            inPointStyle,
            outPointStyle,
            OnClickInPoint,
            OnClickOutPoint,
            OnClickRemoveNode,
            OnClickSelectNode,
            nodeAsset
        );

        editorNodes.Add(editorNode);

        SyncGraphWithEditorNodes();
        hasUnsavedChanges = true;
        SaveGraph();
        Repaint();

        Debug.Log($"Added node: {nodeType.Name}. Total nodes: {editorNodes.Count}");
    }



    private void OnClickInPoint(ConnectionPoint inPoint)
    {
        if (inPoint.node.node is StartNode)
        {
            Debug.LogWarning("Cannot connect to Start node's input");
            ClearConnectionSelection();
            return;
        }

        selectedInPoint = inPoint;

        if (selectedOutPoint != null)
        {
            if (selectedOutPoint.node != selectedInPoint.node)
            {
                CreateConnection();
                ClearConnectionSelection();
            }
            else
            {
                ClearConnectionSelection();
            }
        }
    }

    private void OnClickOutPoint(ConnectionPoint outPoint)
    {
        if (outPoint.node.node is EndNode)
        {
            Debug.LogWarning("Cannot connect from End node's output");
            ClearConnectionSelection();
            return;
        }

        selectedOutPoint = outPoint;

        if (selectedInPoint != null)
        {
            if (selectedOutPoint.node != selectedInPoint.node)
            {
                CreateConnection();
                ClearConnectionSelection();
            }
            else
            {
                ClearConnectionSelection();
            }
        }
    }

    private void OnClickRemoveNode(EditorNode node)
    {
        if (node?.node == null) return;

        if (connections != null)
        {
            List<Connection> connectionsToRemove = new();

            for (int i = connections.Count - 1; i >= 0; i--)
            {
                var conn = connections[i];
                if (conn?.inPoint?.node == node || conn?.outPoint?.node == node)
                {
                    connectionsToRemove.Add(conn);
                }
            }

            foreach (var connection in connectionsToRemove)
            {
                connections.Remove(connection);

                if (connection?.outPoint?.node?.node != null && connection?.inPoint?.node?.node != null)
                {
                    connection.outPoint.node.node.outputs.Remove(connection.inPoint.node.node);
                    connection.inPoint.node.node.inputs.Remove(connection.outPoint.node.node);
                }
            }
            UpdateConnectionsList();

        }

        if (editorNodes.Contains(node))
        {
            editorNodes.Remove(node);
        }

        SyncGraphWithEditorNodes();
        GUI.changed = true;
        // DebugAllConnections();


    }


    private void OnClickSelectNode(EditorNode node)
    {
        selectedNode = node;
    }

    #endregion


    #region Connections
    private void CreateConnection()
    {
        if (connections == null)
        {
            connections = new List<Connection>();
        }

        if (CheckConnectionDouble(selectedInPoint, selectedOutPoint))
        {
            return;
        }


        connections.Add(new Connection(selectedInPoint, selectedOutPoint));

        selectedOutPoint.node.node.outputs.Add(selectedInPoint.node.node);
        selectedInPoint.node.node.inputs.Add(selectedOutPoint.node.node);
        UpdateConnectionsList();
        OnConnectionsChanged();
        SyncGraphWithEditorNodes();


    }


    private bool CheckConnectionDouble(ConnectionPoint inPoint, ConnectionPoint outPoint)
    {
        Connection connectionDouble = connections.Find(r => r.inPoint == inPoint && r.outPoint == outPoint);

        if (connectionDouble != null)
        {
            return true;
        }

        return false;
    }

    private void ClearConnectionSelection()
    {
        selectedInPoint = null;
        selectedOutPoint = null;
    }

    private void UpdateConnectionsList()
    {
        for (int i = connections.Count - 1; i >= 0; i--)
        {
            if (connections[i].inPoint == null || connections[i].outPoint == null)
            {
                connections.RemoveAt(i);
            }
        }
    }

    private Connection GetConnectionAtPosition(Vector2 position)
    {
        if (connections == null || connections.Count == 0) return null;

        Connection closestConnection = null;
        float closestDistance = float.MaxValue;

        foreach (var connection in connections)
        {
            if (connection?.inPoint == null || connection?.outPoint == null)
                continue;

            if (connection.IsMouseOver(position, 12f))
            {
                float distance = GetExactDistanceToConnection(position, connection);
                if (distance < closestDistance)
                {
                    closestConnection = connection;
                    closestDistance = distance;
                }
            }
        }

        return closestConnection;
    }

    private float GetExactDistanceToConnection(Vector2 point, Connection connection)
    {
        if (connection?.inPoint == null || connection?.outPoint == null)
            return float.MaxValue;

        Vector2 start = connection.inPoint.rect.center;
        Vector2 end = connection.outPoint.rect.center;
        Vector2 startTangent = start + Vector2.left * 50f;
        Vector2 endTangent = end - Vector2.left * 50f;

        int samples = 30;
        float minDistance = float.MaxValue;

        Vector2 previousPoint = start;
        for (int i = 1; i <= samples; i++)
        {
            float t = i / (float)samples;
            Vector2 currentPoint = connection.CalculateBezierPoint(t, start, startTangent, endTangent, end);

            float distance = connection.DistanceToLineSegment(point, previousPoint, currentPoint);
            if (distance < minDistance)
            {
                minDistance = distance;
            }
            previousPoint = currentPoint;
        }

        return minDistance;
    }



    private void RemoveConnection(Connection connection)
    {
        if (connection == null)
        {
            Debug.LogWarning("Attempted to remove null connection");
            return;
        }

        if (connections == null)
        {
            Debug.LogWarning("Connections list is null");
            return;
        }

        if (connections.Contains(connection))
        {
            connections.Remove(connection);
            Debug.Log("Removed connection from visual list");
        }
        else
        {
            Debug.LogWarning("Connection not found in connections list");
        }

        if (connection.outPoint?.node?.node != null && connection.inPoint?.node?.node != null)
        {
            if (connection.outPoint.node.node.outputs.Contains(connection.inPoint.node.node))
            {
                connection.outPoint.node.node.outputs.Remove(connection.inPoint.node.node);
                Debug.Log("Removed from output node's outputs");
            }

            if (connection.inPoint.node.node.inputs.Contains(connection.outPoint.node.node))
            {
                connection.inPoint.node.node.inputs.Remove(connection.outPoint.node.node);
                Debug.Log("Removed from input node's inputs");
            }
        }
        else
        {
            Debug.LogWarning("Could not clean up node graph references - null connection points");
        }

        if (hoveredConnection == connection)
        {
            hoveredConnection = null;
        }

        OnConnectionsChanged();
        GUI.changed = true;
        // Debug.Log($"Connection removed. Total connections: {connections.Count}");
        SyncGraphWithEditorNodes();

    }


    private void OnConnectionsChanged()
    {
        Repaint();
    }

    private void ValidateConnections()
    {
        int validConnections = 0;
        int invalidConnections = 0;

        foreach (var connection in connections)
        {
            bool isValid = connection != null &&
                          connection.inPoint != null &&
                          connection.outPoint != null &&
                          connection.inPoint.node != null &&
                          connection.outPoint.node != null &&
                          connection.inPoint.node.node != null &&
                          connection.outPoint.node.node != null;

            if (isValid)
            {
                validConnections++;
            }
            else
            {
                invalidConnections++;
            }
        }

        Debug.Log($"Connection Validation: {validConnections} valid, {invalidConnections} invalid");

        if (invalidConnections > 0)
        {
            connections.RemoveAll(conn =>
                conn == null ||
                conn.inPoint == null ||
                conn.outPoint == null ||
                conn.inPoint.node == null ||
                conn.outPoint.node == null ||
                conn.inPoint.node.node == null ||
                conn.outPoint.node.node == null
            );
            Debug.Log($"Removed {invalidConnections} invalid connections");
        }
    }

    private void CheckCompilationHover(Vector2 mousePosition)
    {
        var newHoveredConnection = GetConnectionAtPosition(mousePosition);

        if (newHoveredConnection != hoveredConnection)
        {
            hoveredConnection = newHoveredConnection;
            Repaint();

            if (hoveredConnection != null)
            {
                Debug.Log($"Hovering connection: {hoveredConnection.outPoint?.node?.node?.name} -> {hoveredConnection.inPoint?.node?.node?.name}");
            }
        }

    }


    #endregion

    #region Nodes


    private TweenNode CreateNodeAsset(System.Type nodeType)
    {
        var node = ScriptableObject.CreateInstance(nodeType) as TweenNode;
        node.name = $"{nodeType.Name}_{Guid.NewGuid().ToString("N").Substring(0, 8)}";
        node.guid = Guid.NewGuid().ToString();
        node.hideFlags = HideFlags.None;

        if (activeGraphAsset != null && !string.IsNullOrEmpty(AssetDatabase.GetAssetPath(activeGraphAsset)))
        {
            if (!AssetDatabase.Contains(node))
            {
                AssetDatabase.AddObjectToAsset(node, activeGraphAsset);

            }
        }


        return node;
    }

    private void CleanupDestroyedNodes()
    {
        if (editorNodes != null)
        {
            for (int i = editorNodes.Count - 1; i >= 0; i--)
            {
                if (editorNodes[i] == null || editorNodes[i].node == null)
                {
                    editorNodes.RemoveAt(i);
                }
            }
        }

        if (connections != null)
        {
            for (int i = connections.Count - 1; i >= 0; i--)
            {
                var conn = connections[i];
                if (conn == null ||
                    conn.inPoint?.node?.node == null ||
                    conn.outPoint?.node?.node == null)
                {
                    connections.RemoveAt(i);
                }
            }
        }
    }


    private System.Type GetNodeTypeFromName(string typeName)
    {
        if (string.IsNullOrEmpty(typeName))
        {
            Debug.LogError("Node type name is null or empty");
            return null;
        }

        if (typeName == "StartNode") return typeof(StartNode);
        if (typeName == "EndNode") return typeof(EndNode);

        var fromRegistry = NodeRegistry.GetAllNodeTypes().FirstOrDefault(t => t.Name == typeName);
        if (fromRegistry != null) return fromRegistry;

        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            try
            {
                var type = assembly.GetType(typeName) ??
                           assembly.GetType($"YourNamespace.{typeName}") ??
                           assembly.GetTypes().FirstOrDefault(t => t.Name == typeName && typeof(TweenNode).IsAssignableFrom(t));

                if (type != null)
                {
                    Debug.Log($"Resolved node type '{typeName}' to {type.FullName}");
                    return type;
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning($"Error searching assembly {assembly.GetName().Name}: {ex.Message}");
            }
        }

        Debug.LogError($"❌ FAILED TO RESOLVE NODE TYPE: '{typeName}'");
        return null;
    }



    private void CreateDefaultNodes()
    {
        if (editorNodes.Count == 0 && connections.Count == 0)
        {
            OnClickAddNode(new Vector2(100, 200), typeof(StartNode));
            OnClickAddNode(new Vector2(500, 200), typeof(EndNode));
        }
    }

    #endregion

    #region UI Integration

    private void DrawExecutionToolbar()
    {
        GUILayout.BeginHorizontal(EditorStyles.toolbar);

        // Preview target selection
        GUILayout.Label("Preview Target:", GUILayout.Width(80));
        previewTarget = (GameObject)EditorGUILayout.ObjectField(previewTarget, typeof(GameObject), true, GUILayout.Width(150));

        GUILayout.Space(10);

        // Execution buttons
        GUI.enabled = previewTarget != null && editorNodes.Count > 0;

        if (GUILayout.Button("Preview", EditorStyles.toolbarButton))
        {
            PreviewSequence();
        }

        if (GUILayout.Button(isPreviewPlaying ? "Stop" : "Play", EditorStyles.toolbarButton))
        {
            if (isPreviewPlaying)
            {
                StopPreview();
            }
            else
            {
                PlaySequence();
            }
        }

        GUI.enabled = true;

        GUILayout.Space(10);

        // Validation
        if (GUILayout.Button("Validate", EditorStyles.toolbarButton))
        {
            ValidateGraph();
        }

        GUILayout.FlexibleSpace();

        // Status indicator
        GUILayout.Label(isPreviewPlaying ? "● Playing" : "○ Stopped", EditorStyles.miniLabel);

        GUILayout.EndHorizontal();
    }

    private void DrawToolbar()
    {
        GUILayout.BeginHorizontal(EditorStyles.toolbar);

        if (GUILayout.Button("New", EditorStyles.toolbarButton))
        {
            NewGraph();
        }

        if (GUILayout.Button("Save", EditorStyles.toolbarButton))
        {
            SaveGraph();
        }

        if (GUILayout.Button("Save As", EditorStyles.toolbarButton))
        {
            SaveGraph(null);
        }

        if (GUILayout.Button("Load", EditorStyles.toolbarButton))
        {
            LoadGraphFromFile();
        }

        // Graph asset field
        GUILayout.Space(10);
        GUILayout.Label("Active Graph:", GUILayout.Width(80));
        TweenGraph newGraphAsset = (TweenGraph)EditorGUILayout.ObjectField(activeGraphAsset, typeof(TweenGraph), false, GUILayout.Width(200));
        if (newGraphAsset != activeGraphAsset)
        {
            LoadGraphAsset(newGraphAsset);
        }

        GUILayout.FlexibleSpace();

        string fileName = string.IsNullOrEmpty(currentSavePath) ? "Unsaved Graph" : Path.GetFileName(currentSavePath);
        GUILayout.Label(fileName, EditorStyles.miniLabel);

        if (hasUnsavedChanges)
        {
            GUILayout.Label("●", EditorStyles.miniLabel);
        }

        GUILayout.EndHorizontal();
    }

    [MenuItem("Window/DOTween Node Editor %#t")]
    public static void OpenWindow()
    {
        NodeBasedTweenEditor window = GetWindow<NodeBasedTweenEditor>();
        window.titleContent = new GUIContent("DOTween Node Editor");
        window.minSize = new Vector2(800, 600);
    }

    [MenuItem("Assets/Create/DOTween/Node Graph")]
    private static void CreateNewGraphAsset()
    {
        TweenGraph graph = ScriptableObject.CreateInstance<TweenGraph>();

        string path = EditorUtility.SaveFilePanelInProject("Create Node Graph", "NewTweenSequence", "asset", "Save node graph");
        if (!string.IsNullOrEmpty(path))
        {
            AssetDatabase.CreateAsset(graph, path);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = graph;
        }
    }

    private void LoadNodeTextures()
    {
        defaultNodeTexture = LoadTexture("Assets/NodeTweener/Textures/NodeDefault.png");
        selectedNodeTexture = LoadTexture("Assets/NodeTweener/Textures/NodeSelected.png");

        if (defaultNodeTexture == null)
        {
            defaultNodeTexture = CreateFallbackTexture(Color.gray);
            Debug.LogWarning("Default node texture not found, using fallback");
        }

        if (selectedNodeTexture == null)
        {
            selectedNodeTexture = CreateFallbackTexture(Color.blue);
            Debug.LogWarning("Selected node texture not found, using fallback");
        }
    }

    private Texture2D LoadTexture(string path)
    {
        Texture2D texture = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
        if (texture == null)
        {
            Debug.LogWarning($"Could not load texture at path: {path}");
            return null;
        }
        return texture;
    }

    private Texture2D CreateFallbackTexture(Color color)
    {
        Texture2D texture = new Texture2D(64, 64);
        Color[] pixels = new Color[64 * 64];

        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = color;
        }

        texture.SetPixels(pixels);
        texture.Apply();
        return texture;
    }

    private void ReloadTextures()
    {
        LoadNodeTextures();
        CreateNodeStyles();
        Repaint();
    }

    [MenuItem("DOTween Node Editor/Reload Textures")]
    private static void ReloadTexturesMenu()
    {
        NodeBasedTweenEditor window = GetWindow<NodeBasedTweenEditor>();
        if (window != null)
        {
            window.ReloadTextures();
            Debug.Log("Node textures reloaded");
        }
    }


    #endregion

    #region Sequence Preview

    private void PreviewSequence()
    {
        if (previewTarget == null)
        {
            Debug.LogWarning("Please assign a Preview Target first!");
            return;
        }

        BuildAndShowSequence();
    }

    private void PlaySequence()
    {
        if (previewTarget == null)
        {
            Debug.LogWarning("Please assign a Preview Target first!");
            return;
        }

        if (previewSequence != null && previewSequence.IsPlaying())
        {
            previewSequence.Kill();
        }

        previewSequence = BuildSequence();
        if (previewSequence != null)
        {
            previewSequence.OnStart(() =>
            {
                isPreviewPlaying = true;
                Repaint();
            });

            previewSequence.OnComplete(() =>
            {
                isPreviewPlaying = false;
                Repaint();
            });

            previewSequence.Play();
        }
    }

    private void StopPreview()
    {
        if (previewSequence != null)
        {
            previewSequence.Kill();
            isPreviewPlaying = false;
            Repaint();
        }
    }

    private Sequence BuildSequence()
    {
        SyncGraphWithEditorNodes();

        if (currentGraph != null)
        {
            return currentGraph.BuildSequence(previewTarget);
        }

        return null;
    }

    private void BuildAndShowSequence()
    {
        Sequence sequence = BuildSequence();
        if (sequence != null)
        {
            string sequenceInfo = $"Sequence built with {currentGraph.nodes.Count} nodes:\n";

            foreach (var node in currentGraph.nodes)
            {
                sequenceInfo += $"- {node.name} ({node.GetType().Name})\n";
            }

            Debug.Log(sequenceInfo);
            ShowNotification(new GUIContent($"Sequence built with {currentGraph.nodes.Count} nodes"));
        }
        else
        {
            Debug.LogWarning("Failed to build sequence - no valid nodes found");
            ShowNotification(new GUIContent("Failed to build sequence"));
        }
    }

    private void DebugGraphSync()
    {
        Debug.Log($"=== GRAPH SYNC DEBUG ===");
        Debug.Log($"Editor Nodes: {editorNodes?.Count ?? 0}");
        Debug.Log($"Graph Nodes: {currentGraph?.nodes?.Count ?? 0}");

        if (editorNodes != null && currentGraph != null)
        {
            foreach (var editorNode in editorNodes)
            {
                bool inGraph = currentGraph.nodes.Contains(editorNode.node);
                Debug.Log($"Editor Node: {editorNode.node.name} - In Graph: {inGraph}");
            }
        }
        Debug.Log($"=== END SYNC DEBUG ===");
    }

    private void ValidateGraph()
    {
        DebugGraphSync();

        if (currentGraph != null)
        {
            if (currentGraph.ValidateGraph(out string errorMessage))
            {
                ShowNotification(new GUIContent("Graph is valid!"));
            }
            else
            {
                EditorUtility.DisplayDialog("Graph Validation", $"Graph validation failed:\n{errorMessage}", "OK");
            }
        }
    }

    private void SyncGraphWithEditorNodes()
    {
        if (currentGraph == null || editorNodes == null)
        {
            Debug.LogWarning("Cannot sync - currentGraph or editorNodes is null");
            return;
        }

        // Clean up any destroyed nodes first
        CleanupDestroyedNodes();

        // Validate connections
        ValidateConnections();

        // Sync nodes
        currentGraph.nodes.Clear();
        foreach (var editorNode in editorNodes)
        {
            if (editorNode?.node != null)
            {
                currentGraph.nodes.Add(editorNode.node);
            }
        }

        // Clear and rebuild node connections
        foreach (var editorNode in editorNodes)
        {
            if (editorNode?.node != null)
            {
                editorNode.node.inputs.Clear();
                editorNode.node.outputs.Clear();
            }
        }

        // Rebuild connections from visual connections
        foreach (var connection in connections)
        {
            if (connection?.outPoint?.node?.node != null &&
                connection?.inPoint?.node?.node != null)
            {
                connection.outPoint.node.node.outputs.Add(connection.inPoint.node.node);
                connection.inPoint.node.node.inputs.Add(connection.outPoint.node.node);
            }
        }

        // Mark as dirty if we have an asset path
        if (!string.IsNullOrEmpty(AssetDatabase.GetAssetPath(currentGraph)))
        {
            EditorUtility.SetDirty(currentGraph);
        }

        Debug.Log($"Synced: {currentGraph.nodes.Count} nodes in graph, {editorNodes.Count} editor nodes, {connections.Count} connections");
    }

    #endregion

    #region Asset Management


    private void CleanupTemporaryAssets()
    {
        if (editorNodes != null)
        {
            foreach (var editorNode in editorNodes)
            {
                if (editorNode?.node != null &&
                    (editorNode.node.hideFlags & HideFlags.DontSave) != 0)
                {
                    ScriptableObject.DestroyImmediate(editorNode.node, true);
                }
            }
        }
    }

    private void RebuildNodeReferences()
    {
        if (editorNodes != null)
        {
            foreach (var editorNode in editorNodes)
            {
                if (editorNode?.node == null)
                {
                }
            }
        }
        SyncGraphWithEditorNodes();
    }


    #endregion

    #region Compilation


    private void CheckCompilationStatus()
    {
        if (EditorApplication.isCompiling && !isCompiling)
        {
            isCompiling = true;
            OnCompilationStarted();
        }
        else if (!EditorApplication.isCompiling && isCompiling)
        {
            isCompiling = false;
            OnCompilationFinished();
        }
    }

    private void OnCompilationStarted()
    {
        Debug.Log("Compilation started - saving current state");
        // Save to EditorPrefs as backup during compilation
        // SaveToEditorPrefs();
    }

    private void OnCompilationFinished()
    {
        Debug.Log("Compilation finished - restoring graph");
        // Restore after compilation completes
        EditorApplication.delayCall += () =>
        {
            AutoRestoreGraph();
            Repaint();
        };
    }

    #endregion

    #region Utils

    private string ConvertValueToString(object value)
    {
        if (value == null) return null;

        try
        {
            if (value is Vector3 vector3)
            {
                return $"{vector3.x},{vector3.y},{vector3.z}";
            }
            else if (value is Vector2 vector2)
            {
                return $"{vector2.x},{vector2.y}";
            }
            else if (value is Color color)
            {
                return ColorUtility.ToHtmlStringRGBA(color);
            }
            else if (value is Enum)
            {
                return value.ToString();
            }
            else if (value is bool boolValue)
            {
                return boolValue.ToString();
            }
            else if (value is float floatValue)
            {
                return floatValue.ToString("F6");
            }
            else
            {
                return value.ToString();
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning($"Failed to convert value to string: {e.Message}");
            return null;
        }
    }

    private object ConvertStringToValue(string stringValue, Type targetType)
    {
        if (string.IsNullOrEmpty(stringValue)) return null;

        try
        {
            if (targetType == typeof(Vector3))
            {
                string[] parts = stringValue.Split(',');
                if (parts.Length == 3)
                {
                    return new Vector3(
                        float.Parse(parts[0]),
                        float.Parse(parts[1]),
                        float.Parse(parts[2])
                    );
                }
            }
            else if (targetType == typeof(Vector2))
            {
                string[] parts = stringValue.Split(',');
                if (parts.Length == 2)
                {
                    return new Vector2(
                        float.Parse(parts[0]),
                        float.Parse(parts[1])
                    );
                }
            }
            else if (targetType == typeof(Color))
            {
                Color color;
                if (ColorUtility.TryParseHtmlString("#" + stringValue, out color))
                    return color;
            }
            else if (targetType == typeof(float))
            {
                return float.Parse(stringValue);
            }
            else if (targetType == typeof(int))
            {
                return int.Parse(stringValue);
            }
            else if (targetType == typeof(bool))
            {
                return bool.Parse(stringValue);
            }
            else if (targetType == typeof(string))
            {
                return stringValue;
            }
            else if (targetType.IsEnum)
            {
                return Enum.Parse(targetType, stringValue);
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning($"Failed to convert '{stringValue}' to {targetType.Name}: {e.Message}");
        }

        return null;
    }

    #endregion

}


#endif