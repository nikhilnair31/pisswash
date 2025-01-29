using UnityEngine;

public class Controller_Bottle : Controller_Interactables 
{
    #region Vars
    [Header("Hydration Settings")]
    [SerializeField] private float increaseHydrationAmount = 10f;

    [Header("Money Settings")]
    [SerializeField] private int buyCost = 5;

    [Header("UI Settings")]
    [SerializeField] private string notEnoughMoneyStr = "Not enough money!";
    #endregion

    public void BuyBottle() {
        if (Manager_Money.Instance.GetCanBuy()) {
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
        Manager_Money.Instance.UpdateMoney(0);
        gameObject.SetActive(false);
    }
}