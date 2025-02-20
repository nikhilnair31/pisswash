using UnityEngine;

public class Controller_Pee_Collision : MonoBehaviour 
{
    private void OnParticleCollision(GameObject other) {
        // Debug.Log($"OnParticleCollision: {other.name}");
        
        if (other.CompareTag("Stain")) {
            if (other.TryGetComponent(out Controller_Stain controllerStain)) {
                controllerStain.FadeOutAndDisable();
            }
        }
        if (other.CompareTag("Person")) {
            if (other.TryGetComponent(out Controller_Person person)) {
                person.AddPeedOnSound();
            }
        }
    }  
}