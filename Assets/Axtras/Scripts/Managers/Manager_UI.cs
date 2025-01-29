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
    private bool inMenu = true;

    [Header("Game UI")]
    [SerializeField] private GameObject gameCanvasGO;
    [SerializeField] private GameObject dehydratedImageGO;
    [SerializeField] private TMP_Text lookedAt_Text;
    [SerializeField] private TMP_Text timer_Text;
    private bool inGame = false;

    [Header("Pause UI")]
    [SerializeField] private GameObject pauseCanvasGO;

    [Header("Game Over UI")]
    [SerializeField] private GameObject gameoverCanvasGO;
    [SerializeField] private Button restartGame_GameOver_Button;
    [SerializeField] private Button exitGame_GameOver_Button;
    private bool gameOver = false;

    [Header("Modals")]
    [SerializeField] private GameObject modalPrefab;
    #endregion

    private void Awake() {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
    
    private void Start() {
        startGame_Menu_Button?.onClick.AddListener(StartGame);
        startTutorial_Menu_Button?.onClick.AddListener(StartTutorial);
        exitGame_Menu_Button?.onClick.AddListener(ExitGame);

        restartGame_GameOver_Button?.onClick.AddListener(RestartGame);
        exitGame_GameOver_Button?.onClick.AddListener(ExitGame);
        
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;

        inMenu = true;

        Time.timeScale = 0f;
    }
    
    private void Update() {
        if (Input.GetKeyDown(KeyCode.Escape) && !gameOver && !inMenu) {
            PauseGame();
        }
    }
    
    public void StartGame() {
        Debug.Log($"StartGame");

        inGame = true;
        inMenu = false;
        
        menuCanvasGO.SetActive(false);
        gameCanvasGO.SetActive(true);
        pauseCanvasGO.SetActive(false);
        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Manager_Timeline.Instance.PlayCutscene_GameStart();

        Time.timeScale = 1f;
    }
    public void StartTutorial() {
        Debug.Log($"StartTutorial");

        inGame = true;
        inMenu = false;
        
        menuCanvasGO.SetActive(false);
        gameCanvasGO.SetActive(true);
        pauseCanvasGO.SetActive(false);

        Manager_Timer.Instance.StartTimer();
        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Time.timeScale = 1f;
    }

    public void PauseGame() {
        if (inGame) {
            inGame = false;
            gameCanvasGO.SetActive(false);
            pauseCanvasGO.SetActive(true);
            Time.timeScale = 0f;
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
        }
        else {
            inGame = true;
            gameCanvasGO.SetActive(true);
            pauseCanvasGO.SetActive(false);
            Time.timeScale = 1f;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    public void GameOver() {
        Debug.Log($"GameOver");

        gameOver = true;
        inGame = false;
        inMenu = false;

        gameCanvasGO.SetActive(false);
        pauseCanvasGO.SetActive(false);
        gameoverCanvasGO.SetActive(true);
        
        Time.timeScale = 0f;
        
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
    }
    public void RestartGame() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);  
        Time.timeScale = 1f;
    }
    public void ExitGame() {
        Application.Quit();
    }

    public void SetTimer(float time) {
        timer_Text.text = time.ToString("F0");
    }
    public void SetDehydrated(bool active) {
        dehydratedImageGO.SetActive(active);
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