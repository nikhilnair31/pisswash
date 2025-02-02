using UnityEngine;

public class Manager_Hazards : MonoBehaviour 
{
    #region Vars
    public static Manager_Hazards Instance { get; private set; }

    [Header("Damage Settings")]
    [SerializeField] private int damageCount;
    #endregion

    private void Awake() {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void AddDamage() {
        damageCount++;
        Manager_Effects.Instance.DamageEffects();
    }

    public int GetDamageCount() {
        return damageCount;
    }
}