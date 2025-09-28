using System;
using UnityEngine;

public static class HexColorUtility
{
    /// <summary>
    /// Parses a hex color string to Unity Color
    /// Supports formats: #RGB, #RGBA, #RRGGBB, #RRGGBBAA
    /// </summary>
    /// <param name="hexColor">Hex color string with or without #</param>
    /// <returns>Unity Color object</returns>
    public static Color ParseHex(string hexColor)
    {
        if (string.IsNullOrEmpty(hexColor))
        {
            Debug.LogWarning("Hex color string is null or empty");
            return Color.white;
        }

        // Remove # if present
        string hex = hexColor.Trim().Replace("#", "");

        try
        {
            switch (hex.Length)
            {
                case 3: // RGB
                    return ParseRGB(hex);
                case 4: // RGBA
                    return ParseRGBA(hex);
                case 6: // RRGGBB
                    return ParseRRGGBB(hex);
                case 8: // RRGGBBAA
                    return ParseRRGGBBAA(hex);
                default:
                    Debug.LogWarning($"Invalid hex color format: {hexColor}. Supported formats: #RGB, #RGBA, #RRGGBB, #RRGGBBAA");
                    return Color.white;
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to parse hex color '{hexColor}': {e.Message}");
            return Color.white;
        }
    }

    private static Color ParseRGB(string hex)
    {
        float r = HexToFloat(hex[0]) * 16 + HexToFloat(hex[0]);
        float g = HexToFloat(hex[1]) * 16 + HexToFloat(hex[1]);
        float b = HexToFloat(hex[2]) * 16 + HexToFloat(hex[2]);
        return new Color(r / 255f, g / 255f, b / 255f, 1f);
    }

    private static Color ParseRGBA(string hex)
    {
        float r = HexToFloat(hex[0]) * 16 + HexToFloat(hex[0]);
        float g = HexToFloat(hex[1]) * 16 + HexToFloat(hex[1]);
        float b = HexToFloat(hex[2]) * 16 + HexToFloat(hex[2]);
        float a = HexToFloat(hex[3]) * 16 + HexToFloat(hex[3]);
        return new Color(r / 255f, g / 255f, b / 255f, a / 255f);
    }

    private static Color ParseRRGGBB(string hex)
    {
        float r = HexToFloat(hex[0]) * 16 + HexToFloat(hex[1]);
        float g = HexToFloat(hex[2]) * 16 + HexToFloat(hex[3]);
        float b = HexToFloat(hex[4]) * 16 + HexToFloat(hex[5]);
        return new Color(r / 255f, g / 255f, b / 255f, 1f);
    }

    private static Color ParseRRGGBBAA(string hex)
    {
        float r = HexToFloat(hex[0]) * 16 + HexToFloat(hex[1]);
        float g = HexToFloat(hex[2]) * 16 + HexToFloat(hex[3]);
        float b = HexToFloat(hex[4]) * 16 + HexToFloat(hex[5]);
        float a = HexToFloat(hex[6]) * 16 + HexToFloat(hex[7]);
        return new Color(r / 255f, g / 255f, b / 255f, a / 255f);
    }

    private static int HexToFloat(char hexChar)
    {
        return hexChar switch
        {
            '0' => 0,
            '1' => 1,
            '2' => 2,
            '3' => 3,
            '4' => 4,
            '5' => 5,
            '6' => 6,
            '7' => 7,
            '8' => 8,
            '9' => 9,
            'A' or 'a' => 10,
            'B' or 'b' => 11,
            'C' or 'c' => 12,
            'D' or 'd' => 13,
            'E' or 'e' => 14,
            'F' or 'f' => 15,
            _ => throw new ArgumentException($"Invalid hex character: {hexChar}")
        };
    }
}
