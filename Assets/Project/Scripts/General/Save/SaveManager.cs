using UnityEngine;
using System;
using System.Security.Cryptography;
using System.Collections;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;


/// <summary>
/// A cross-platform save manager that handles both desktop and WebGL.
/// Uses JSON serialization and async file operations for compatibility.
/// </summary>
public partial class SaveManager : EventListener
{
    public static event Action OnSaveLoaded;
    public static event Action OnSaveCompleted;

    private const string SAVE_FILE_NAME = "savegame.json";
    private string SaveFilePath => Path.Combine(Application.persistentDataPath, SAVE_FILE_NAME);


    private JsonSerializerSettings jsonSettings = new JsonSerializerSettings
    {
        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
        NullValueHandling = NullValueHandling.Ignore,
        DefaultValueHandling = DefaultValueHandling.Ignore,
        Formatting = Formatting.Indented
    };

    public SaveData currentSaveData;

    /// <summary>
    /// Call this early (e.g., from Bootstrap) to load saved data.
    /// For WebGL, this must be asynchronous.
    /// </summary>
    public async Task LoadDataAsync()
    {
        await LoadSaveDataRoutineAsync();
    }

    private async Task LoadSaveDataRoutineAsync()
    {
        if (!File.Exists(SaveFilePath))
        {
            Debug.Log("No save file found. Using default data");
            currentSaveData = new SaveData();
            OnSaveLoaded?.Invoke();
            return;
        }

        string filePath = SaveFilePath;
#if UNITY_WEBGL
        filePath = "file:///" + SaveFilePath;
#endif


        string json;

        try
        {
            if (filePath.StartsWith("file:///"))
            {
                var loadOperation = UnityEngine.Networking.UnityWebRequest.Get(filePath);
                await loadOperation.SendWebRequest();

                if (loadOperation.result == UnityEngine.Networking.UnityWebRequest.Result.Success)
                {
                    json = loadOperation.downloadHandler.text.Trim('\uFEFF', '\u200B');
                }
                else
                {
                    throw new Exception($"Failed to load save file: {loadOperation.error}");
                }

                loadOperation.Dispose();
            }
            else
            {
                json = await File.ReadAllTextAsync(filePath, Encoding.UTF8);
                json = json.Trim('\uFEFF', '\u200B');
            }

            currentSaveData = JsonConvert.DeserializeObject<SaveData>(json, jsonSettings);

        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to load save data: {e.Message}");
            currentSaveData = new();
        }

        _eventBus.Publish<LoadDataEvent>(new(this));
        OnSaveLoaded?.Invoke();





        // if (loadOperation.result == UnityEngine.Networking.UnityWebRequest.Result.Success)
        // {
        //     string json = loadOperation.downloadHandler.text.Trim('\uFEFF', '\u200B');

        //     try
        //     {
        //         // DEBUG: Log what's being loaded
        //         // Debug.Log("=== LOAD DATA DEBUG ===");
        //         // Debug.Log("Raw JSON: " + json);

        //         currentSaveData = JsonConvert.DeserializeObject<SaveData>(json, jsonSettings);

        //         // Debug.Log($"Loaded PlayerCommodities count: {currentSaveData.PlayerCommodities?.Count}");
        //         if (currentSaveData.PlayerCommodities != null)
        //         {
        //             foreach (var entry in currentSaveData.PlayerCommodities)
        //             {
        //                 // Debug.Log($"Loaded Commodity: {entry.id}, Amount: {entry.amount}");
        //             }
        //         }
        //         // Debug.Log("=====================");

        //         Debug.Log("Save data loaded successfully.");
        //     }
        //     catch (Exception e)
        //     {
        //         Debug.LogError($"Failed to parse save data: {e.Message}");
        //         currentSaveData = new SaveData();
        //     }
        // }
        // else
        // {
        //     Debug.LogError($"Failed to load save file: {loadOperation.error}");
        //     currentSaveData = new SaveData();
        // }

        // Mediator.Instance.GlobalEventBus.Publish<LoadDataEvent>(new(this));
        // loadOperation.Dispose();
        // OnSaveLoaded?.Invoke();
    }

    /// <summary>
    /// Saves the current in-memory data to disk.
    /// Uses the proper async method for cross-platform compatibility.
    /// </summary>
    public async Task SaveDataAsync()
    {
        await SaveDataRoutineAsync();
    }

