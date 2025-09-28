using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;
using System.IO;
using DG.Tweening;


public class NodeBasedTweenEditor : EditorWindow
{
    [SerializeField] private GameObject previewTarget;
    private Sequence previewSequence;
    private bool isPreviewPlaying = false;

    private TweenGraph currentGraph;
    private string currentSavePath;

    private new bool hasUnsavedChanges = false;
    private Vector2 lastSaveCanvasState;

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
    private Vector2 lastMousePosition;
    private Dictionary<Connection, Rect> connectionBoundsCache = new Dictionary<Connection, Rect>();

    private Dictionary<Color, Texture2D> nodeTextures = new Dictionary<Color, Texture2D>();
    private Color currentNodeColor = new Color(0.3f, 0.3f, 0.3f);
    private bool stylesInitialized = false;

    private double lastAutoSaveTime;
    private const double AUTO_SAVE_INTERVAL = 300;

    private const string EDITOR_PREFS_KEY = "DOTweenNodeEditor_GraphData";


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

    #endregion

    #region Draw

    private void OnGUI()
    {

        if (!stylesInitialized)
        {
            InitializeStyles();
        }

        Rect backgroundRect = new Rect(0, 0, position.width, position.height);
        EditorGUI.DrawRect(backgroundRect, HexColorUtility.ParseHex("#2d232e"));

        DrawGrid(20, 0.05f, HexColorUtility.ParseHex("#594e36"));
        DrawGrid(100, 0.1f, HexColorUtility.ParseHex("#594e36"));

        DrawToolbar();
        DrawExecutionToolbar();

        Event currentEvent = Event.current;
        bool shouldCheckHover = currentEvent.type == EventType.MouseMove ||
                               currentEvent.type == EventType.MouseDrag ||
                               currentEvent.type == EventType.Layout ||
                               (currentEvent.type == EventType.Repaint && currentEvent.mousePosition != lastMousePosition);

        if (currentEvent.type == EventType.MouseDrag)
        {
            UpdateConnectionBoundsCache();
        }

        if (shouldCheckHover)
        {
            Connection newHovered = GetConnectionAtPosition(currentEvent.mousePosition);
            if (newHovered != hoveredConnection)
            {
                hoveredConnection = newHovered;
                Repaint();
            }
            lastMousePosition = currentEvent.mousePosition;
        }

        DrawConnections();
        DrawConnectionLine(Event.current);

        DrawNodes();


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
        if (editorNodes != null)
        {
            for (int i = 0; i < editorNodes.Count; i++)
            {
                editorNodes[i].Draw();
            }
        }
    }

