using UnityEngine;
using System.Collections.Generic;

public class Manager_Bottles : MonoBehaviour 
{
    #region Vars
    public static Manager_Bottles Instance { get; private set; }

    [Header("Bottles Settings")]
    [SerializeField] private List<Controller_Bottle> allBottles;
    #endregion

    private void Awake() {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }    

    private void Start() {
        FindAllBottles();
    }  
    private void FindAllBottles() {
        var bottles = FindObjectsOfType<Controller_Bottle>();
        foreach (var bottle in bottles) {
            if (bottle.gameObject.CompareTag("Bottle"))
                allBottles.Add(bottle);
        }
    }

    public int GetBottlesStolenCount() {
        int stolenBottlesCnt = 0;
        
        foreach (var bottle in allBottles) {
            if (bottle.stolen)
                stolenBottlesCnt++;
        }

        return stolenBottlesCnt;
    } 
    public int GetMaxBottlesPerLevel() {
        return allBottles.Count;
    } 
}