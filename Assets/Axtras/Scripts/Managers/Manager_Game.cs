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

    public (string grade, string stats) CalcStatAndGrade() {
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
        var baseScore = (stainPerc * 0.75f) + (Mathf.Pow(Mathf.Max(timeLeftPerc, 0.0001f), 0.5f) * 5f); 
        
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
        var grade = finalScore switch {
            float n when n >= 95 => "S+",
            float n when n >= 80 => "S",
            float n when n >= 70 => "A",
            float n when n >= 60 => "B",
            float n when n >= 55 => "C", 
            float n when n >= 40 => "D",
            _ => "F"
        };
       
        var stats = 
            $"% of Time Left: +{timeLeftPerc, -5}\n" +
            $"% of Stains Cleaned: +{stainPerc, -5}\n\n" +
            $"Kidney Stones Passed: +{stonesPassed, -5}\n" +
            $"Kidney Stones Created: -{stonesAcquired, -5}\n" +
            $"Slaps Taken: -{slapTaken, -5}\n\n" +
            $"Drinks Stolen: +{stolenDrinks, -5}\n\n" +
            $"Base Score: {baseScore, -5}\n\n" +
            $"Kidney Stone Penalty: -{stonePenalty, -5}\n" +
            $"Slap Penalty: -{slapPenalty, -5}\n" +
            $"Total Penalties: -{totalPenalties, -5}\n\n" +
            $"Kidney Stone Bonus: {stoneBonus, -5}\n" +
            $"Stealing Multiplier: {stealMultiplier, -5}\n" +
            $"Perfect Cleaning Bonus: {perfectCleanBonus, -5}\n\n" +
            $"Final Score: {finalScore, -5}\n"
        ;

        Debug.Log($"grade: {grade}\nstats:\n{stats}");

        return (grade, stats);
    }
}