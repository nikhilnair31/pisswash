using UnityEngine;
using DG.Tweening;

public class Manager_Tutorials : MonoBehaviour 
{
    #region Vars
    public static Manager_Tutorials Instance { get; private set; }

    private bool shownTutorials = false;
    
    [Header("Tutorial Settings")]
    public Type_Tutorial playerTutorial;
    #endregion

    private void Awake() {
        if (Instance == null){
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
    }

    private void OnYesClicked() {
        Debug.Log("Yes button pressed! Do something.");
        // Call any function from a scene object
    }
    private void OnNoClicked() {
        Debug.Log("No button pressed! Do something else.");
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
                playerTutorial.Show(OnYesClicked, OnNoClicked);
            })
            // Mark it shown
            .OnComplete(() => {
                PlayerPrefs.SetInt("Tutorials-ShowControls", 1);
            });
        }
    }
}