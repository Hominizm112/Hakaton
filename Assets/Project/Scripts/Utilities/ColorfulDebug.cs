using UnityEngine;

public static class ColorfulDebug
{
    public static void LogRed(string message)
    {
        Debug.Log($"<color=#a6334c>{message}</color>");
    }

    public static void LogGreen(string message)
    {
        Debug.Log($"<color=#50a633>{message}</color>");
    }

    public static void LogBlue(string message)
    {
        Debug.Log($"<color=#4196ae>{message}</color>");
    }

    public static void LogYellow(string message)
    {
        Debug.Log($"<color=#b6b321>{message}</color>");
    }

    public static void LogOrange(string message)
    {
        Debug.Log($"<color=#a66533>{message}</color>");
    }

    public static void LogCustom(string message, string color)
    {
        Debug.Log($"<color={color}>{message}</color>");
    }

    public static void LogWarning(string message)
    {
        Debug.LogWarning($"<color=#a66533>{message}</color>");
    }

    public static void LogError(string message)
    {
        Debug.LogError($"<color=#a6334c>{message}</color>");
    }
}
