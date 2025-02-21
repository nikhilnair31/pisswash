using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class Controller_Drinker : Controller_Person 
{
    #region Variables
    private List<Controller_Drink> theirDrinks = new ();

    [Header("Stealing Settings")]
    [SerializeField] private Transform seeFromTransform;
    [SerializeField] private float canSeeInRange = 5f;
    [SerializeField] private float canSeeInAngle = 110f;
    
    [Header("Stun Settings")]
    [SerializeField] private float stunTimeAmt = 3f;
    
    [Header("UI Settings")]
    [SerializeField] private Image drinkerImage;
    [SerializeField] private Sprite canSeeSprite;
    [SerializeField] private Sprite cantSeeSprite;
    
    [Header("Audio Settings")]
    [SerializeField] private AudioClip[] caughtClips;
    #endregion

    protected override void Start() {
        base.Start();
                
        CheckForNearbyDrinks();
    }
    private void CheckForNearbyDrinks() {
        Collider[] colliders = Physics.OverlapSphere(seeFromTransform.position, canSeeInRange, LayerMask.GetMask("Interactable"));
        foreach (Collider col in colliders) {
            if (col.TryGetComponent(out Controller_Drink Drink)) {
                theirDrinks.Add(Drink);
                Drink.AddOwner(this);
            }
        }
    }

    private void Update() {
        CheckForPlayer();
        CheckForPlayerStealing();
    }
    private void CheckForPlayer() {
        if (GetCanSeePlayer()) {
            drinkerImage.sprite = canSeeSprite;
        }
        else {
            drinkerImage.sprite = cantSeeSprite;
        }
    }
    private void CheckForPlayerStealing() {
        var newDrinks = new List<Controller_Drink>(theirDrinks);
        foreach (Controller_Drink drink in newDrinks) {
            if (drink.GetIsStolen()) {
                PlayerStole();
                drink.RemoveOwner(this);
                theirDrinks.Remove(drink);
            }
        }
    }
    private void PlayerStole() {
        Manager_Effects.Instance.StartStunEffectsSeq(stunTimeAmt);
        Manager_Drinks.Instance.SetStealSlap();
        Manager_SaveLoad.Instance.SaveStatData("totalSlaps", "add", 1);
        
        Manager_Audio.Instance.PlayAudio(audioSource, caughtClips);

        if (transform.TryGetComponent(out Generator_Stain gen)) {
            gen.SpawnDecalsWithConeRaycasts(
                source: GetPlayerSeeSource(),
                spawnAsChild: false,
                coneAngle: 60f
            );
        }
    }
    
    private bool GetCanSeePlayer() {
        if (!player) return false;

        Vector3 toPlayer = player.transform.position - seeFromTransform.position;
        toPlayer.y = 0;
        float angle = Vector3.Angle(seeFromTransform.forward, toPlayer);
        // Debug.Log($"Angle: {angle}");

        return angle <= canSeeInAngle;
    }
    private Transform GetPlayerSeeSource() {
        return seeFromTransform;
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.green;
        foreach (Controller_Drink drink in theirDrinks) {
            Gizmos.DrawLine(seeFromTransform.position, drink.transform.position);
        }
    }
}