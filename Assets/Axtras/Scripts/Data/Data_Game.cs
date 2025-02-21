using System;
using UnityEngine;
using System.Collections.Generic;

[Serializable]
public class Data_Game
{
    [Header("Scenes")]
    public List<Data_Scene> sceneDataList;
    
    [Header("Bonuses")]
    public bool stickyfingers;
    public bool stoneCold;
    public bool speedRunner;
    public bool perfectionist;
    public bool hydroHomie;
    public bool ninja;
    public bool economical;
    
    [Header("Stats")]
    public int toalShiftWorked;
    public int totalCleanedStains;
    public int totalKidneyStonesCreated;
    public int totalKidneyStonesPassed;
    public int totalSlaps;
    public int drinksBought;
    public int drinksStolen;
    public int haveMoney;
    public int spentMoney;
    public float peedAmount;

    public Data_Game() {
        stickyfingers = false;
        stoneCold = false;
        speedRunner = false;
        perfectionist = false;
        hydroHomie = false;
        ninja = false;
        economical = false;

        toalShiftWorked = 0;
        totalCleanedStains = 0;
        totalKidneyStonesCreated = 0;
        totalKidneyStonesPassed = 0;
        totalSlaps = 0;
        drinksBought = 0;
        drinksStolen = 0;
        haveMoney = 0;
        spentMoney = 0;
        peedAmount = 0f;
    }
}
