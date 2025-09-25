using System;

[AttributeUsage(AttributeTargets.Method)]
public class ConsoleCommandAttribute : Attribute
{
    public string Name { get; }
    public string Description { get; }
    public string Usage { get; }

    public ConsoleCommandAttribute(string name, string description = "", string usage = "")
    {
        Name = name;
        Description = description;
        Usage = usage;
    }
}