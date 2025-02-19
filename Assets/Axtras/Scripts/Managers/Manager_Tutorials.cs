using UnityEngine;
using DG.Tweening;

public class Manager_Tutorials : MonoBehaviour 
{
    #region Vars
    public static Manager_Tutorials Instance { get; private set; }

    private bool showingTutorials = false;
    
    [Header("Tutorial Settings")]
    public Type_Tutorial playerMoveLookTutorial;
    public Type_Tutorial playerPeeTutorial;
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

    public void PlayTutorial(string key) {
        Debug.Log($"PlayTutorial");
        var check = PlayerPrefs.GetInt(key);
        if (check != 1) {
            var sequence = DOTween.Sequence().SetId(key);
            sequence
            // Wait a second
            .AppendInterval(1.0f)
            .AppendCallback(() => {
                // Flash modal
                Manager_UI.Instance.PauseDuringModal(true);
                Manager_UI.Instance.SpawnModal(playerMoveLookTutorial, OnYesClicked, null);
            })
            // Mark it shown
            .OnComplete(() => {
                PlayerPrefs.SetInt(key, 1);
            });
        }
    }
    private void OnYesClicked() {
        Manager_UI.Instance.SpawnModal(playerPeeTutorial, null, OnNoClicked);
    }
    private void OnNoClicked() {
        Manager_UI.Instance.PauseDuringModal(false);
    }
    
    public bool GetIfShowingTutorials() {
        return showingTutorials;
    }
}