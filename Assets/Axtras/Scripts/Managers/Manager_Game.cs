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
        Manager_UI.Instance.ShowMenu();
    }

    public string CalculateScoreLetter() {
        // Base Metrics
        var timeScore = Manager_Timer.Instance.GetTimeRemainingPerc(); // % of time left
        var stainScore = Manager_Stains.Instance.GetStainCleanedPerc();
        
        // Risk Metrics
        var stonesPassed = Controller_Pee.Instance.GetStonesPassedCount();
        var stonesAcquired = Controller_Pee.Instance.GetStonesAcquiredCount();
        var damageTaken = Manager_Hazards.Instance.GetDamageCount();
        
        // Stealing Metrics
        var stolenDrinks = Manager_Drinks.Instance.GetDrinksStolenCount();

        // Calculations
        var baseScore = (timeScore * 0.5f) + (stainScore * 0.5f);
        
        // Penalties
        var stonePenalty = stonesAcquired * 2f; // -2% per stone risked
        var damagePenalty = damageTaken * 5f; // -5% per electric shock
        var totalPenalties = Mathf.Min(stonePenalty + damagePenalty, 30f); // Cap penalties

        // Bonuses
        var stoneBonus = stonesPassed * 5f; // +5% per successfully passed stone
        var stealMultiplier = 1f + (0.05f * stolenDrinks); // +5% multiplier per stolen drink
        var perfectCleanBonus = (stainScore >= 99.9f) ? 10f : 0f;

        // Final Score
        var finalScore = (baseScore - totalPenalties + stoneBonus + perfectCleanBonus) * stealMultiplier;
        finalScore = Mathf.Clamp(finalScore, 0f, 100f); // Keep within 0-100 range
        Debug.Log(
            $"timeScore: {timeScore}\n" +
            $"stainScore: {stainScore}\n\n" +
            $"stonesPassed: {stonesPassed}\n" +
            $"stonesAcquired: {stonesAcquired}\n" +
            $"damageTaken: {damageTaken}\n\n" +
            $"stolenDrinks: {stolenDrinks}\n\n" +
            $"baseScore: {baseScore}\n\n" +
            $"stonePenalty: {stonePenalty}\n" +
            $"damagePenalty: {damagePenalty}\n" +
            $"totalPenalties: {totalPenalties}\n\n" +
            $"stoneBonus: {stoneBonus}\n" +
            $"stealMultiplier: {stealMultiplier}\n" +
            $"perfectCleanBonus: {perfectCleanBonus}\n\n" +
            $"finalScore: {finalScore}"
        );

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