public struct CommandResult
{
    public bool Succes { get; }
    public string Message { get; }

    public CommandResult(bool success, string message = "")
    {
        Succes = success;
        Message = message;
    }

    public static CommandResult Ok(string message = "") => new CommandResult(true, message);
    public static CommandResult Error(string message) => new CommandResult(false, message);
}

public class CommandContext
{
    public ConsoleService Console { get; }
    public string[] Arguments { get; }
    public int ArgumentCount => Arguments.Length;

    public CommandContext(ConsoleService console, string[] arguments)
    {
        Console = console;
        Arguments = arguments;
    }

    public string GetString(int index, string defaultValue = "") =>
        index < Arguments.Length ? Arguments[index] : defaultValue;

    public int GetInt(int index, int defaultValue = 0) =>
        int.TryParse(GetString(index), out int result) ? result : defaultValue;

    public float GetFloat(int index, float defaultValue = 0f) =>
        float.TryParse(GetString(index), out float result) ? result : defaultValue;

    public bool GetBool(int index, bool defaultValue = false)
    {
        string arg = GetString(index).ToLower();
        return arg switch
        {
            "true" or "1" or "yes" => true,
            "false" or "0" or "no" => false,
            _ => defaultValue
        };
    }
}