using System.Collections.Generic;
using UnityEngine;

public class Controller_Bottle : Controller_Interactables 
{
    #region Vars
    [Header("Owner Settings")]
    [SerializeField] List<Controller_Drinker> ownerDrinkers = new ();

    [Header("Hydration Settings")]
    [SerializeField] private float increaseHydrationAmount = 10f;

    [Header("Money Settings")]
    [SerializeField] private bool canBeBought = true;
    [SerializeField] private int buyCost = 5;

    [Header("UI Settings")]
    [SerializeField] private string notEnoughMoneyStr = "Can't buy!";
    #endregion

    public void BuyBottle() {
        if (Manager_Money.Instance.GetCanBuy() && canBeBought) {
            Controller_Pee.Instance.AddPeeAmount(increaseHydrationAmount);
            Manager_Money.Instance.UpdateMoney(-buyCost);
            gameObject.SetActive(false);
        }
        else {
            Manager_Thoughts.Instance.ShowText(
                notEnoughMoneyStr, 
                1f,
                Manager_Thoughts.TextPriority.Player
            );
        }
    }
    public void StealBottle() {
        CheckIfOwnersSees();
        Manager_Money.Instance.UpdateMoney(0);
        gameObject.SetActive(false);
    }

    private void CheckIfOwnersSees() {
        foreach (Controller_Drinker cd in ownerDrinkers) {
            if (cd.GetCanSeePlayerStealing()) {
                Debug.Log($"SEEN! Slap and stun player");
                Controller_Player.Instance.GotSlapped();
            }
        }
    }
    public void AddOwner(Controller_Drinker cd) {
        canBeBought = false;
        ownerDrinkers.Add(cd);
    }
}