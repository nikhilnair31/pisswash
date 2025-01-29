using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Controller_Stain : MonoBehaviour 
{
    #region Vars
    [Header("Control Properties")]
    [SerializeField] private float fadeAmountPerCollision = 0.01f;
    private DecalProjector decalProjector;
    #endregion

    private void Start() {
        decalProjector = GetComponent<DecalProjector>();
    }

    public void FadeOutAndDisable() {
        decalProjector.fadeFactor -= fadeAmountPerCollision;

        if (decalProjector.fadeFactor <= 0f) {
            decalProjector.fadeFactor = 0f;
            gameObject.SetActive(false);
        }
    }
}