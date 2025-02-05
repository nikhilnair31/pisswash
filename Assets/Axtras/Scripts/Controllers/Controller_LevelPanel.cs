using UnityEngine;
using UnityEngine.UI;
using SimpleJSON;
using TMPro;

public class Controller_LevelPanel : MonoBehaviour 
{
    #region Variables
    [Header("Level Data")]
    [SerializeField] public int levelIndex;
    [SerializeField] public string levelName;
    [SerializeField] public string levelRating;
    [SerializeField] public bool levelPlayable;
    [SerializeField] public bool levelunlocked;

    [Header("UI Elements")]
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text ratingText;
    [SerializeField] private Image unlockedImage;
    private Button button;
    #endregion

    private void Start() {
        SetupUI();
    }
    private void SetupUI() {
        nameText.text = levelName;
        ratingText.text = levelRating;
        unlockedImage.sprite = Manager_UI.Instance.GetRatingSprite(levelPlayable, levelunlocked);

        button = GetComponent<Button>();
        button.interactable = levelPlayable && levelunlocked;
        button.onClick.AddListener(OnClick);
    }
    private void OnClick() {
        if (levelPlayable && levelunlocked) {
            Debug.Log($"Level {levelIndex} - {levelName} selected!");
            Manager_UI.Instance.StartGame(levelName);
            Manager_SaveLoad.Instance.SaveStatData("toalShiftWorked", "add", 1);
        }
        else {
            Debug.LogWarning($"Level {levelIndex} - {levelName} is locked!");
        }
    } 

    public void Initialize(int ind, JSONObject levelData) {
        levelIndex = ind;
        levelName = levelData["name"];
        levelRating = levelData["rating"];
        levelPlayable = levelData["playable"];
        levelunlocked = levelData["unlocked"];
    }   
}