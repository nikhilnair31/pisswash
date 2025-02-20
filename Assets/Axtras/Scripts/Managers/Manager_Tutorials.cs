using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;

public class Manager_Tutorials : MonoBehaviour 
{
    #region Vars
    public static Manager_Tutorials Instance { get; private set; }

    private bool showingTutorials = false;

    [Header("Tutorial Settings")]
    public List<Type_Tutorial> tutorials;
    private int currentTutorialIndex = 0;

    #endregion

    private void Awake() {
        if (Instance == null){
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
    }

    public void PlayTutorial(string key) {
        Debug.Log($"PlayTutorial");

        // Check if in tutorial scene
        var inTutorialMenu = Manager_Scene.Instance.GetCurrSceneName().Contains("T");
        if (inTutorialMenu) {
            var sequence = DOTween.Sequence().SetId(key);
            sequence
            // Wait a second
            .AppendInterval(1.0f)
            .AppendCallback(() => {
                // Flash modal
                Manager_UI.Instance.PauseDuringModal(true);
                ShowTutorial(tutorials[currentTutorialIndex]);
            })
            // Mark it shown
            .OnComplete(() => {
                PlayerPrefs.SetInt(key, 1);
            });
        }
    }

    private void ShowTutorial(Type_Tutorial tutorial) {
        Manager_UI.Instance.SpawnModal(tutorial, OnYesClicked, OnNoClicked);
    }

    private void OnYesClicked() {
        currentTutorialIndex++;

        if (currentTutorialIndex < tutorials.Count) {
            ShowTutorial(tutorials[currentTutorialIndex]);
        } 
        else {
            Manager_UI.Instance.PauseDuringModal(false);
            currentTutorialIndex = 0;
        }
    }

    private void OnNoClicked() {
        Manager_UI.Instance.PauseDuringModal(false);
        currentTutorialIndex = 0;
    }
    
    public bool GetIfShowingTutorials() {
        return showingTutorials;
    }
}