    private async Task SaveDataRoutineAsync()
    {
        // DEBUG: Log what's actually being saved
        // Debug.Log("=== SAVE DATA DEBUG ===");
        // Debug.Log($"PlayerCommodities count: {currentSaveData.PlayerCommodities?.Count}");
        if (currentSaveData.PlayerCommodities != null)
        {
            foreach (var entry in currentSaveData.PlayerCommodities)
            {
                // Debug.Log($"Commodity: {entry.id}, Amount: {entry.amount}");
            }
        }

        string plainJson = JsonConvert.SerializeObject(currentSaveData, jsonSettings);
        // Debug.Log("Plain JSON: " + plainJson);
        // Debug.Log("=====================");

        try
        {
            // Use async file writing
            await File.WriteAllTextAsync(SaveFilePath, plainJson, Encoding.UTF8);
            Debug.Log("Game saved successfully!");
            OnSaveCompleted?.Invoke();
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to save game: {e.Message}");
        }
    }

    public void SetInt(string key, int value) => currentSaveData.IntValues[key] = value;
    public int GetInt(string key, int defaultValue = 0) => currentSaveData.IntValues.TryGetValue(key, out int value) ? value : defaultValue;

    public void SetFloat(string key, float value) => currentSaveData.FloatValues[key] = value;
    public float GetFloat(string key, float defaultValue = 0) => currentSaveData.FloatValues.TryGetValue(key, out float value) ? value : defaultValue;

    public void SetString(string key, string value) => currentSaveData.StringValues[key] = value;
    public string GetString(string key, string defaultValue = "") => currentSaveData.StringValues.TryGetValue(key, out string value) ? value : defaultValue;

    public void SetBool(string key, bool value) => currentSaveData.BoolValues[key] = value;
    public bool GetBool(string key, bool defaultValue = false) => currentSaveData.BoolValues.TryGetValue(key, out bool value) ? value : defaultValue;


    /// <summary>
    /// Generic load method that supports both encrypted and plain data
    /// </summary>

    public T Load<T>(string key, T defaultValue = default(T))
    {
        try
        {
            // First try to load from current save data dictionaries
            if (typeof(T) == typeof(int))
            {
                if (currentSaveData.IntValues.TryGetValue(key, out int value))
                    return (T)(object)value;
            }
            else if (typeof(T) == typeof(float))
            {
                if (currentSaveData.FloatValues.TryGetValue(key, out float value))
                    return (T)(object)value;
            }
            else if (typeof(T) == typeof(string))
            {
                if (currentSaveData.StringValues.TryGetValue(key, out string value))
                    return (T)(object)value;
            }
            else if (typeof(T) == typeof(bool))
            {
                if (currentSaveData.BoolValues.TryGetValue(key, out bool value))
                    return (T)(object)value;
            }
            else
            {
                // For complex types, try to load from JSON in string dictionary
                if (currentSaveData.StringValues.TryGetValue(key, out string jsonValue))
                {
                    // Check if the value is encrypted
                    if (IsEncrypted(jsonValue))
                    {
                        string decryptedJson = EncryptionUtility.Decrypt(jsonValue);
                        if (!string.IsNullOrEmpty(decryptedJson))
                        {
                            return JsonConvert.DeserializeObject<T>(decryptedJson, jsonSettings);
                        }
                    }
                    else
                    {
                        // Plain JSON
                        return JsonConvert.DeserializeObject<T>(jsonValue, jsonSettings);
                    }
                }
            }


            return defaultValue;
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to load data for key '{key}': {e.Message}");
            return defaultValue;
        }
    }


