using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using System.Collections.Generic;

public class Manager_Game : MonoBehaviour 
{
    #region Vars
    public static Manager_Game Instance { get; private set; }
    #endregion

    private void Awake() {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
    
    private void Start() {
        string progressionFilePath = Application.persistentDataPath + "/game.json";
        string saveDatStr = Manager_SaveLoad.Instance.Load(progressionFilePath);
        Debug.Log($"Save Data: {saveDatStr}");

        if (string.IsNullOrEmpty(saveDatStr)) {
            Debug.Log("No save data found. Creating new save file...");
            string newSaveData = GenerateGameSaveFile();
            Manager_SaveLoad.Instance.Save(progressionFilePath, newSaveData);
        }
        
        Manager_UI.Instance.ShowMenu();
    }

    private string GenerateGameSaveFile() {
        // Get list of all scenes
        // var sceneCount = SceneManager.sceneCount;
        var sceneCountInBuildSettings = SceneManager.sceneCountInBuildSettings;
        Debug.Log($"Scene Count: {sceneCountInBuildSettings}");

        var sceneList = new List<string>();
        for (int i = 0; i < sceneCountInBuildSettings; i++) {
            sceneList.Add(SceneManager.GetSceneByBuildIndex(i).name);
        }
        Debug.Log($"Scenes: {string.Join(", ", sceneList)}");

        // Get list of all scenes in build settings
        var sceneNumber = SceneManager.sceneCountInBuildSettings;
        var sceneNameList = new string[sceneNumber];
        for (int i = 0; i < sceneNumber; i++) {
            sceneNameList[i] = Path.GetFileNameWithoutExtension(SceneUtility.GetScenePathByBuildIndex(i));
        }
        Debug.Log($"sceneNameList: {string.Join(", ", sceneNameList)}");

        // Create new scene data
        var sceneDataList = new List<Data_Scene>();
        for (int i = 0; i < sceneCountInBuildSettings; i++) {
            sceneDataList.Add(new Data_Scene {
                sceneName = sceneNameList[i]
            });
        }

        // Unlock 1st and 2nd scene
        sceneDataList[0].unlocked = true;
        sceneDataList[0].playable = false;
        sceneDataList[1].unlocked = true;

        // Create new game data
        Data_Game gameData = new() {
            sceneDataList = sceneDataList
        };

        return JsonUtility.ToJson(gameData);
    }

    public string CalculateScoreLetter() {
        // Base Metrics
        float timeScore = Manager_Timer.Instance.GetTimeRemainingPerc(); // % of time left
        float stainScore = Manager_Stains.Instance.GetStainCleanedPerc();
        
        // Risk Metrics
        int stonesPassed = Controller_Pee.Instance.GetStonesPassedCount();
        int stonesAcquired = Controller_Pee.Instance.GetStonesAcquiredCount();
        int damageTaken = Manager_Hazards.Instance.GetDamageCount();
        
        // Stealing Metrics
        int stolenDrinks = Manager_Bottles.Instance.GetBottlesStolenCount();

        // Calculations
        float baseScore = (timeScore * 0.5f) + (stainScore * 0.5f);
        
        // Penalties
        float stonePenalty = stonesAcquired * 2f; // -2% per stone risked
        float damagePenalty = damageTaken * 5f; // -5% per electric shock
        float totalPenalties = Mathf.Min(stonePenalty + damagePenalty, 30f); // Cap penalties

        // Bonuses
        float stoneBonus = stonesPassed * 5f; // +5% per successfully passed stone
        float stealMultiplier = 1f + (0.05f * stolenDrinks); // +5% multiplier per stolen drink
        float perfectCleanBonus = (stainScore >= 99.9f) ? 10f : 0f;

        // Final Score
        float finalScore = (baseScore - totalPenalties + stoneBonus + perfectCleanBonus) * stealMultiplier;
        finalScore = Mathf.Clamp(finalScore, 0f, 100f); // Keep within 0-100 range

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