using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;
using System.Reflection;
using System.Linq;

public partial class ConsoleService : MonoService
{
    public override List<Type> requiredServices { get; protected set; } = new() { typeof(InputManager) };

    [Header("UI References")]
    [SerializeField] private GameObject consoleObject;
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private TMP_Text outputText;
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private int maxOutputLines;

    [Header("Settings")]
    [SerializeField] private bool enableCheats = true;
    [SerializeField] private bool logToDebug = true;

    private bool _consoleOppened;
    private Mediator _mediator;
    private InputAction _toggleAction;
    private readonly List<string> _commandHistory = new();
    private int _historyIndex = -1;
    private string _currentInput = "";

    private readonly Dictionary<string, MethodInfo> _commands = new();
    private readonly Dictionary<string, ConsoleCommandAttribute> _commandAttributes = new();
    private readonly List<string> _outputLines = new();

    protected override void OnAllServicesReady()
    {
        _mediator = Mediator.Instance;
        _mediator.GetService<InputManager>().TryGetAction("SwitchDebugConsole", out _toggleAction);

        _toggleAction.performed += OnInputAction;

        inputField.onSubmit.AddListener(HandleInput);
        inputField.onValueChanged.AddListener(OnInputChanged);

        DiscoverCommands();
        AddOutput("Developer Console Ready. Type 'help' for avaliable commands.");

        SetupInputNavigation();
    }

    private void SetupInputNavigation()
    {
        inputField.onSelect.AddListener(_ => _currentInput = inputField.text);
    }

    private void Update()
    {
        if (!_consoleOppened) return;

        HandleNavigationInput();
    }

    private void HandleNavigationInput()
    {
        if (Keyboard.current.upArrowKey.wasPressedThisFrame)
        {
            NavigateHistory(1);
        }
        else if (Keyboard.current.downArrowKey.wasPressedThisFrame)
        {
            NavigateHistory(-1);
        }
        else if (Keyboard.current.tabKey.wasPressedThisFrame)
        {
            AutoComplete();
        }
    }

    private void NavigateHistory(int direction)
    {
        if (_commandHistory.Count == 0) return;

        _historyIndex = Mathf.Clamp(_historyIndex + direction, -1, _commandHistory.Count - 1);

        if (_historyIndex == -1)
        {
            inputField.text = _currentInput;
        }
        else
        {
            inputField.text = _commandHistory[_historyIndex];
            inputField.caretPosition = inputField.text.Length;
        }

        inputField.Select();
        inputField.ActivateInputField();
    }

    private void AutoComplete()
    {
        string partial = inputField.text.Trim();
        if (string.IsNullOrEmpty(partial)) return;

        var matches = _commands.Keys.Where(cmd => cmd.StartsWith(partial, StringComparison.OrdinalIgnoreCase)).ToList();

        if (matches.Count == 1)
        {
            inputField.text = matches[0] + " ";
            inputField.caretPosition = inputField.text.Length;
        }
        else if (matches.Count > 1)
        {
            AddOutput($"Possible completions: {string.Join(", ", matches)}");
        }

        inputField.Select();
        inputField.ActivateInputField();
    }

    private void OnInputChanged(string value)
    {
        if (!inputField.isFocused) return;
        _currentInput = value;
    }

    private void OnInputAction(InputAction.CallbackContext ctx)
    {
        _consoleOppened = !_consoleOppened;
        consoleObject.SetActive(_consoleOppened);

        if (_consoleOppened)
        {
            inputField.Select();
            inputField.ActivateInputField();
        }
    }

    private void HandleInput(string input)
    {
        if (string.IsNullOrEmpty(input)) return;

        AddOutput($"> {input}");
        ParseCommand(input.Trim());

        if (_commandHistory.Count == 0 || _commandHistory[0] != input)
        {
            _commandHistory.Insert(0, input);
            if (_commandHistory.Count > 50) _commandHistory.RemoveAt(50);
        }

        _historyIndex = -1;
        inputField.text = "";
        inputField.Select();
        inputField.ActivateInputField();
    }

