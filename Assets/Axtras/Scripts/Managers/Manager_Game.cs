using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using System.Collections.Generic;
using SimpleJSON;

public class Manager_Game : MonoBehaviour 
{
    #region Vars
    public static Manager_Game Instance { get; private set; }
    #endregion

    private void Awake() {
        if (Instance == null){
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
    }
    
    private void Start() {
        LoadLevelData();
    }
    
    private void LoadLevelData() {
        string saveDatStr = Manager_SaveLoad.Instance.Load();
        // Debug.Log($"saveDatStr: {saveDatStr}");

        string saveData;
        if (string.IsNullOrEmpty(saveDatStr)) {
            Debug.LogWarning("No save data found. Creating new save file...");
            saveData = Manager_SaveLoad.Instance.GenerateGameSaveFile();
            Manager_SaveLoad.Instance.Save(saveData);
        }
        else {
            Debug.Log("Save data found. Loading save file...");
            saveData = saveDatStr;
        }
        // Debug.Log($"saveData: {saveData}");
        
        JSONObject loadedGameData = JSON.Parse(saveData) as JSONObject;
        var sceneDataList = loadedGameData["sceneDataList"].AsArray;
        
        Manager_UI.Instance.SpawnLevelPanels(sceneDataList);
        Manager_UI.Instance.ShowMenu();
    }

    public string CalculateScoreLetter() {
        // Base Metrics
        float timeScore = Manager_Timer.Instance.GetTimeRemainingPerc(); // % of time left
        Debug.Log($"timeScore: {timeScore}");
        float stainScore = Manager_Stains.Instance.GetStainCleanedPerc();
        Debug.Log($"stainScore: {stainScore}");
        
        // Risk Metrics
        int stonesPassed = Controller_Pee.Instance.GetStonesPassedCount();
        Debug.Log($"stonesPassed: {stonesPassed}");
        int stonesAcquired = Controller_Pee.Instance.GetStonesAcquiredCount();
        Debug.Log($"stonesAcquired: {stonesAcquired}");
        int damageTaken = Manager_Hazards.Instance.GetDamageCount();
        Debug.Log($"damageTaken: {damageTaken}");
        
        // Stealing Metrics
        int stolenDrinks = Manager_Bottles.Instance.GetBottlesStolenCount();
        Debug.Log($"stolenDrinks: {stolenDrinks}");

        // Calculations
        float baseScore = (timeScore * 0.5f) + (stainScore * 0.5f);
        Debug.Log($"baseScore: {baseScore}");
        
        // Penalties
        float stonePenalty = stonesAcquired * 2f; // -2% per stone risked
        Debug.Log($"stonePenalty: {stonePenalty}");
        float damagePenalty = damageTaken * 5f; // -5% per electric shock
        Debug.Log($"damagePenalty: {damagePenalty}");
        float totalPenalties = Mathf.Min(stonePenalty + damagePenalty, 30f); // Cap penalties
        Debug.Log($"totalPenalties: {totalPenalties}");

        // Bonuses
        float stoneBonus = stonesPassed * 5f; // +5% per successfully passed stone
        Debug.Log($"stoneBonus: {stoneBonus}");
        float stealMultiplier = 1f + (0.05f * stolenDrinks); // +5% multiplier per stolen drink
        Debug.Log($"stealMultiplier: {stealMultiplier}");
        float perfectCleanBonus = (stainScore >= 99.9f) ? 10f : 0f;
        Debug.Log($"perfectCleanBonus: {perfectCleanBonus}");

        // Final Score
        float finalScore = (baseScore - totalPenalties + stoneBonus + perfectCleanBonus) * stealMultiplier;
        finalScore = Mathf.Clamp(finalScore, 0f, 100f); // Keep within 0-100 range
        Debug.Log($"finalScore: {finalScore}");

        // Letter Grade
        string grade = finalScore switch {
            float n when n >= 97 => "S+",
            float n when n >= 90 => "S",
            float n when n >= 80 => "A",
            float n when n >= 70 => "B",
            float n when n >= 60 => "C", 
            float n when n >= 50 => "D",
            _ => "F"
        };

        return grade;
    }
}