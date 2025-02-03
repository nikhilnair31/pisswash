using System;
using UnityEngine;
using System.Collections.Generic;

[Serializable]
public class Data_Game
{
    [Header("Scenes")]
    public List<Data_Scene> sceneDataList;
    
    [Header("Stats")]
    public string toalShiftWorked;
    public string totalCleanedStains;
    public string totalSlaps;
    public string totalKidneyStonesCreated;
    public string totalKidneyStonesPassed;
    public string timeSpentDrinking;
    public string bottlesStolen;

    public Data_Game() {
        toalShiftWorked = "0";
        totalCleanedStains = "0";
        totalSlaps = "0";
        totalKidneyStonesCreated = "0";
        totalKidneyStonesPassed = "0";
        timeSpentDrinking = "0";
        bottlesStolen = "0";
    }
}
