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
        Debug.Log("CalculateScore");

        float timeScore = Manager_Timer.Instance.GetTimePerc();
        Debug.Log($"timeScore: {timeScore}");
        float healthScore = Controller_Health.Instance.GetHealthPerc();
        Debug.Log($"healthScore: {healthScore}");
        float stainScore = Manager_Stains.Instance.GetStainCleanedPerc();
        Debug.Log($"stainScore: {stainScore}");
        float stolenScore = Manager_Bottles.Instance.GetBottlesStolenPerc();
        Debug.Log($"stolenScore: {stolenScore}");

        float finalScore = (timeScore * 0.3f) + (stainScore * 0.4f) + (healthScore * 0.3f) + stolenScore;
        Debug.Log($"finalScore: {finalScore}");

        string grade = finalScore switch {
            float n when n >= 90 => "S",
            float n when n >= 80 => "A",
            float n when n >= 70 => "B",
            float n when n >= 60 => "C",
            float n when n >= 50 => "D",
            _ => "B",
        };
        Debug.Log($"grade: {grade}");

        return grade;
    }
}