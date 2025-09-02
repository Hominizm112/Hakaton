using UnityEngine;
using System;
using System.Security.Cryptography;
using System.Collections;
using System.IO;
using System.Text;
using System.Collections.Generic;



/// <summary>
/// A cross-platform save manager that handles both desktop and WebGL.
/// Uses JSON serialization and async file operations for compatibility.
/// </summary>
public class SaveManager : UnitySingleton<SaveManager>
{
    public static event Action OnSaveLoaded;
    public static event Action OnSaveCompleted;

    private const string SAVE_FILE_NAME = "savegame.json";

    private string SaveFilePath => Path.Combine(Application.persistentDataPath, SAVE_FILE_NAME);

    private SaveData _currentSaveData;
    public SaveData CurrentSaveData => _currentSaveData;


    protected override void Awake()
    {
        base.Awake();
        SetPersistent(this);

        _currentSaveData = new SaveData();
        Debug.Log($"Save file path: {SaveFilePath}");
    }

    /// <summary>
    /// Call this early (e.g., from Bootstrap) to load saved data.
    /// For WebGL, this must be asynchronous.
    /// </summary>
    public void LoadSaveData()
    {
        StartCoroutine(LoadSaveDataRoutine());
    }

    private IEnumerator LoadSaveDataRoutine()
    {
        if (!File.Exists(SaveFilePath))
        {
            Debug.Log("No save file found. Using default data");
            OnSaveLoaded?.Invoke();
            yield break;
        }

        var loadOperation = UnityEngine.Networking.UnityWebRequest.Get(SaveFilePath);

        string filePath = SaveFilePath;
#if UNITY_WEBGL
        filePath = "file:///" + SaveFilePath;
#endif

        loadOperation = UnityEngine.Networking.UnityWebRequest.Get(filePath);
        yield return loadOperation.SendWebRequest();

        string encryptedJson = loadOperation.downloadHandler.text;


        if (loadOperation.result == UnityEngine.Networking.UnityWebRequest.Result.Success)
        {

            string decryptedJson = EncryptionUtility.Decrypt(encryptedJson);

            if (decryptedJson != null)
            {
                try
                {
                    _currentSaveData = JsonUtility.FromJson<SaveData>(decryptedJson);
                    Debug.Log("Save data loaded succensfully.");
                }
                catch (Exception e)
                {
                    Debug.LogError($"Failed to parse save data: {e.Message}");
                    _currentSaveData = new SaveData();
                }
            }
        }
        else
        {
            Debug.LogWarning("Decryption failed. Attempting to load as plain text...");
            try
            {
                _currentSaveData = JsonUtility.FromJson<SaveData>(encryptedJson);
                Debug.Log("Legacy (unencrypted) save data loaded.");
                SaveData();
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to load legacy save data: {e.Message}. Resetting to default.");
                _currentSaveData = new SaveData();
            }
        }

        loadOperation.Dispose();
        OnSaveLoaded?.Invoke();
    }

    /// <summary>
    /// Saves the current in-memory data to disk.
    /// Uses the proper async method for cross-platform compatibility.
    /// </summary>
    public void SaveData()
    {
        StartCoroutine(SaveDataRoutine());
    }

    private IEnumerator SaveDataRoutine()
    {
        string plainJson = JsonUtility.ToJson(_currentSaveData, prettyPrint: true);

        string encryptedJson = EncryptionUtility.Encrypt(plainJson);

        byte[] bytes = Encoding.UTF8.GetBytes(encryptedJson);
        var saveOperation = UnityEngine.Networking.UnityWebRequest.Put(SaveFilePath, bytes);
        yield return saveOperation.SendWebRequest();

        if (saveOperation.result == UnityEngine.Networking.UnityWebRequest.Result.Success)
        {
            Debug.Log("Game saved successfully!");
            OnSaveCompleted?.Invoke();
        }
        else
        {
            Debug.LogError($"Failed to save game: {saveOperation.error}");
        }

        saveOperation.Dispose();
    }

    public void SetInt(string key, int value) => _currentSaveData.IntValues[key] = value;
    public int GetInt(string key, int defaultValue = 0) => _currentSaveData.IntValues.TryGetValue(key, out int value) ? value : defaultValue;

    public void SetFloat(string key, float value) => _currentSaveData.FloatValues[key] = value;
    public float GetFloat(string key, float defaultValue = 0) => _currentSaveData.FloatValues.TryGetValue(key, out float value) ? value : defaultValue;

    public void SetString(string key, string value) => _currentSaveData.StringValues[key] = value;
    public string GetString(string key, string defaultValue = "") => _currentSaveData.StringValues.TryGetValue(key, out string value) ? value : defaultValue;

    public void SetBool(string key, bool value) => _currentSaveData.BoolValues[key] = value;
    public bool GetBool(string key, bool defaultValue = false) => _currentSaveData.BoolValues.TryGetValue(key, out bool value) ? value : defaultValue;

    /// <summary>
    /// Deletes the save file from disk and resets in-memory data.
    /// </summary>
    public void DeleteSave()
    {
        if (File.Exists(SaveFilePath))
        {
            File.Delete(SaveFilePath);
#if !UNITY_WEBGL
            Debug.Log("Save file deleted.");
#endif
        }
        _currentSaveData = new SaveData();
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