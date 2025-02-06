using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using SimpleJSON;

public class Manager_SaveLoad : MonoBehaviour 
{
    #region Variables
    public static Manager_SaveLoad Instance { get; private set; }
    string filePath;
    #endregion

    private void Awake() {
        if (Instance == null){
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);

        filePath = Application.persistentDataPath + "/game.json";
        Debug.Log($"Awake filePath: {filePath}");
    }

    private void Start() {
        LoadLevelData();
    }
    
    public void Save(string plainText) {
        if (string.IsNullOrEmpty(plainText)) {
            throw new ArgumentException("Plaintext cannot be null or empty.");
        }
        
        try {
            // Debug.Log($"Encrypting data: {plainText}");

            using Aes aes = Aes.Create();
            aes.Key = GetEncryptionKey();
            aes.GenerateIV(); // Ensure a new IV is generated

            // Save the IV first
            Debug.Log($"Save filePath: {filePath}");
            using FileStream dataStream = new(filePath, FileMode.Create);
            dataStream.Write(aes.IV, 0, aes.IV.Length);

            // Encrypt the data
            using CryptoStream cryptoStream = new(dataStream, aes.CreateEncryptor(), CryptoStreamMode.Write);
            using StreamWriter writer = new(cryptoStream);

            writer.Write(plainText);
            Debug.Log("Data saved successfully!");
        }
        catch (Exception ex) {
            Debug.LogError($"Encryption error: {ex.Message}");
            Debug.LogError($"Stack Trace: {ex.StackTrace}");
        }
    }
    public string Load() {
        if (!File.Exists(filePath)) {
            Debug.LogWarning("Load file not found!");
            return null;
        }

        try {
            using FileStream dataStream = new(filePath, FileMode.Open);
            byte[] iv = new byte[16];
            dataStream.Read(iv, 0, iv.Length);

            using Aes aes = Aes.Create();
            aes.Key = GetEncryptionKey();
            aes.IV = iv;

            using CryptoStream cryptoStream = new(dataStream, aes.CreateDecryptor(), CryptoStreamMode.Read);
            using StreamReader reader = new(cryptoStream);

            string decryptedData = reader.ReadToEnd();
            // Debug.Log($"Decrypted data successfully!\n{decryptedData}");

            return decryptedData;
        }
        catch (Exception ex) {
            Debug.LogError($"Load error: {ex.Message}");
            Debug.LogError($"Load stacktrace: {ex.StackTrace}");
            return null;
        }
    }   
    
    private byte[] GetEncryptionKey() {
        string keyString32 = "Th1sIsMy32ByteS3cur3K3y@ForAES25";
        // 32 bytes for AES-256
        byte[] keyBytes32 = Encoding.UTF8.GetBytes(keyString32);
        // Debug.Log($"Key length: {keyBytes32.Length}");

        return keyBytes32;
    }

    public string GenerateGameSaveFile() {
        // Get list of all scenes in build settings
        var sceneNameList = Manager_Scene.Instance.GetSceneStrArr();

        // Create new scene data
        var sceneDataList = new List<Data_Scene>();
        for (int i = 0; i < sceneNameList.Length; i++) {
            sceneDataList.Add(new Data_Scene {
                name = sceneNameList[i]
            });
        }
        Debug.Log($"sceneDataList: {string.Join(", ", sceneDataList)}");

        // Unlock 1st and 2nd scene
        sceneDataList[0].unlocked = true;
        sceneDataList[1].unlocked = true;

        // Create new game data
        Data_Game gameData = new() {
            sceneDataList = sceneDataList
        };

        return JsonUtility.ToJson(gameData);
    }

    public void SaveLevelUnlocked(string scenename, string rating = "") {
        var dataStr = Load();
        
        var dataJson = JSON.Parse(dataStr) as JSONObject;
        Debug.Log($"SaveLevelUnlocked og dataJson: {dataJson}");
        
        var sceneDataList = dataJson["sceneDataList"].AsArray;
        Debug.Log($"SaveLevelUnlocked og sceneDataList: {sceneDataList}");

        for (int i = 0; i < sceneDataList.Count; i++) {
            JSONObject levelData = sceneDataList[i] as JSONObject;
            Debug.Log($"levelData: {levelData}");

            if (levelData["name"] == scenename) {
                levelData["unlocked"] = true;
                levelData["rating"] = rating;
                break;
            }
        }
        Debug.Log($"SaveLevelUnlocked new sceneDataList: {sceneDataList}");

        dataJson["sceneDataList"] = sceneDataList;
        Debug.Log($"SaveLevelUnlocked new dataJson: {dataJson}");

        Save(dataJson.ToString());
    }
    public void SaveStatData(string statKey, string type = "set", int val = 1) {
        Debug.Log($"SaveStatData");
        
        var dataJson = JSON.Parse(Load()) as JSONObject;
        Debug.Log($"SaveStatData og dataJson: {dataJson}");
        
        Debug.Log($"SaveStatData og toalShiftWorkedInt: {dataJson[statKey]}");
        if (type == "add") {
            dataJson[statKey] += val;
        }
        else if (type == "set") {
            dataJson[statKey] = val;
        }
        Debug.Log($"SaveStatData new toalShiftWorkedInt: {dataJson[statKey]}");

        Save(dataJson.ToString());
    }
    
    public void LoadLevelData() {
        string saveDatStr = Load();
        // Debug.Log($"saveDatStr: {saveDatStr}");

        string saveData;
        if (string.IsNullOrEmpty(saveDatStr)) {
            Debug.LogWarning("No save data found. Creating new save file...");
            saveData = GenerateGameSaveFile();
            Save(saveData);
        }
        else {
            Debug.Log("Save data found. Loading save file...");
            saveData = saveDatStr;
        }
        // Debug.Log($"saveData: {saveData}");
        
        JSONObject loadedGameData = JSON.Parse(saveData) as JSONObject;
        var sceneDataList = loadedGameData["sceneDataList"].AsArray;
        
        Manager_UI.Instance.SpawnLevelPanels(sceneDataList);
    }
}