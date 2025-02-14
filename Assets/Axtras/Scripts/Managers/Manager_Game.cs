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
        var timeLeftPerc = Manager_Timer.Instance.GetTimeRemainingPerc();
        var stainPerc = Manager_Stains.Instance.GetStainCleanedPerc();
        
        // Risk Metrics
        var stonesPassed = Controller_Pee.Instance.GetStonesPassedCount();
        var stonesAcquired = Controller_Pee.Instance.GetStonesAcquiredCount();
        var slapTaken = Manager_Drinks.Instance.GetStealSlap();
        
        // Stealing Metrics
        var stolenDrinks = Manager_Drinks.Instance.GetDrinksStolenCount();

        // Calculations
        var baseScore = (timeLeftPerc * 0.4f) + (stainPerc * 0.6f);
        
        // Penalties
        var stonePenalty = stonesAcquired * 2f; // -X% per stone risked
        var slapPenalty = slapTaken * 3f; // -X% per slap
        var totalPenalties = Mathf.Min(stonePenalty + slapPenalty, 20f); // Cap penalties

        // Bonuses
        var stoneBonus = stonesPassed * 7f; // +X% per successfully passed stone
        var stealMultiplier = 1f + (0.1f * stolenDrinks); // +X% multiplier per stolen drink
        var perfectCleanBonus = (stainPerc >= 99f) ? 10f : 0f;

        // Final Score
        var finalScore = (baseScore - totalPenalties + stoneBonus + perfectCleanBonus) * stealMultiplier;
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
        
        Debug.Log(
            $"timeLeftPerc: {timeLeftPerc}\n" +
            $"stainPerc: {stainPerc}\n\n" +
            $"stonesPassed: {stonesPassed}\n" +
            $"stonesAcquired: {stonesAcquired}\n" +
            $"slapTaken: {slapTaken}\n\n" +
            $"stolenDrinks: {stolenDrinks}\n\n" +
            $"baseScore: {baseScore}\n\n" +
            $"stonePenalty: {stonePenalty}\n" +
            $"slapPenalty: {slapPenalty}\n" +
            $"totalPenalties: {totalPenalties}\n\n" +
            $"stoneBonus: {stoneBonus}\n" +
            $"stealMultiplier: {stealMultiplier}\n" +
            $"perfectCleanBonus: {perfectCleanBonus}\n\n" +
            $"finalScore: {finalScore}" +
            $"grade: {grade}"
        );

        return grade;
    }
}