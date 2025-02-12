using UnityEngine;
using System.Collections.Generic;

public class Manager_Drinks : MonoBehaviour 
{
    #region Vars
    public static Manager_Drinks Instance { get; private set; }

    [Header("Drinks Settings")]
    [SerializeField] public List<Type_Drink> drinkTypesSO;
    [SerializeField] private List<Controller_Drink> allDrinks;
    #endregion

    private void Awake() {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }    

    private void Start() {
        FindAllDrinks();
    }  
    private void FindAllDrinks() {
        var bottles = FindObjectsByType<Controller_Drink>(FindObjectsSortMode.None);
        foreach (var Drink in bottles) {
            if (Drink.gameObject.CompareTag("Drink"))
                allDrinks.Add(Drink);
        }
    }

    public int GetDrinksStolenCount() {
        int stolenDrinksCnt = 0;
        
        foreach (var Drink in allDrinks) {
            if (Drink.stolen)
                stolenDrinksCnt++;
        }

        return stolenDrinksCnt;
    } 
}