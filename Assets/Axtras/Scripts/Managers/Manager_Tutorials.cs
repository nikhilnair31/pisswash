using UnityEngine;
using DG.Tweening;

public class Manager_Tutorials : MonoBehaviour 
{
    #region Vars
    public static Manager_Tutorials Instance { get; private set; }

    private bool showingTutorials = false;
    private bool shownTutorials = false;
    
    [Header("Tutorial Settings")]
    public Type_Tutorial playerTutorial;
    public Type_Tutorial drinkTutorial;
    #endregion

    private void Awake() {
        if (Instance == null){
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
    }

    public void PlayTutorial() {
        if (shownTutorials) 
            return;
 
        var check = PlayerPrefs.GetInt("Tutorials-ShowControls");
        if (check == 1) {
            shownTutorials = true;
        }
        else {
            var sequence = DOTween.Sequence().SetId("Tutorials-ShowControls");
            sequence
            // Wait a second
            .AppendInterval(1.0f)
            .AppendCallback(() => {
                // Pause time and enable cursor
                Time.timeScale = 0f;
                Cursor.lockState = CursorLockMode.Confined;
                Cursor.visible = true;

                // Flash modal
                Manager_UI.Instance.SpawnModal(playerTutorial);
            })
            // Mark it shown
            .OnComplete(() => {
                PlayerPrefs.SetInt("Tutorials-ShowControls", 1);
            });
        }
    }
    private void OnYesClicked() {
        Debug.Log("Yes button pressed! Do something.");
        // Call any function from a scene object
    }
    private void OnNoClicked() {
        Debug.Log("No button pressed! Do something else.");

        // Unpause time and disable cursor
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    
    public bool GetIfShowingTutorials() {
        return showingTutorials;
    }
}