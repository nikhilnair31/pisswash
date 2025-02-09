using UnityEngine;
using DG.Tweening;

public class Controller_Crowd : Controller_Person 
{
    #region Variables
    #endregion

    protected override void Start() {
        base.Start();
        
        transform.localEulerAngles = new Vector3(0, Random.Range(0, 360), 0);
        
        transform
            .DOMoveY(
                0.5f, 
                Random.Range(0.5f, 1f)
            )
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.OutBounce)
            .SetDelay(Random.Range(0f, 1f));

        anim.SetBool("IsDancing", true);
    }
}