using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;

public class Manager_UI : MonoBehaviour 
{
    #region Vars
    public static Manager_UI Instance { get; private set; }

    [Header("Menu UI")]
    [SerializeField] private GameObject menuCanvasGO;
    [SerializeField] private Button startGame_Menu_Button;
    [SerializeField] private Button exitGame_Menu_Button;

    [Header("Game UI")]
    [SerializeField] private GameObject gameCanvasGO;
    [SerializeField] private GameObject dehydratedImageGO;
    [SerializeField] private TMP_Text lookedAt_Text;
    private bool inGame = true;

    [Header("Pause UI")]
    [SerializeField] private GameObject pauseCanvasGO;

    [Header("Game Over UI")]
    [SerializeField] private GameObject gameoverCanvasGO;
    [SerializeField] private Button restartGame_GameOver_Button;
    [SerializeField] private Button exitGame_GameOver_Button;
    private bool gameOver = false;
    #endregion

    private void Awake() {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }
    
    private void Start() {
        startGame_Menu_Button?.onClick.AddListener(StartGame);
        exitGame_Menu_Button?.onClick.AddListener(ExitGame);

        restartGame_GameOver_Button?.onClick.AddListener(RestartGame);
        exitGame_GameOver_Button?.onClick.AddListener(ExitGame);
        
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
        Debug.Log($"StartGame");
        
        menuCanvasGO.SetActive(false);
        gameCanvasGO.SetActive(true);
        pauseCanvasGO.SetActive(false);
        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Manager_Timeline.Instance.PlayCutscene_GameStart();

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
        gameOver = true;
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

    public void SetDehydrated(bool active) {
        dehydratedImageGO.SetActive(active);
    }

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

            AudioClip clip = clips[Random.Range(0, clips.Length)];
            source.PlayOneShot(clip);

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
}