    private void DrawConnections()
    {
        if (connections != null)
        {
            for (int i = 0; i < connections.Count; i++)
            {
                bool isHovered = connections[i] == hoveredConnection;
                connections[i].Draw(isHovered);
            }
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

    private void CreateNodeStyles()
    {
        nodeStyle = new GUIStyle();
        nodeStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node1.png") as Texture2D;
        nodeStyle.normal.textColor = GetContrastColor(HexColorUtility.ParseHex("#474448"));
        nodeStyle.border = new RectOffset(12, 12, 12, 12);
        nodeStyle.padding = new RectOffset(10, 10, 10, 10);
        nodeStyle.alignment = TextAnchor.UpperCenter;
        nodeStyle.fontStyle = FontStyle.Bold;

        selectedNodeStyle = new GUIStyle();
        selectedNodeStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node1 on.png") as Texture2D;
        selectedNodeStyle.normal.textColor = GetContrastColor(HexColorUtility.ParseHex("#534b52"));
        selectedNodeStyle.border = new RectOffset(12, 12, 12, 12);
        selectedNodeStyle.padding = new RectOffset(10, 10, 10, 10);
        selectedNodeStyle.alignment = TextAnchor.UpperCenter;
        selectedNodeStyle.fontStyle = FontStyle.Bold;
    }


    public void SetNodeColor(Color newColor)
    {
        currentNodeColor = newColor;
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
                    EditorNode clickedNode = GetNodeAtPosition(e.mousePosition);
                    if (clickedNode == null)
                    {
                        ProcessContextMenu(e.mousePosition);

                    }
                }

                if (e.button == 0)
                {
                    if (hoveredConnection != null)
                    {
                        RemoveConnection(hoveredConnection);
                    }
                }
                break;

            case EventType.MouseDrag:
                if (e.button == 0)
                {
                    OnDrag(e.delta);
                }
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
                    LoadGraph();
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
                bool guiChanged = editorNodes[i].ProcessEvents(e);
                if (guiChanged)
                {
                    GUI.changed = true;
                }
            }
        }
    }

    private void ProcessContextMenu(Vector2 mousePosition)
    {
        GenericMenu genericMenu = new GenericMenu();
        genericMenu.AddItem(new GUIContent("Add Move Node"), false, () => OnClickAddNode(mousePosition, typeof(MoveNode)));
        genericMenu.AddItem(new GUIContent("Add Scale Node"), false, () => OnClickAddNode(mousePosition, typeof(ScaleNode)));
        genericMenu.AddItem(new GUIContent("Add Wait Node"), false, () => OnClickAddNode(mousePosition, typeof(WaitNode)));
        genericMenu.ShowAsContext();
    }

    #endregion

    #region Save/Load

    private void SaveGraph(string path = null)
    {
        try
        {
            string savePath = path ?? currentSavePath;

            if (string.IsNullOrEmpty(savePath))
            {
                savePath = EditorUtility.SaveFilePanel("Save Node Graph", "Assets", "NewTweenSequence", "asset");
                if (string.IsNullOrEmpty(savePath)) return;

                // Convert to project-relative path
                savePath = "Assets" + savePath.Substring(Application.dataPath.Length);
            }

            NodeGraphSaveData saveData = CreateSaveData();

            // Save as ScriptableObject
            currentGraph.saveData = saveData;
            currentGraph.name = Path.GetFileNameWithoutExtension(savePath);

            if (!string.IsNullOrEmpty(AssetDatabase.GetAssetPath(currentGraph)))
            {
                // Asset already exists, just save it
                EditorUtility.SetDirty(currentGraph);
            }
            else
            {
                // Create new asset
                AssetDatabase.CreateAsset(currentGraph, savePath);
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            currentSavePath = savePath;
            hasUnsavedChanges = false;

            NodeBasedTweenEditor window = GetWindow<NodeBasedTweenEditor>();
            window.titleContent = new GUIContent("DOTween Node Editor");

            Debug.Log($"Graph saved to: {savePath}");
            ShowNotification(new GUIContent("Graph Saved Successfully"));
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to save graph: {e.Message}");
            ShowNotification(new GUIContent("Save Failed!"));
        }
    }

    private void LoadGraph()
    {
        try
        {
            string loadPath = EditorUtility.OpenFilePanel("Load Node Graph", "Assets", "asset");
            if (string.IsNullOrEmpty(loadPath)) return;

            // Convert to project-relative path
            loadPath = "Assets" + loadPath.Substring(Application.dataPath.Length);

            TweenGraph loadedGraph = AssetDatabase.LoadAssetAtPath<TweenGraph>(loadPath);
            if (loadedGraph == null)
            {
                Debug.LogError("Failed to load graph asset");
                return;
            }

            currentGraph = loadedGraph;
            currentSavePath = loadPath;
            LoadFromSaveData(currentGraph.saveData);
            hasUnsavedChanges = false;

            Debug.Log($"Graph loaded from: {loadPath}");
            ShowNotification(new GUIContent("Graph Loaded Successfully"));
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to load graph: {e.Message}");
            ShowNotification(new GUIContent("Load Failed!"));
        }
    }

    private void NewGraph()
    {
        if (hasUnsavedChanges)
        {
            if (!EditorUtility.DisplayDialog("Unsaved Changes",
                "You have unsaved changes. Create new graph anyway?", "Yes", "No"))
            {
                return;
            }
        }

        currentGraph = ScriptableObject.CreateInstance<TweenGraph>();
        editorNodes.Clear();
        connections.Clear();
        currentSavePath = null;
        hasUnsavedChanges = false;

        EditorPrefs.DeleteKey(EDITOR_PREFS_KEY);
        EditorPrefs.DeleteKey(EDITOR_PREFS_KEY + "_Path");

        Repaint();
        ShowNotification(new GUIContent("New Graph Created"));
    }

    private NodeGraphSaveData CreateSaveData()
    {
        NodeGraphSaveData saveData = new NodeGraphSaveData();
        saveData.canvasOffset = offset;

        // Save nodes
        foreach (var editorNode in editorNodes)
        {
            NodeSaveData nodeData = new NodeSaveData
            {
                nodeType = editorNode.node.GetType().Name,
                nodeName = editorNode.node.name,
                position = editorNode.rect.position,
                guid = editorNode.guid
            };

            // Save node-specific data
            if (editorNode.node is MoveNode moveNode)
            {
                nodeData.targetPosition = moveNode.targetPosition;
                nodeData.duration = moveNode.duration;
            }
            else if (editorNode.node is ScaleNode scaleNode)
            {
                nodeData.targetScale = scaleNode.targetScale;
                nodeData.duration = scaleNode.duration;
            }
            else if (editorNode.node is WaitNode waitNode)
            {
                nodeData.waitTime = waitNode.waitTime;
            }

            saveData.nodes.Add(nodeData);
        }

        // Save connections
        foreach (var connection in connections)
        {
            if (connection.outPoint?.node != null && connection.inPoint?.node != null)
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

    private void LoadFromSaveData(NodeGraphSaveData saveData)
    {
        if (saveData == null) return;

        editorNodes.Clear();
        connections.Clear();
        offset = saveData.canvasOffset;

        Dictionary<string, EditorNode> guidToNodeMap = new Dictionary<string, EditorNode>();

        foreach (var nodeData in saveData.nodes)
        {
            System.Type nodeType = GetNodeTypeFromName(nodeData.nodeType);
            if (nodeType != null)
            {
                TweenNode nodeAsset = CreateNodeAsset(nodeType);


                EditorNode newNode = new EditorNode(
                    nodeData.position, 200, 100, GetVisualNodeType(nodeType), nodeStyle, selectedNodeStyle, inPointStyle, outPointStyle,
                    OnClickInPoint, OnClickOutPoint, OnClickRemoveNode, OnClickSelectNode, nodeType, nodeAsset
                );

                newNode.guid = nodeData.guid;
                newNode.node.name = nodeData.nodeName;

                if (newNode.node is MoveNode moveNode)
                {
                    moveNode.targetPosition = nodeData.targetPosition;
                    moveNode.duration = nodeData.duration;
                }
                else if (newNode.node is ScaleNode scaleNode)
                {
                    scaleNode.targetScale = nodeData.targetScale;
                    scaleNode.duration = nodeData.duration;
                }
                else if (newNode.node is WaitNode waitNode)
                {
                    waitNode.waitTime = nodeData.waitTime;
                }

                editorNodes.Add(newNode);
                guidToNodeMap[nodeData.guid] = newNode;
            }
        }

        foreach (var connectionData in saveData.connections)
        {
            if (guidToNodeMap.TryGetValue(connectionData.fromNodeGuid, out EditorNode fromNode) &&
                guidToNodeMap.TryGetValue(connectionData.toNodeGuid, out EditorNode toNode))
            {
                connections.Add(new Connection(toNode.inPoint, fromNode.outPoint));

                fromNode.node.outputs.Add(toNode.node);
                toNode.node.inputs.Add(fromNode.node);
            }
        }

        SyncGraphWithEditorNodes();
        Repaint();
    }

    private System.Type GetNodeTypeFromName(string typeName)
    {
        return typeName switch
        {
            "MoveNode" => typeof(MoveNode),
            "ScaleNode" => typeof(ScaleNode),
            "WaitNode" => typeof(WaitNode),
            _ => null
        };
    }

    private void MarkUnsavedChanges()
    {
        hasUnsavedChanges = true;
        titleContent = new GUIContent("DOTween Node Editor *");
    }

    private NodeType GetVisualNodeType(Type type)
    {
        if (type == typeof(MoveNode))
        {
            return NodeType.Move;
        }
        else if (type == typeof(ScaleNode))
        {
            return NodeType.Scale;
        }
        else if (type == typeof(WaitNode))
        {
            return NodeType.Wait;
        }

        return NodeType.None;

    }

    private void SaveToEditorPrefs()
    {
        try
        {
            NodeGraphSaveData saveData = CreateSaveData();
            string json = JsonUtility.ToJson(saveData);
            EditorPrefs.SetString(EDITOR_PREFS_KEY, json);
            EditorPrefs.SetString(EDITOR_PREFS_KEY + "_Path", currentSavePath);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to save to EditorPrefs: {e.Message}");
        }
    }

    private void LoadFromEditorPrefs()
    {
        try
        {
            if (EditorPrefs.HasKey(EDITOR_PREFS_KEY))
            {
                string json = EditorPrefs.GetString(EDITOR_PREFS_KEY);
                NodeGraphSaveData saveData = JsonUtility.FromJson<NodeGraphSaveData>(json);
                LoadFromSaveData(saveData);
                currentSavePath = EditorPrefs.GetString(EDITOR_PREFS_KEY + "_Path");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to load from EditorPrefs: {e.Message}");
            // Clear corrupted data
            EditorPrefs.DeleteKey(EDITOR_PREFS_KEY);
            EditorPrefs.DeleteKey(EDITOR_PREFS_KEY + "_Path");
        }
    }


    #endregion

    #region Event Handlers

    private void OnEnable()
    {
        nodeStyle = new GUIStyle();
        nodeStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node1.png") as Texture2D;
        nodeStyle.border = new RectOffset(12, 12, 12, 12);

        selectedNodeStyle = new GUIStyle();
        selectedNodeStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node1 on.png") as Texture2D;
        selectedNodeStyle.border = new RectOffset(12, 12, 12, 12);

        inPointStyle = new GUIStyle();
        inPointStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn left.png") as Texture2D;
        inPointStyle.active.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn left on.png") as Texture2D;
        inPointStyle.border = new RectOffset(4, 4, 12, 12);

        outPointStyle = new GUIStyle();
        outPointStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn right.png") as Texture2D;
        outPointStyle.active.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn right on.png") as Texture2D;
        outPointStyle.border = new RectOffset(4, 4, 12, 12);


        EditorApplication.update += OnEditorUpdate;

        CreateNodeStyles();

        if (currentGraph == null)
        {
            currentGraph = ScriptableObject.CreateInstance<TweenGraph>();
        }

        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;

        // Initialize styles
        InitializeStyles();

        LoadFromEditorPrefs();
    }

    private void OnDisable()
    {
        EditorApplication.update -= OnEditorUpdate;
        EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
        SaveToEditorPrefs();
        StopPreview();
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

    private void OnPlayModeStateChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.ExitingPlayMode)
        {
            StopPreview();
        }
        else if (state == PlayModeStateChange.EnteredEditMode)
        {
            stylesInitialized = false;
            InitializeStyles();
            Repaint();
        }
    }

    private void Update()
    {
        // Auto-save functionality
        if (hasUnsavedChanges && EditorApplication.timeSinceStartup - lastAutoSaveTime > AUTO_SAVE_INTERVAL)
        {
            if (!string.IsNullOrEmpty(currentSavePath))
            {
                SaveGraph(currentSavePath);
                lastAutoSaveTime = EditorApplication.timeSinceStartup;
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
        if (editorNodes == null)
        {
            editorNodes = new List<EditorNode>();
        }

        NodeType visualNodeType = GetVisualNodeType(nodeType);

        TweenNode nodeAsset = CreateNodeAsset(nodeType);

        EditorNode editorNode = new EditorNode(mousePosition, 200, 100, visualNodeType, nodeStyle, selectedNodeStyle, inPointStyle, outPointStyle,
            OnClickInPoint, OnClickOutPoint, OnClickRemoveNode, OnClickSelectNode, nodeType, nodeAsset);

        editorNodes.Add(editorNode);
        SyncGraphWithEditorNodes();
        MarkUnsavedChanges();
    }



    private void OnClickInPoint(ConnectionPoint inPoint)
    {
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
        MarkUnsavedChanges();


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
        MarkUnsavedChanges();


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

        if (connectionBoundsCache.Count != connections.Count)
            UpdateConnectionBoundsCache();

        for (int i = connections.Count - 1; i >= 0; i--)
        {
            var connection = connections[i];
            if (connectionBoundsCache.TryGetValue(connection, out Rect bounds) && bounds.Contains(position))
            {
                if (connection.IsMouseOver(position))
                    return connection;
            }
        }

        return null;
    }

    private bool IsMouseOverConnection(Vector2 position)
    {
        return GetConnectionAtPosition(position) != null;
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
        MarkUnsavedChanges();

    }

    private void UpdateConnectionBoundsCache()
    {
        if (connections == null) return;

        connectionBoundsCache.Clear();
        foreach (var connection in connections)
        {
            if (connection?.inPoint != null && connection?.outPoint != null)
            {
                Vector2 start = connection.inPoint.rect.center;
                Vector2 end = connection.outPoint.rect.center;
                connectionBoundsCache[connection] = new Rect(
                    Mathf.Min(start.x, end.x) - 12f,
                    Mathf.Min(start.y, end.y) - 12f,
                    Mathf.Abs(end.x - start.x) + 24f,
                    Mathf.Abs(end.y - start.y) + 24f
                );
            }
        }
    }

    private void OnConnectionsChanged()
    {
        UpdateConnectionBoundsCache();
        Repaint();
    }


    #endregion

    #region Nodes


    private TweenNode CreateNodeAsset(System.Type nodeType)
    {
        TweenNode node = ScriptableObject.CreateInstance(nodeType) as TweenNode;
        node.name = nodeType.Name;

        if (currentGraph != null && !string.IsNullOrEmpty(AssetDatabase.GetAssetPath(currentGraph)))
        {
            AssetDatabase.AddObjectToAsset(node, currentGraph);
            AssetDatabase.SaveAssets();
        }

        return node;
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
            SaveGraph(null); // Force save dialog
        }

        if (GUILayout.Button("Load", EditorStyles.toolbarButton))
        {
            LoadGraph();
        }

        GUILayout.FlexibleSpace();

        string fileName = string.IsNullOrEmpty(currentSavePath) ? "Unsaved Graph" : Path.GetFileName(currentSavePath);
        GUILayout.Label(fileName, EditorStyles.miniLabel);

        if (hasUnsavedChanges)
        {
            GUILayout.Label("●", EditorStyles.miniLabel); // Dot indicator for unsaved changes
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
        if (currentGraph != null)
        {
            currentGraph.SyncWithEditorNodes(editorNodes);
        }
    }

    #endregion

}
