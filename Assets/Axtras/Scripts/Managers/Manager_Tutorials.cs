using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;

public class Manager_Tutorials : MonoBehaviour 
{
    #region Vars
    public static Manager_Tutorials Instance { get; private set; }

    private bool showingTutorials = false;

    [Header("Tutorial Settings")]
    public List<Type_Tutorial> tutorials;  // List of tutorials to go through
    private int currentTutorialIndex = 0;  // To track the current tutorial

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

        // Check if the tutorial has been shown before
        // var check = PlayerPrefs.GetInt(key);
        // if (check != 1) {
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
        // }
    }

    private void ShowTutorial(Type_Tutorial tutorial) {
        Manager_UI.Instance.SpawnModal(tutorial, OnYesClicked, null);
    }

    private void OnYesClicked() {
        // Move to next tutorial
        currentTutorialIndex++;

        // If we have reached the end of the list, close the tutorial process
        if (currentTutorialIndex < tutorials.Count) {
            ShowTutorial(tutorials[currentTutorialIndex]);
        } else {
            Manager_UI.Instance.PauseDuringModal(false);  // End tutorials
            currentTutorialIndex = 0;  // Reset the index for next time
        }
    }

    private void OnNoClicked() {
        Manager_UI.Instance.PauseDuringModal(false);
    }
    
    public bool GetIfShowingTutorials() {
        return showingTutorials;
    }
}