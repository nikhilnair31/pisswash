using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Rendering.Universal;

[Serializable]
public class Stain {
    public DecalProjector stain;
    public float opacity;
}

public class Manager_Stains : MonoBehaviour 
{
    #region Vars
    public static Manager_Stains Instance { get; private set; }

    [Header("Stains Settings")]
    [SerializeField] private List<Stain> allStains;
    #endregion

    private void Awake() {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }    

    private void Start() {
        FindAllStains();
    }  
    private void FindAllStains() {
        var stains = FindObjectsByType<DecalProjector>(FindObjectsSortMode.None);
        foreach (var stain in stains) {
            if (stain.gameObject.CompareTag("Stain"))
                allStains.Add(new Stain { stain = stain, opacity = stain.fadeFactor });
        }
    }

    public float GetStainCleanedPerc() {
        int allStainCnt = allStains.Count;
        int cleanedStainCnt = 0;
        
        foreach (var stain in allStains) {
            if (stain.opacity <= 0)
                cleanedStainCnt++;
        }

        return (cleanedStainCnt / allStainCnt) * 100f;
    }
}