    /// <summary>
    /// Generic save method that supports both simple types and complex objects
    /// </summary>
    public void Save<T>(string key, T value, bool encrypt = false)
    {
        try
        {
            if (typeof(T) == typeof(int))
            {
                currentSaveData.IntValues[key] = (int)(object)value;
            }
            else if (typeof(T) == typeof(float))
            {
                currentSaveData.FloatValues[key] = (float)(object)value;
            }
            else if (typeof(T) == typeof(string))
            {
                currentSaveData.StringValues[key] = (string)(object)value;
            }
            else if (typeof(T) == typeof(bool))
            {
                currentSaveData.BoolValues[key] = (bool)(object)value;
            }
            else
            {
                // For complex types, serialize to JSON
                string json = JsonConvert.SerializeObject(value, jsonSettings);

                if (encrypt)
                {
                    json = EncryptionUtility.Encrypt(json);
                }

                currentSaveData.StringValues[key] = json;
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to save data for key '{key}': {e.Message}");
        }
    }


    /// <summary>
    /// Check if a string value is encrypted
    /// </summary>
    private bool IsEncrypted(string value)
    {
        if (string.IsNullOrEmpty(value))
            return false;

        try
        {
            if (value.Length % 4 == 0 && System.Text.RegularExpressions.Regex.IsMatch(value, @"^[a-zA-Z0-9\+/]*={0,3}$"))
            {
                string decrypted = EncryptionUtility.Decrypt(value);
                return !string.IsNullOrEmpty(decrypted) && decrypted.Trim().StartsWith("{");
            }
        }
        catch
        {
        }

        return false;
    }

    /// <summary>
    /// Loads a list of items with fallback to empty list
    /// </summary>
    public List<T> LoadList<T>(string key)
    {
        var result = Load<List<T>>(key);
        return result ?? new List<T>();
    }

    /// <summary>
    /// Loads a dictionary with fallback to empty dictionary
    /// </summary>
    public Dictionary<TKey, TValue> LoadDictionary<TKey, TValue>(string key)
    {
        var result = Load<Dictionary<TKey, TValue>>(key);
        return result ?? new Dictionary<TKey, TValue>();
    }


    /// <summary>
    /// Deletes the save file from disk and resets in-memory data.
    /// </summary>
    public void DeleteSave()
    {
        if (File.Exists(SaveFilePath))
        {
            File.Delete(SaveFilePath);
            Debug.Log("Save file deleted.");
        }
        currentSaveData = new SaveData();
        Debug.Log("Save data reset to default.");
    }


}




/// <summary>
/// The serializable class that holds all our save data.
/// Add any other data structures you need here.
/// </summary>
[System.Serializable]
public class SaveData
{
    public Dictionary<string, int> IntValues = new Dictionary<string, int>();
    public Dictionary<string, float> FloatValues = new Dictionary<string, float>();
    public Dictionary<string, string> StringValues = new Dictionary<string, string>();
    public Dictionary<string, bool> BoolValues = new Dictionary<string, bool>();

    public List<CommoditySaveData> PlayerCommodities = new();


}

public static class EncryptionUtility
{
    // IMPORTANT: CHANGE THESE KEYS!
    private static readonly string _key = "S4VW05BOv8T6bGdNz671kQXjqfa0y2Dc"; // 32 chars for AES-256
    private static readonly string _iv = "QGy9G3xagZ77nhXA"; // 16 chars for AES

    /// <summary>
    /// Encrypts a plain text string using AES encryption.
    /// </summary>
    public static string Encrypt(string plainText)
    {
        using (Aes aesAlg = Aes.Create())
        {
            aesAlg.Key = Encoding.UTF8.GetBytes(_key);
            aesAlg.IV = Encoding.UTF8.GetBytes(_iv);

            ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

            using (MemoryStream msEncrypt = new MemoryStream())
            {
                using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                {
                    using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                    {
                        swEncrypt.Write(plainText);
                    }
                }

                return Convert.ToBase64String(msEncrypt.ToArray());
            }
        }
    }

    /// <summary>
    /// Decrypts a cipher text string using AES encryption.
    /// </summary>
    public static string Decrypt(string cipherText)
    {
        if (string.IsNullOrEmpty(cipherText))
            return null;

        try
        {
            byte[] buffer = Convert.FromBase64String(cipherText);

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Encoding.UTF8.GetBytes(_key);
                aesAlg.IV = Encoding.UTF8.GetBytes(_iv);

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msDecrypt = new MemoryStream(buffer))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            return srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
        }
        catch (FormatException)
        {
            Debug.LogError("EncryptionUtility: Decryption failed - invalid Base64 string. The save file might be corrupt or unencrypted.");
            return null;
        }
        catch (CryptographicException)
        {
            Debug.LogError("EncryptionUtility: Decryption failed - wrong key or corrupt data.");
            return null;
        }
        catch (Exception e)
        {
            Debug.LogError($"EncryptionUtility: Decryption failed with error: {e.Message}");
            return null;
        }
    }

}