using UnityEngine;

public class Controller_Boss : Controller_Interactables 
{
    public void FinshRound() {
        Manager_UI.Instance.LevelOver();
    }
}