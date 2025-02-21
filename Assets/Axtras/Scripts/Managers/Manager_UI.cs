using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;
using System;
using SimpleJSON;
using UnityEngine.Events;
using DG.Tweening;

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
    [SerializeField] private RectTransform peePanelRectTrans;
    [SerializeField] private Image peeImage;
    [SerializeField] private TMP_Text lookedAt_Text;
    [SerializeField] private TMP_Text qteKey_Text;
    [SerializeField] private TMP_Text kidneyStone_Text;
    [SerializeField] private TMP_Text timer_Text;
    [SerializeField] private TMP_Text money_Text;
    [SerializeField] public bool inGame = false;

    [Header("Level Over UI")]
    [SerializeField] private GameObject levelOverCanvasGO;
    [SerializeField] private TMP_Text grade_LevelOver_Text;
    [SerializeField] private TMP_Text stats_LevelOver_Text;
    [SerializeField] private Button retry_LevelOver_Button;
    [SerializeField] private Button next_LevelOver_Button;
    [SerializeField] private Button menu_LevelOver_Button;
    [SerializeField] public bool levelOver = false;

    [Header("Level Over UI")]
    [SerializeField] private GameObject gameCompletedCanvasGO;
    [SerializeField] private Button menu_GameCompleted_Button;

    [Header("Pause UI")]
    [SerializeField] private GameObject pauseCanvasGO;
    [SerializeField] private Button backToMenu_Pause_Button;
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
            DontDestroyOnLoad(gameObject);
            Instance = this;
        }
        else
            Destroy(gameObject);
    }
    
    #region Initial
    private void Start() {
        SetupButtons();
    }
    private void SetupButtons() {
        startGame_Menu_Button?.onClick.AddListener(ShowSelection);
        backToMenu_Menu_Button?.onClick.AddListener(HideSelection);
        stats_Menu_Button?.onClick.AddListener(ShowStats);
        exitGame_Menu_Button?.onClick.AddListener(ExitGame);

        backToMenu_Pause_Button?.onClick.AddListener(ShowMenu);

        retry_LevelOver_Button?.onClick.AddListener(RetryLevel);
        next_LevelOver_Button?.onClick.AddListener(NextLevel);
        menu_LevelOver_Button?.onClick.AddListener(ShowMenu);

        menu_GameCompleted_Button?.onClick.AddListener(ShowMenu);
    } 
    #endregion

    #region Pause Related
    private void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            PauseGame();
        }
    }
    public void PauseGame() {
        if (inGame && !inPause) {
            inGame = false;
            inPause = true;

            gameCanvasGO.SetActive(false);
            pauseCanvasGO.SetActive(true);
            
            Manager_Timer.Instance.StopTimer(false);
            Manager_Audio.Instance.ControlAudioAmbient(true);
            
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;

            Time.timeScale = 0f;
        }
        else if (!inGame && inPause) {
            inPause = false;
            inGame = true;

            gameCanvasGO.SetActive(true);
            pauseCanvasGO.SetActive(false);
            
            Manager_Timer.Instance.StartTimer();
            Manager_Audio.Instance.ControlAudioAmbient(false);
            
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            Time.timeScale = 1f;
        }
    }
    #endregion
    
    #region General
    public void SpawnLevelPanels(JSONObject dataJson) {
        var sceneDataList = dataJson["sceneDataList"].AsArray;

        foreach (Transform child in levelContentPanelGO.transform) {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < sceneDataList.Count; i++) {
            JSONObject levelData = sceneDataList[i] as JSONObject;

            GameObject levelPanelGO = Instantiate(levelPanelPrefab);
            
            levelPanelGO.transform.SetParent(levelContentPanelGO.transform, false);
            levelPanelGO.transform.localScale = Vector3.one;
            levelPanelGO.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            
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

        Time.timeScale = 1f;
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

        yield return new WaitForSecondsRealtime(0.2f);

        inGame = true;

        loadingCanvasGO.SetActive(false);
        gameCanvasGO.SetActive(true);

        var data = Manager_SaveLoad.Instance.LoadLevelData();
        Manager_Money.Instance.UpdateMoney(data["haveMoney"]);

        Manager_Effects.Instance.ResetDehydrationEffects();
        Manager_Timer.Instance.StartTimer();
        
        Manager_Audio.Instance.ControlAudioAmbient(false);
        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Time.timeScale = 1f;
    }

    public void LevelOver() {
        Debug.Log($"LevelOver");

        levelOver = true;
        inGame = false;

        // Get stats and rating
        var (grade, stats) = Manager_Game.Instance.CalcStatAndGrade();
        
        // Set stats and grade for the level
        var currSceneName = Manager_Scene.Instance.GetCurrSceneName();
        var nextSceneName = Manager_Scene.Instance.GetNextSceneName(currSceneName);
        SetStatsUI(stats);
        SetGradeUI(grade);
        SetNextLevelEnabledUI(grade);
        Manager_SaveLoad.Instance.SaveLevelUnlocked(nextSceneName);
        
        Manager_Audio.Instance.ControlAudioAmbient(true);
        
        // Add money gained
        var money = Manager_Money.Instance.GetHasMoneyByRating(grade);
        Manager_Money.Instance.UpdateMoney(money);
        
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

        if (nextSceneName == "Done") {
            GameCompleted();
            return;
        }
        else {
            StartGame(nextSceneName);
        }
    }
    
    private void GameCompleted() {
        Debug.Log($"GameCompleted");

        inGame = false;

        gameCanvasGO.SetActive(false);
        levelOverCanvasGO.SetActive(false);
        gameCompletedCanvasGO.SetActive(true);

        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;

        Time.timeScale = 0f;
    }
    
    public void ShowMenu() {
        Debug.Log($"ShowMenu");
        
        StartCoroutine(StartMenuCoroutine());
    }
    private IEnumerator StartMenuCoroutine() {
        Debug.Log($"StartMenuCoroutine");

        inGame = false;

        loadingCanvasGO.SetActive(true);
        menuCanvasGO.SetActive(false);
        gameCanvasGO.SetActive(false);
        pauseCanvasGO.SetActive(false);
        levelOverCanvasGO.SetActive(false);

        var data = Manager_SaveLoad.Instance.LoadLevelData();
        SetStatsText(data);
        SpawnLevelPanels(data);

        Manager_Scene.Instance.LoadSceneByName("M");

        yield return new WaitForSecondsRealtime(1f);

        inMenu = true;
        
        Manager_Audio.Instance.ControlAudioAmbient(true);

        loadingCanvasGO.SetActive(false); 
        menuCanvasGO.SetActive(true);

        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;

        Time.timeScale = 1f;
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
    public RectTransform GetPeeRectTransfUI() {
        return peePanelRectTrans;
    }
    
    public void SetNextLevelEnabledUI(string score) {
        if (score != "F") {
            next_LevelOver_Button.interactable = true;
        }
        else {
            next_LevelOver_Button.interactable = false;
        }
    }
    public void SetStatsText(JSONObject dataJson) {
        var labelWidth = 25; // Adjust to fit your longest label

        var statsStr =
            $"{"Shifts Worked:".PadRight(labelWidth)} {dataJson["toalShiftWorked"] ?? 0}\n" +
            $"{"Cleaned Stains:".PadRight(labelWidth)} {dataJson["totalCleanedStains"] ?? 0}\n" +
            $"{"Peed Amount:".PadRight(labelWidth)} {dataJson["peedAmount"] ?? 0} fl oz\n" +
            $"\n" +
            $"{"Kidney Stones Created:".PadRight(labelWidth)} {dataJson["totalKidneyStonesCreated"] ?? 0}\n" +
            $"{"Kidney Stones Passed:".PadRight(labelWidth)} {dataJson["totalKidneyStonesPassed"] ?? 0}\n" +
            $"\n" +
            $"{"Total Slaps:".PadRight(labelWidth)} {dataJson["totalSlaps"] ?? 0}\n" +
            $"\n" +
            $"{"Drinks Bought:".PadRight(labelWidth)} {dataJson["drinksBought"] ?? 0}\n" +
            $"{"Drinks Stolen:".PadRight(labelWidth)} {dataJson["drinksStolen"] ?? 0}\n" +
            $"\n" +
            $"{"Have Money:".PadRight(labelWidth)} ${dataJson["haveMoney"] ?? 0}\n" +
            $"{"Spent Money:".PadRight(labelWidth)} ${dataJson["spentMoney"] ?? 0}\n";

        var statsText = statsContentPanelGO.GetComponent<TMP_Text>();
        statsText.text = statsStr;
    }
    public void SetStatsUI(string stats) {
        stats_LevelOver_Text.text = stats;
    }
    public void SetGradeUI(string score) {
        grade_LevelOver_Text.text = score;
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
    #endregion

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

            Manager_Audio.Instance.PlayAudio(source, clips);

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
    public void PauseDuringModal(bool active) {
        if (active) {
            Time.timeScale = 0f;
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
        }
        else {
            Time.timeScale = 1f;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    public void SpawnModal(Type_Tutorial tutorialSO, Action yesAction, Action noAction) {
        GameObject modalGO = Instantiate(modalPrefab);

        var modalCanvas = GameObject.Find("Canvii/Modals Canvas");
        modalGO.transform.SetParent(modalCanvas.transform);

        RectTransform modalRect = modalGO.GetComponent<RectTransform>();
        modalRect.anchoredPosition = new Vector2(0, 0);

        modalGO.transform.localScale = Vector3.zero;
        modalGO.transform.DOScale(Vector3.one, 0.2f).SetEase(Ease.InOutQuart).SetUpdate(true);

        var modalText = modalGO.transform.Find("Modal Text");
        var modalImages = modalGO.transform.Find("Modal Images");
        var modalButtons = modalGO.transform.Find("Modal Buttons");

        var titleText = modalText.transform.Find("Title Text").GetComponent<TMP_Text>();
        titleText.text = tutorialSO.titleStr;
        var contentText = modalText.transform.Find("Content Text").GetComponent<TMP_Text>();
        contentText.text = tutorialSO.contentStr;

        modalImages.gameObject.SetActive(tutorialSO.images.Length > 0);
        var imageImage = modalImages.transform.Find("Image Image").GetComponent<Image>();
        imageImage.sprite = tutorialSO.GetSprite();
        var leftButton = modalImages.transform.Find("Left Button").GetComponent<Button>();
        leftButton.onClick.AddListener(() => {
            imageImage.sprite = tutorialSO.GetSprite(-1);
        });
        leftButton.gameObject.SetActive(tutorialSO.images.Length > 1);
        var rightButton = modalImages.transform.Find("Right Button").GetComponent<Button>();
        rightButton.onClick.AddListener(() => {
            imageImage.sprite = tutorialSO.GetSprite(+1);
        });
        rightButton.gameObject.SetActive(tutorialSO.images.Length > 1);

        var yesButton = modalButtons.transform.Find("Yes Button").GetComponent<Button>();
        yesButton.onClick.AddListener(() => {
            yesAction?.Invoke();
            modalGO.transform.DOScale(Vector3.zero, 0.2f).SetEase(Ease.InOutQuart).OnComplete(() => Destroy(modalGO));
        });
        var noButton = modalButtons.transform.Find("No Button").GetComponent<Button>();
        noButton.onClick.AddListener(() => {
            noAction?.Invoke();
            modalGO.transform.DOScale(Vector3.zero, 0.2f).SetEase(Ease.InOutQuart).OnComplete(() => Destroy(modalGO));
        });

        var yesButtonText = yesButton.transform.Find("Yes Button Text").GetComponent<TMP_Text>();
        yesButtonText.text = tutorialSO.yesButtonStr;
        var noButtonText = noButton.transform.Find("No Button Text").GetComponent<TMP_Text>();
        noButtonText.text = tutorialSO.noButtonStr;
    }
    #endregion
}