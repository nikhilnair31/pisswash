using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;

public class Controller_Drink : Controller_Interactables 
{
    #region Vars
    [Header("Status Settings")]
    [SerializeField] private bool canBeBought = true;
    [SerializeField] private bool canBeStolen = true;
    public bool bought = false;
    public bool stolen = false;

    [Header("Type Settings")]
    [SerializeField] private Manager_Drinks.DrinkType drinkType;
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
            case Manager_Drinks.DrinkType.Beer:
                selectedDrink = Manager_Drinks.Instance.drinkTypesSO.FirstOrDefault(tb => tb.type == Manager_Drinks.DrinkType.Beer);
                break;
            case Manager_Drinks.DrinkType.Whiskey:
                selectedDrink = Manager_Drinks.Instance.drinkTypesSO.FirstOrDefault(tb => tb.type == Manager_Drinks.DrinkType.Whiskey);
                break;
            case Manager_Drinks.DrinkType.Mystery:
                selectedDrink = Manager_Drinks.Instance.drinkTypesSO.FirstOrDefault(tb => tb.type == Manager_Drinks.DrinkType.Mystery);
                break;
        }
    }

    public void ConsumeDrink(string buyOrSteal) {
        if (buyOrSteal == "buy")
            TryBuyDrink();
        
        if (buyOrSteal == "steal")
            StealDrink();
    }
    private void TryBuyDrink() {
        var hasMoney = Manager_Money.Instance.GetHasMoneyToBuy();

        if (hasMoney && canBeBought && !bought) {
            bought = true;
            
            // Increase hydration
            var isPeeFull = Controller_Pee.Instance.GetIsPeeFull();
            if (!isPeeFull)
                Controller_Pee.Instance.AddPeeAmount(selectedDrink.increaseHydrationAmount);

            // Update money
            Manager_Money.Instance.UpdateMoney(-selectedDrink.buyCost);
            // Update drinks bought
            Manager_SaveLoad.Instance.SaveStatData("drinksBought", "add", 1);

            // Play the consumption effect
            selectedDrink.StartConsumptionEffect();

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
        var isPeeFull = Controller_Pee.Instance.GetIsPeeFull();
        if (!isPeeFull)
            Controller_Pee.Instance.AddPeeAmount(selectedDrink.increaseHydrationAmount);
        
        // Update money
        Manager_Money.Instance.UpdateMoney(0);
        // Update drinks stolen
        Manager_SaveLoad.Instance.SaveStatData("drinksStolen", "add", 1);

        // Play the consumption effect
        selectedDrink.StartConsumptionEffect();

        gameObject.SetActive(false);
    }

    public void AddOwner(Controller_Drinker cd) {
        canBeBought = false;
        ownerDrinkers.Add(cd);
    }
}