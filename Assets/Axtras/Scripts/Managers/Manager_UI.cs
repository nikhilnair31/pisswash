using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;
using System;
using SimpleJSON;

public class Manager_UI : MonoBehaviour 
{
    #region Vars
    public static Manager_UI Instance { get; private set; }

    [Header("Menu UI")]
    [SerializeField] private GameObject menuCanvasGO;
    [SerializeField] private GameObject titleScreenGO;
    [SerializeField] private GameObject selectionScreenGO;
    [SerializeField] private GameObject levelContentPanelGO;
    [SerializeField] private GameObject statsContentPanelGO;
    [SerializeField] private Button startGame_Menu_Button;
    [SerializeField] private Button backToMenu_Menu_Button;
    [SerializeField] private Button stats_Menu_Button;
    [SerializeField] private Button exitGame_Menu_Button;
    [SerializeField] public bool inMenu = true;

    [Header("Game UI")]
    [SerializeField] private GameObject gameCanvasGO;
    [SerializeField] private Image dehydrationImage;
    [SerializeField] private Image damageImage;
    [SerializeField] private Image peeImage;
    [SerializeField] private TMP_Text lookedAt_Text;
    [SerializeField] private TMP_Text qteKey_Text;
    [SerializeField] private TMP_Text kidneyStone_Text;
    [SerializeField] private TMP_Text timer_Text;
    [SerializeField] private TMP_Text money_Text;
    [SerializeField] public bool inGame = false;

    [Header("Level Over UI")]
    [SerializeField] private GameObject levelOverCanvasGO;
    [SerializeField] private TMP_Text score_LevelOver_Text;
    [SerializeField] private Button retry_LevelOver_Button;
    [SerializeField] private Button next_LevelOver_Button;
    [SerializeField] private Button menu_LevelOver_Button;
    [SerializeField] public bool levelOver = false;

    [Header("Pause UI")]
    [SerializeField] private GameObject pauseCanvasGO;
    [SerializeField] public bool inPause = false;

    [Header("Loading UI")]
    [SerializeField] private GameObject loadingCanvasGO;

    [Header("Modals")]
    [SerializeField] private GameObject modalPrefab;

    [Header("Level Panels")]
    [SerializeField] private GameObject levelPanelPrefab;

