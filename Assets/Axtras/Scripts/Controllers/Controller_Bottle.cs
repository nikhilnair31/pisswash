using UnityEngine;

public class Controller_Bottle : Controller_Interactables 
{
    #region Vars
    [Header("Hydration Settings")]
    [SerializeField] private float increaseHydrationAmount = 10f;

    [Header("Money Settings")]
    [SerializeField] private int buyCost = 5;
    #endregion

    public void BuyBottle() {
        Manager_Money.Instance.UpdateMoney(-buyCost);
        gameObject.SetActive(false);
    }
    public void StealBottle() {
        Manager_Money.Instance.UpdateMoney(0);
        gameObject.SetActive(false);
    }
    public float GetHydrationAmount() {
        return increaseHydrationAmount;
    }
}