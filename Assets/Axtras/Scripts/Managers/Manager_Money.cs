using UnityEngine;
using TMPro;

public class Manager_Money : MonoBehaviour 
{
    #region Vars
    public static Manager_Money Instance { get; private set; }

    [Header("Money Settings")]
    [SerializeField] private int initMoney = 10;
    [SerializeField] private int currMoney = 0;

    [Header("Audio Settings")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip[] moneyClips;
    #endregion  

    private void Awake() {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void UpdateMoney(int amount) {
        currMoney += amount;

        // Update money amount in UI
        Manager_UI.Instance.SetMoneyUI(currMoney);
        // Update current money and spent money
        Manager_SaveLoad.Instance.SaveStatData("haveMoney", "set", currMoney);

        Debug.Log($"currMoney: {currMoney} | amount: {amount}");
    }

    public bool GetHasMoneyToBuy() {
        return currMoney > 0;
    }
    public int GetHasMoneyByRating(string grade) {
        int moneyGained = grade switch {
            string n when n == "S+" => 35,
            string n when n == "S" => 25,
            string n when n == "A" => 15,
            string n when n == "B" => 8,
            string n when n == "C" => 5, 
            string n when n == "D" => 3,
            _ => 0
        };
        return moneyGained;
    }
}