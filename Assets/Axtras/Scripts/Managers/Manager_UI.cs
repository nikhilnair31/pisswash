using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;
using System;

public class Manager_UI : MonoBehaviour 
{
    #region Vars
    public static Manager_UI Instance { get; private set; }

    [Header("Menu UI")]
    [SerializeField] private GameObject menuCanvasGO;
    [SerializeField] private Button startGame_Menu_Button;
    [SerializeField] private Button startTutorial_Menu_Button;
    [SerializeField] private Button exitGame_Menu_Button;
    [SerializeField] public bool inMenu = true;

    [Header("Game UI")]
    [SerializeField] private GameObject gameCanvasGO;
    [SerializeField] private Image dehydratedImage;
    [SerializeField] private Image peeImage;
    [SerializeField] private TMP_Text lookedAt_Text;
    [SerializeField] private TMP_Text qteKey_Text;
    [SerializeField] private TMP_Text timer_Text;
    [SerializeField] private TMP_Text money_Text;
    [SerializeField] public bool inGame = false;

    [Header("Finished Round UI")]
    [SerializeField] private GameObject finishedRoundCanvasGO;
    [SerializeField] private TMP_Text score_FinishedRound_Text;
    [SerializeField] private Button next_FinishedRound_Button;
    [SerializeField] private Button menu_FinishedRound_Button;
    [SerializeField] public bool finishedRound = false;

    [Header("Game Over UI")]
    [SerializeField] private GameObject gameoverCanvasGO;
    [SerializeField] private Button restartGame_GameOver_Button;
    [SerializeField] private Button exitGame_GameOver_Button;
    [SerializeField] public bool gameOver = false;

    [Header("Pause UI")]
    [SerializeField] private GameObject pauseCanvasGO;
    [SerializeField] public bool inPause = false;

    [Header("Loading UI")]
    [SerializeField] private GameObject loadingCanvasGO;

    [Header("Modals")]
    [SerializeField] private GameObject modalPrefab;
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
        InitMenu();
    }
    private void SetupButtons() {
        startGame_Menu_Button?.onClick.AddListener(StartGame);
        startTutorial_Menu_Button?.onClick.AddListener(StartTutorial);
        exitGame_Menu_Button?.onClick.AddListener(ExitGame);

        next_FinishedRound_Button?.onClick.AddListener(NextRound);
        menu_FinishedRound_Button?.onClick.AddListener(ToMenu);

        restartGame_GameOver_Button?.onClick.AddListener(RestartGame);
        exitGame_GameOver_Button?.onClick.AddListener(ExitGame);
    }
    private void InitMenu() {
        inMenu = true;

        menuCanvasGO.SetActive(true);
        gameCanvasGO.SetActive(false);
        pauseCanvasGO.SetActive(false);
        finishedRoundCanvasGO.SetActive(false);
        gameoverCanvasGO.SetActive(false);

        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;

        Time.timeScale = 0f;
    }
    
    private void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            PauseGame();
        }
    }
    
    public void StartGame() {
        Debug.Log("StartGame");

        StartCoroutine(StartGameCoroutine());
    }
    public void StartTutorial() {
        Debug.Log($"StartTutorial");

        StartCoroutine(StartGameCoroutine());
    }
    private IEnumerator StartGameCoroutine() {
        Debug.Log("StartGameCoroutine");

        inMenu = false;

        menuCanvasGO.SetActive(false);
        loadingCanvasGO.SetActive(true);

        Manager_Scene.Instance.LoadCurrentScene();

        yield return new WaitForSecondsRealtime(1f);

        inGame = true;

        loadingCanvasGO.SetActive(false);
        gameCanvasGO.SetActive(true);

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

    public void FinishRound() {
        Debug.Log($"FinishRound");

        finishedRound = true;
        inGame = false;

        SetScoreUI();
        
        finishedRoundCanvasGO.SetActive(true);
        gameCanvasGO.SetActive(false);
        
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
        
        Time.timeScale = 0f;
    }
    public void NextRound() {
        Debug.Log($"NextRound");
    }
    public void ToMenu() {
        Debug.Log($"ToMenu");
    }

    public void GameOver() {
        Debug.Log($"GameOver");

        gameOver = true;
        inGame = false;
        inMenu = false;

        gameoverCanvasGO.SetActive(true);
        gameCanvasGO.SetActive(false);
        pauseCanvasGO.SetActive(false);
        
        Time.timeScale = 0f;
        
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
    }
    public void RestartGame() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);  
        Time.timeScale = 1f;
    }
    public void ExitGame() {
        Debug.Log($"ExitGame");

        Application.Quit();
    }

    public void SetScoreUI() {
        score_FinishedRound_Text.text = Manager_Game.Instance.CalculateScoreLetter();
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
    public void SetDehydratedUI(bool active) {
        dehydratedImage.enabled = active;
    }
    public void SetQTEKeyUI(string key) {
        qteKey_Text.text = key;
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