    private void ParseCommand(string command)
    {
        try
        {
            string[] parts = command.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 0) return;

            string commandName = parts[0].ToLower();
            string[] args = parts.Length > 1 ? parts.Skip(1).ToArray() : new string[0];

            if (_commands.TryGetValue(commandName, out MethodInfo method))
            {
                ExecuteCommand(method, args);
            }
            else
            {
                AddOutput($"Unknown command: {commandName}. Type 'help' for avaliable commands.");
            }
        }
        catch (Exception e)
        {
            AddOutput($"Error parsing command: {e.Message}");
        }
    }

    private void ExecuteCommand(MethodInfo method, string[] args)
    {
        try
        {
            var context = new CommandContext(this, args);
            var result = method.Invoke(this, new object[] { context }) as CommandResult?;

            if (result.HasValue)
            {
                if (!string.IsNullOrEmpty(result.Value.Message))
                {
                    AddOutput(result.Value.Message);
                }

                if (!result.Value.Succes)
                {
                    AddOutput($"Command failed: {result.Value.Message}");
                }
            }
        }
        catch (Exception e)
        {
            AddOutput($"Error executing command: {e.Message}");
            if (logToDebug) Debug.LogError($"Console command error: {e}");
        }
    }

    #region Command Discovery

    private void DiscoverCommands()
    {
        _commands.Clear();
        _commandAttributes.Clear();

        var methods = GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
            .Where(m => m.GetCustomAttribute<ConsoleCommandAttribute>() != null);

        foreach (var method in methods)
        {
            var attribute = method.GetCustomAttribute<ConsoleCommandAttribute>();
            if (attribute != null)
            {
                string commandName = attribute.Name.ToLower();
                _commands[commandName] = method;
                _commandAttributes[commandName] = attribute;
            }
        }
    }

    #endregion

    #region Output Management

    public void AddOutput(string message)
    {
        _outputLines.Add(message);

        while (_outputLines.Count > maxOutputLines)
        {
            _outputLines.RemoveAt(0);
        }

        UpdateOutputDisplay();

        if (logToDebug)
        {
            Debug.Log($"[Console] {message}");
        }
    }

    private void UpdateOutputDisplay()
    {
        outputText.text = string.Join("\n", _outputLines);

        Canvas.ForceUpdateCanvases();
        scrollRect.verticalNormalizedPosition = 0f;
        Canvas.ForceUpdateCanvases();
    }

    public void ClearOutput()
    {
        _outputLines.Clear();
        UpdateOutputDisplay();
    }

    #endregion

    #region Built-in Commands

    [ConsoleCommand("help", "Shows avaliable commands", "help [command]")]
    private CommandResult HelpCommand(CommandContext context)
    {
        if (context.ArgumentCount > 0)
        {
            string commandName = context.GetString(0).ToLower();
            if (_commandAttributes.TryGetValue(commandName, out var attribute))
            {
                return CommandResult.Ok($"{attribute.Name}: {attribute.Description}\nUsage: {attribute.Usage}");
            }
            return CommandResult.Error($"Unknown command: {commandName}");
        }

        var output = "Avaliable commands:\n";

        foreach (var kvp in _commandAttributes.OrderBy(k => k.Key))
        {
            output += $"{kvp.Key}: {kvp.Value.Description}\n";
        }
        output += "Use 'help <command>' for more info";

        return CommandResult.Ok(output);
    }

    [ConsoleCommand("clear", "Clears console output", "clear")]
    private CommandResult ClearCommand(CommandContext context)
    {
        ClearOutput();
        return CommandResult.Ok("Console cleared");
    }

    [ConsoleCommand("echo", "Echoes text back", "echo <message>")]
    private CommandResult EchoCommand(CommandContext context)
    {
        if (context.ArgumentCount == 0)
            return CommandResult.Error("Usage: echo <message>");

        return CommandResult.Ok(string.Join(" ", context.Arguments));
    }

    [ConsoleCommand("time", "Shows or sets game time scale", "time [scale]")]
    private CommandResult TimeCommand(CommandContext context)
    {
        if (context.ArgumentCount == 0)
        {
            return CommandResult.Ok($"Time scale: {Time.timeScale}");
        }

        float scale = context.GetFloat(0, 1f);
        Time.timeScale = Mathf.Clamp(scale, 0f, 100f);
        return CommandResult.Ok($"Time scale set to: {Time.timeScale}");
    }

    [ConsoleCommand("fps", "Shows FPS information", "fps")]
    private CommandResult FpsCommand(CommandContext context)
    {
        float fps = 1f / Time.unscaledDeltaTime;
        return CommandResult.Ok($"FPS: {fps:0.0} ({Time.unscaledDeltaTime * 1000:0.0}ms)");
    }
    #endregion

    private void OnDestroy()
    {
        if (_toggleAction != null)
        {
            _toggleAction.performed -= OnInputAction;
        }

        if (inputField != null)
        {
            inputField.onSubmit.RemoveListener(HandleInput);
            inputField.onValueChanged.RemoveListener(OnInputChanged);
        }
    }
}