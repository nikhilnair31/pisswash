using System.Collections.Generic;
using UnityEngine;

public class Controller_Drinker : Controller_Person 
{
    #region Variables
    private List<Controller_Drink> theirDrinks = new ();

    [Header("Stealing Settings")]
    [SerializeField] private Transform seeFromTransform;
    [SerializeField] private float canSeeInRange = 5f;
    [SerializeField] private float canSeeInAngle = 110f;
    
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

    public void CheckForPlayerStealing() {
        Manager_Effects.Instance.StartStunEffectsSeq(5f);
        Manager_Drinks.Instance.SetStealSlap();
        Manager_SaveLoad.Instance.SaveStatData("totalSlaps", "add", 1);
        
        Manager_Audio.Instance.PlayRandAudio(audioSource, caughtClips);

        if (transform.TryGetComponent(out Generator_Stain gen)) {
            gen.SpawnDecalsWithConeRaycasts(
                source: GetPlayerSeeSource(),
                spawnAsChild: false,
                coneAngle: 60f
            );
        }
    }
    
    public bool GetCanSeePlayerStealing() {
        if (!player) return false;

        Vector3 toPlayer = player.transform.position - seeFromTransform.position;
        toPlayer.y = 0;
        float angle = Vector3.Angle(seeFromTransform.forward, toPlayer);
        Debug.Log($"Angle: {angle}");

        return angle <= canSeeInAngle;
    }
    public Transform GetPlayerSeeSource() {
        return seeFromTransform;
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.green;
        foreach (Controller_Drink drink in theirDrinks) {
            Gizmos.DrawLine(seeFromTransform.position, drink.transform.position);
        }
    }
}