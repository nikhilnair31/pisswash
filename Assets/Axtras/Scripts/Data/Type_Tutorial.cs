using System;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "Tutorial", menuName = "Scriptable Objects/Tutorial", order = 0)]
public class Type_Tutorial : ScriptableObject 
{
    #region Vars
    [Header("Tutorial Settings")]
    [SerializeField] public string titleStr;
    [SerializeField] public string contentStr;
    [SerializeField] public Sprite[] images;
    [SerializeField] public string yesButtonStr;
    [SerializeField] public string noButtonStr;
    [SerializeField] public Action onYesClicked;
    [SerializeField] public Action onNoClicked;
    private int currImgInd = 0;
    #endregion

    public Sprite GetSprite(int moveVal = 0) {
        if (currImgInd + moveVal >= images.Length) 
            currImgInd = 0;
        else if (currImgInd + moveVal <= 0) 
            currImgInd = images.Length - 1;
        else 
            currImgInd += moveVal;
        return images[currImgInd];
    }
}