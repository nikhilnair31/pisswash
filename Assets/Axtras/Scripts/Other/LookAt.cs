using UnityEngine;

[ExecuteInEditMode]
public class LookAt : MonoBehaviour
{
    public enum FaceDirection
    {
        Forward,
        Up,
        Left
    }

    public FaceDirection faceDirection = FaceDirection.Forward;
    public bool fixX = false;
    public bool fixY = false;
    public bool fixZ = false;

    void Update() {
        if (Camera.main != null) {
            Vector3 direction = Vector3.forward;
            switch (faceDirection)
            {
                case FaceDirection.Forward:
                    direction = Vector3.forward;
                    break;
                case FaceDirection.Up:
                    direction = Vector3.up;
                    break;
                case FaceDirection.Left:
                    direction = Vector3.left;
                    break;
            }

            Vector3 targetPosition = transform.position + Camera.main.transform.rotation * direction;
            Vector3 lookAtPosition = new Vector3(
                fixX ? transform.position.x : targetPosition.x,
                fixY ? transform.position.y : targetPosition.y,
                fixZ ? transform.position.z : targetPosition.z
            );

            transform.LookAt(
                lookAtPosition, 
                Camera.main.transform.rotation * Vector3.up
            );
        }
    }
}