    [Header("Sprites")]
    [SerializeField] private Sprite lockedSprite;
    [SerializeField] private Sprite unlockedSprite;
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
        SetupButtons();
    }
    private void SetupButtons() {
        startGame_Menu_Button?.onClick.AddListener(ShowSelection);
        backToMenu_Menu_Button?.onClick.AddListener(HideSelection);
        stats_Menu_Button?.onClick.AddListener(ShowStats);
        exitGame_Menu_Button?.onClick.AddListener(ExitGame);

        retry_LevelOver_Button?.onClick.AddListener(RetryLevel);
        next_LevelOver_Button?.onClick.AddListener(NextLevel);
        menu_LevelOver_Button?.onClick.AddListener(ShowMenu);
    }
     
    private void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            PauseGame();
        }
    }
    
    public void SetStatsText(JSONObject dataJson) {
        var labelWidth = 25; // Adjust to fit your longest label

        var statsStr =
            $"{"Shifts Worked:".PadRight(labelWidth)} {dataJson["totalShiftWorked"] ?? 0}\n" +
            $"{"Cleaned Stains:".PadRight(labelWidth)} {dataJson["totalCleanedStains"]}\n" +
            $"{"Peed Amount:".PadRight(labelWidth)} {dataJson["peedAmount"]} fl oz\n" +
            $"\n" +
            $"{"Kidney Stones Created:".PadRight(labelWidth)} {dataJson["totalKidneyStonesCreated"]}\n" +
            $"{"Kidney Stones Passed:".PadRight(labelWidth)} {dataJson["totalKidneyStonesPassed"]}\n" +
            $"\n" +
            $"{"Total Slaps:".PadRight(labelWidth)} {dataJson["totalSlaps"]}\n" +
            $"\n" +
            $"{"Drinks Bought:".PadRight(labelWidth)} {dataJson["bottlesBought"]}\n" +
            $"{"Drinks Stolen:".PadRight(labelWidth)} {dataJson["bottlesStolen"]}\n" +
            $"\n" +
            $"{"Have Money:".PadRight(labelWidth)} ${dataJson["haveMoney"]}\n" +
            $"{"Spent Money:".PadRight(labelWidth)} ${dataJson["spentMoney"]}\n";

        var statsText = statsContentPanelGO.GetComponent<TMP_Text>();
        statsText.text = statsStr;
    }
    public void SpawnLevelPanels(JSONObject dataJson) {
        var sceneDataList = dataJson["sceneDataList"].AsArray;

        foreach (Transform child in levelContentPanelGO.transform) {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < sceneDataList.Count; i++) {
            JSONObject levelData = sceneDataList[i] as JSONObject;
            GameObject levelPanelGO = Instantiate(levelPanelPrefab);
            levelPanelGO.transform.SetParent(levelContentPanelGO.transform);
            levelPanelGO.GetComponent<Controller_LevelPanel>().Initialize(i, levelData);
        }
    }
    public void ShowSelection() {
        inMenu = true;

        menuCanvasGO.SetActive(true);
        titleScreenGO.SetActive(false);
        selectionScreenGO.SetActive(true);
        gameCanvasGO.SetActive(false);
        pauseCanvasGO.SetActive(false);
        levelOverCanvasGO.SetActive(false);

        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;

        Time.timeScale = 0f;
    }
    public void HideSelection() {
        inMenu = true;

        menuCanvasGO.SetActive(true);
        titleScreenGO.SetActive(true);
        selectionScreenGO.SetActive(false);
        gameCanvasGO.SetActive(false);
        pauseCanvasGO.SetActive(false);
        levelOverCanvasGO.SetActive(false);

        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;

        Time.timeScale = 0f;
    }   
    public void ShowStats() {
        statsContentPanelGO.SetActive(!statsContentPanelGO.activeSelf);
    }
    public void ExitGame() {
        Debug.Log($"ExitGame");

        Application.Quit();
    }

    public void StartGame(string sceneName) {
        Debug.Log("StartGame");

        StartCoroutine(StartGameCoroutine(sceneName));
    }
    private IEnumerator StartGameCoroutine(string sceneName) {
        Debug.Log("StartGameCoroutine");

        inMenu = false;

        menuCanvasGO.SetActive(false);
        levelOverCanvasGO.SetActive(false);
        loadingCanvasGO.SetActive(true);

        Manager_Scene.Instance.LoadSceneByName(sceneName);

        yield return new WaitForSecondsRealtime(1f);

        inGame = true;

        loadingCanvasGO.SetActive(false);
        gameCanvasGO.SetActive(true);

        Manager_SaveLoad.Instance.SaveLevelUnlocked(sceneName);
        Manager_Effects.Instance.ResetDehydrationEffects();
        Manager_Timer.Instance.StartTimer();
        // Manager_Timeline.Instance.PlayCutscene_GameStart();
        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Time.timeScale = 1f;
    }

    public void PauseGame() {
        if (inGame && !inPause) {
            inGame = false;
            inPause = true;

            gameCanvasGO.SetActive(false);
            pauseCanvasGO.SetActive(true);
            
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;

            Time.timeScale = 0f;
        }
        else if (!inGame && inPause) {
            inPause = false;
            inGame = true;

            gameCanvasGO.SetActive(true);
            pauseCanvasGO.SetActive(false);
            
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            Time.timeScale = 1f;
        }
    }

    public void LevelOver() {
        Debug.Log($"LevelOver");

        levelOver = true;
        inGame = false;

        var rating = Manager_Game.Instance.CalculateScoreLetter();
        var currSceneName = Manager_Scene.Instance.GetCurrSceneName();
        SetScoreUI(rating);
        SetNextLevelEnabledUI(rating);
        Manager_SaveLoad.Instance.SaveLevelUnlocked(currSceneName, rating);
        
        levelOverCanvasGO.SetActive(true);
        gameCanvasGO.SetActive(false);
        
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
        
        Time.timeScale = 0f;
    }
    public void RetryLevel() {
        Debug.Log($"RetryLevel");

        var currSceneName = Manager_Scene.Instance.GetCurrSceneName();
        StartGame(currSceneName);
    }
    public void NextLevel() {
        Debug.Log($"NextLevel");

        var currSceneName = Manager_Scene.Instance.GetCurrSceneName();
        var nextSceneName = Manager_Scene.Instance.GetNextSceneName(currSceneName);
        StartGame(nextSceneName);
    }
    
    public void ShowMenu() {
        Debug.Log($"ShowMenu");
        
        StartCoroutine(StartMenuCoroutine());
    }
    private IEnumerator StartMenuCoroutine() {
        Debug.Log($"StartMenuCoroutine");

        inGame = false;

        loadingCanvasGO.SetActive(true);
        gameCanvasGO.SetActive(false);
        menuCanvasGO.SetActive(false);
        levelOverCanvasGO.SetActive(false);

        Manager_SaveLoad.Instance.LoadLevelData();
        Manager_Scene.Instance.LoadSceneByName("M");

        yield return new WaitForSecondsRealtime(1f);

        inMenu = true;

        loadingCanvasGO.SetActive(false);
        menuCanvasGO.SetActive(true);

        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;

        Time.timeScale = 0f;
    }

    public Sprite GetRatingSprite(bool playable, bool unlocked) {
        return playable && unlocked ? unlockedSprite : lockedSprite;
    }
    public Image GetDehydrationImageUI() {
        return dehydrationImage;
    }
    public Image GetDamageImageUI() {
        return damageImage;
    }

    public void SetNextLevelEnabledUI(string score) {
        if (score == "S+" || score == "S" || score == "A" || score == "B") {
            next_LevelOver_Button.interactable = true;
        }
        else if (score == "C" || score == "D" || score == "F") {
            next_LevelOver_Button.interactable = false;
        }
    }
    public void SetScoreUI(string score) {
        score_LevelOver_Text.text = score;
    }
    public void SetPeeAmountUI(float peeAmount) {
        peeImage.fillAmount = peeAmount;
    }
    public void SetMoneyUI(int currMoney) {
        money_Text.text = $"${currMoney}";
    }
    public void SetTimerUI(float time) {
        timer_Text.text = time.ToString("F0");
    }
    public void SetQTEKeyUI(string key) {
        qteKey_Text.text = key;
    }
    public void SetKidneyStoneUI(int stoneCnt) {
        kidneyStone_Text.text = stoneCnt.ToString();
    }

    #region Interaction Text
    public void SetShowText(string text) {
        lookedAt_Text.text = text;
    }
    public void ClearShowText() {
        lookedAt_Text.text = "";
    }
    public IEnumerator ShowTextWithSound(AudioSource source, AudioClip[] clips, float speed, string text) {
        lookedAt_Text.text = text;

        for (int lettersDisplayed = 0; lettersDisplayed <= text.Length; lettersDisplayed++) {
            lookedAt_Text.maxVisibleCharacters = lettersDisplayed;

            Helper.Instance.PlayRandAudio(source, clips);

            yield return new WaitForSeconds(speed);
        }
    }
    public IEnumerator ClearText(float speed) {
        int textLen = lookedAt_Text.text.Length;
        for (int lettersDisplayed = textLen; lettersDisplayed >= 0; lettersDisplayed--) {
            lookedAt_Text.maxVisibleCharacters = lettersDisplayed;

            yield return new WaitForSeconds(speed);
        }

        lookedAt_Text.text = "";
    }   
    #endregion

    #region Modals
    public void SpawnModal(string title, string content, string yesBtnTxt, string noBtnTxt, Action onYesClicked, Action onNoClicked) {
        GameObject modalGO = Instantiate(modalPrefab);

        var modalCanvas = GameObject.Find("Canvii/Modals Canvas");
        modalGO.transform.SetParent(modalCanvas.transform);

        RectTransform modalRect = modalGO.GetComponent<RectTransform>();
        modalRect.anchoredPosition = new Vector2(0, 0);

        var modalText = modalGO.transform.Find("Modal Text");
        var modalButtons = modalGO.transform.Find("Modal Buttons");

        var titleText = modalText.transform.Find("Title Text").GetComponent<TMP_Text>();
        titleText.text = title;
        
        var contentText = modalText.transform.Find("Content Text").GetComponent<TMP_Text>();
        contentText.text = content;

        var yesButton = modalButtons.transform.Find("Yes Button").GetComponent<Button>();
        yesButton.onClick.AddListener(() => {
            onYesClicked.Invoke();
            Destroy(modalGO);
        });

        var noButton = modalButtons.transform.Find("No Button").GetComponent<Button>();
        noButton.onClick.AddListener(() => {
            onNoClicked.Invoke();
            Destroy(modalGO);
        });

        var yesButtonText = yesButton.transform.Find("Yes Button Text").GetComponent<TMP_Text>();
        yesButtonText.text = yesBtnTxt;

        var noButtonText = noButton.transform.Find("No Button Text").GetComponent<TMP_Text>();
        noButtonText.text = noBtnTxt;
    }
    #endregion
}