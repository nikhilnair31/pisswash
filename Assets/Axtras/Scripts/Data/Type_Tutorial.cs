using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Tutorial", menuName = "Scriptable Objects/Tutorial", order = 0)]
public class Type_Tutorial : ScriptableObject 
{
    #region Vars
    [Header("Tutorial Settings")]
    [SerializeField] public string titleStr;
    [SerializeField] public string contentStr;
    [SerializeField] public string yesButtonStr;
    [SerializeField] public string noButtonStr;
    private Action yesAction;
    private Action noAction;
    #endregion

    public void Show(Action onYesClicked, Action onNoClicked) {
        if (Manager_UI.Instance != null) {
            yesAction = onYesClicked;
            noAction = onNoClicked;

            Manager_UI.Instance.SpawnModal(
                titleStr,
                contentStr,
                yesButtonStr,
                noButtonStr,
                yesAction,
                noAction
            );
        } 
        else {
            Debug.LogError("Manager_UI Instance not found! Make sure it exists in the scene.");
        }
    }
}