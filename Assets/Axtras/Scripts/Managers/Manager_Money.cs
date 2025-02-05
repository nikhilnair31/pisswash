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

    [Header("UI Settings")]
    [SerializeField] private TMP_Text moneyText;
    #endregion  

    private void Awake() {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start() {
        UpdateMoney(initMoney);
    }

    public void UpdateMoney(int amount) {
        currMoney += amount;

        // Update money amount in UI
        Manager_UI.Instance.SetMoneyUI(currMoney);
        // Update current money and spent money
        Manager_SaveLoad.Instance.SaveStatData("haveMoney", "set", currMoney);
        Manager_SaveLoad.Instance.SaveStatData("spentMoney", "add", amount);
    }

    public bool GetHasMoneyToBuy() {
        return currMoney > 0;
    }
}