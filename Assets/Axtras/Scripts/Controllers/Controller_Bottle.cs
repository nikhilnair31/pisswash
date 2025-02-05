using UnityEngine;
using System.Collections.Generic;

public class Controller_Bottle : Controller_Interactables 
{
    #region Vars
    public enum BottleType { Beer, Whiskey, Mystery }

    [Header("Status Settings")]
    [SerializeField] public bool bought = false;
    [SerializeField] public bool stolen = false;

    [Header("Owner Settings")]
    [SerializeField] List<Controller_Drinker> ownerDrinkers = new ();

    [Header("Hydration Settings")]
    [SerializeField] private BottleType bottleType;
    [SerializeField] private float increaseHydrationAmount = 10f;

    [Header("Money Settings")]
    [SerializeField] private bool canBeBought = true;
    [SerializeField] private int buyCost = 5;

    [Header("UI Settings")]
    [SerializeField] private string notEnoughMoneyStr = "Can't buy!";
    #endregion

    private void Start() {
        InitBottle();
    }
    private void InitBottle() {
        switch (bottleType) {
            case BottleType.Beer:
                break;
            case BottleType.Whiskey:
                break;
            case BottleType.Mystery:
                break;
        }
    }

    public void ConsumeBottle(string buyOrSteal) {
        if (buyOrSteal == "buy")
            BuyBottle();
        else if (buyOrSteal == "steal")
            StealBottle();
    }
    private void BuyBottle() {
        var hasMoney = Manager_Money.Instance.GetHasMoneyToBuy();
        var isPeeFull = Controller_Pee.Instance.GetIsPeeFull();
        var canBuy = canBeBought;

        if (hasMoney && canBuy) {
            bought = true;
            
            // Increase hydration
            if (!isPeeFull)
                Controller_Pee.Instance.AddPeeAmount(increaseHydrationAmount);

            // Update money
            Manager_Money.Instance.UpdateMoney(-buyCost);
            // Update bottles bought
            Manager_SaveLoad.Instance.SaveStatData("bottlesBought", "add", 1);
            
            gameObject.SetActive(false);
        }
        else {
            // Show not enough money text
            Manager_Thoughts.Instance.ShowText(
                notEnoughMoneyStr, 
                1f,
                Manager_Thoughts.TextPriority.Player
            );
        }
    }
    private void StealBottle() {
        stolen = true;
        var isPeeFull = Controller_Pee.Instance.GetIsPeeFull();

        // Check if owner sees player stealing
        foreach (Controller_Drinker cd in ownerDrinkers) {
            if (cd.GetCanSeePlayerStealing()) {
                Manager_Hazards.Instance.AddDamage();
                Manager_SaveLoad.Instance.SaveStatData("totalSlaps", "add", 1);

                if (cd.transform.TryGetComponent(out Generator_Stain gen)) {
                    gen.SpawnDecalsWithConeRaycasts(
                        source: cd.GetPlayerSeeSource(),
                        spawnAsChild: false,
                        coneAngle: 60f
                    );
                }
            }
        }

        // Increase hydration
        if (!isPeeFull)
            Controller_Pee.Instance.AddPeeAmount(increaseHydrationAmount);
        
        // Update money
        Manager_Money.Instance.UpdateMoney(0);
        // Update bottles stolen
        Manager_SaveLoad.Instance.SaveStatData("bottlesStolen", "add", 1);

        gameObject.SetActive(false);
    }

    public void AddOwner(Controller_Drinker cd) {
        canBeBought = false;
        ownerDrinkers.Add(cd);
    }
}