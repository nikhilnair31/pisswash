using UnityEngine;

public class Controller_Pee_Collision : MonoBehaviour 
{
    private void OnParticleCollision(GameObject other) {
        Debug.Log($"Pee collided with {other.name}");
        if (other.CompareTag("Stain")) {
            if (other.TryGetComponent(out Controller_Stain controllerStain)) {
                controllerStain.FadeOutAndDisable();
            }
        }
    }  
}