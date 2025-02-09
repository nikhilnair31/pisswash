using UnityEngine;

public class Controller_Person : MonoBehaviour 
{
    #region Variables
    [SerializeField] protected SkinnedMeshRenderer rend;
    [SerializeField] protected Animator anim;
    [SerializeField] protected GameObject player;

    [SerializeField] private Material[] skinMats;
    protected Material mat;
    #endregion

    protected virtual void Start() {
        TryGetComponent(out anim);
        
        player = GameObject.FindWithTag("Player");
        
        int randInd = Random.Range(0, skinMats.Length);
        rend.sharedMaterial = skinMats[randInd];
    }
}