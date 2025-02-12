using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Controller_Drink : Controller_Interactables 
{
    #region Vars
    public enum DrinkType { Beer, Whiskey, Mystery }

    [Header("Status Settings")]
    [SerializeField] private bool canBeBought = true;
    [SerializeField] private bool canBeStolen = true;
    public bool bought = false;
    public bool stolen = false;

    [Header("Type Settings")]
    [SerializeField] private DrinkType drinkType;
    private Type_Drink selectedDrink;

    private readonly List<Controller_Drinker> ownerDrinkers = new ();
    private readonly string notEnoughMoneyStr = "Can't buy!";
    #endregion

    private void Start() {
        InitDrink();
    }
    private void InitDrink() {
        selectedDrink = null;
        switch (drinkType) {
            case DrinkType.Beer:
                selectedDrink = Manager_Drinks.Instance.drinkTypesSO.FirstOrDefault(tb => tb.type == DrinkType.Beer);
                break;
            case DrinkType.Whiskey:
                selectedDrink = Manager_Drinks.Instance.drinkTypesSO.FirstOrDefault(tb => tb.type == DrinkType.Whiskey);
                break;
            case DrinkType.Mystery:
                selectedDrink = Manager_Drinks.Instance.drinkTypesSO.FirstOrDefault(tb => tb.type == DrinkType.Mystery);
                break;
        }
    }

    public void ConsumeDrink(string buyOrSteal) {
        if (buyOrSteal == "buy")
            BuyDrink();
        else if (buyOrSteal == "steal")
            StealDrink();
    }
    private void BuyDrink() {
        var hasMoney = Manager_Money.Instance.GetHasMoneyToBuy();
        var isPeeFull = Controller_Pee.Instance.GetIsPeeFull();
        var canBuy = canBeBought;

        if (hasMoney && canBuy) {
            bought = true;
            
            // Increase hydration
            if (!isPeeFull)
                Controller_Pee.Instance.AddPeeAmount(selectedDrink.increaseHydrationAmount);

            // Update money
            Manager_Money.Instance.UpdateMoney(-selectedDrink.buyCost);
            // Update drinks bought
            Manager_SaveLoad.Instance.SaveStatData("drinksBought", "add", 1);
            
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
    private void StealDrink() {
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
            Controller_Pee.Instance.AddPeeAmount(selectedDrink.increaseHydrationAmount);
        
        // Update money
        Manager_Money.Instance.UpdateMoney(0);
        // Update drinks stolen
        Manager_SaveLoad.Instance.SaveStatData("drinksStolen", "add", 1);

        gameObject.SetActive(false);
    }

    public void AddOwner(Controller_Drinker cd) {
        canBeBought = false;
        ownerDrinkers.Add(cd);
    }
}