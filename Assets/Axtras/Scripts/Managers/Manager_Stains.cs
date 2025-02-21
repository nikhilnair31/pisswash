using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Rendering.Universal;

public class Manager_Stains : MonoBehaviour 
{
    #region Vars
    public static Manager_Stains Instance { get; private set; }

    public enum StainType { Acid, Booze, Puke }

    [Header("Stains Settings")]
    [SerializeField] public List<Type_Stain> stainTypesSO;
    [SerializeField] private List<Transform> allStainTransforms;
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
                allStainTransforms.Add(stain.transform);
        }
    }

    public float GetStainCleanedPerc() {
        int allStainCnt = allStainTransforms.Count;
        int cleanedStainCnt = 0;
        
        foreach (var stain in allStainTransforms) {
            if (stain.TryGetComponent(out DecalProjector stainDecal)) {
                if (stainDecal.fadeFactor <= 0) {
                    cleanedStainCnt++;
                }
            }
        }

        var perc = (float)cleanedStainCnt / allStainCnt * 100f;
        Debug.Log($"allStainCnt: {allStainCnt}\ncleanedStainCnt: {cleanedStainCnt}\nperc: {perc}");

        return perc;
    }
}