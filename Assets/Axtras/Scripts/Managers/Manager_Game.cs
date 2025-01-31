using UnityEngine;

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

    public string CalculateScoreLetter() {
        // Base Metrics
        float timeScore = Manager_Timer.Instance.GetTimeRemainingPerc(); // % of time left
        float stainScore = Manager_Stains.Instance.GetStainCleanedPerc();
        float healthScore = Controller_Health.Instance.GetHealthPerc();
        
        // Risk Metrics
        int stonesPassed = Controller_Pee.Instance.GetStonesPassedCount();
        int stonesAcquired = Controller_Pee.Instance.GetStonesAcquiredCount();
        int damageTaken = Manager_Hazards.Instance.GetDamageCount();
        
        // Stealing Metrics
        int stolenDrinks = Manager_Bottles.Instance.GetBottlesStolenCount();
        int maxPossibleSteals = Manager_Bottles.Instance.GetMaxBottlesPerLevel();

        // Calculations
        float baseScore = (timeScore * 0.25f) + (stainScore * 0.5f) + (healthScore * 0.25f);
        
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