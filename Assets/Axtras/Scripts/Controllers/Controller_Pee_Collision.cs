using UnityEngine;

public class Controller_Pee_Collision : MonoBehaviour 
{
    private void OnParticleCollision(GameObject other) {
        if (other.CompareTag("Stain")) {
            if (other.TryGetComponent(out Controller_Stain controllerStain)) {
                controllerStain.FadeOutAndDisable();
            }
        }
    }  
}