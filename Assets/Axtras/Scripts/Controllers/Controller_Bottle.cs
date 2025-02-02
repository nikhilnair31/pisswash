using UnityEngine;
using System.Collections.Generic;

public class Controller_Bottle : Controller_Interactables 
{
    #region Vars
    [Header("Status Settings")]
    [SerializeField] public bool bought = false;
    [SerializeField] public bool stolen = false;

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
        if (Manager_Money.Instance.GetHasMoneyToBuy() && canBeBought) {
            bought = true;
            
            Manager_Money.Instance.UpdateMoney(-buyCost);
            Controller_Pee.Instance.AddPeeAmount(increaseHydrationAmount);
            
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
        stolen = true;

        // Check if owner sees player stealing
        foreach (Controller_Drinker cd in ownerDrinkers) {
            if (cd.GetCanSeePlayerStealing()) {
                Manager_Hazards.Instance.AddDamage();

                if (cd.transform.TryGetComponent(out Generator_Stain gen)) {
                    gen.SpawnDecalsWithConeRaycasts(
                        source: cd.GetPlayerSeeSource(),
                        spawnAsChild: false,
                        coneAngle: 60f
                    );
                }
            }
        }

        Manager_Money.Instance.UpdateMoney(0);
        Controller_Pee.Instance.AddPeeAmount(increaseHydrationAmount);

        gameObject.SetActive(false);
    }

    public void AddOwner(Controller_Drinker cd) {
        canBeBought = false;
        ownerDrinkers.Add(cd);
    }
}