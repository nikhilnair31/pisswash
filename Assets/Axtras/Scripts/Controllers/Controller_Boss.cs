using UnityEngine;

public class Controller_Boss : Controller_Interactables 
{
    #region Vars
    [SerializeField] internal float rotationSpeed = 1f;
    #endregion

    private void Update() {
        Vector3 direction = transform.position - playerController.transform.position;
        direction.y = 0; // Fix the y axis

        if (direction != Vector3.zero) {
            Quaternion rotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * rotationSpeed);
        }
    }

    public void FinshRound() {
        Manager_UI.Instance.LevelOver();
        // Stop timer
        Manager_Timer.Instance.StopTimer(false);
    }
}