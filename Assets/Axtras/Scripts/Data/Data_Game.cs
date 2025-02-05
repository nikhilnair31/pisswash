using System;
using UnityEngine;
using System.Collections.Generic;

[Serializable]
public class Data_Game
{
    [Header("Scenes")]
    public List<Data_Scene> sceneDataList;
    
    [Header("Stats")]
    public int toalShiftWorked;
    public int totalCleanedStains;
    public int totalKidneyStonesCreated;
    public int totalKidneyStonesPassed;
    public int totalSlaps;
    public int bottlesBought;
    public int bottlesStolen;
    public int haveMoney;
    public int spentMoney;

    public Data_Game() {
        toalShiftWorked = 0;
        totalCleanedStains = 0;
        totalKidneyStonesCreated = 0;
        totalKidneyStonesPassed = 0;
        totalSlaps = 0;
        bottlesBought = 0;
        bottlesStolen = 0;
        haveMoney = 0;
        spentMoney = 0;
    }
}
