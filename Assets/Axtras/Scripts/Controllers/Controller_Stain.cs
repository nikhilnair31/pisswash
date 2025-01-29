using UnityEngine;

public class Controller_Stain : MonoBehaviour 
{
    #region Vars
    [Header("Spawn Properties")]
    [SerializeField] private bool spawnBloodDecals = false;

    [Header("Decal Properties")]
    [SerializeField] private GameObject damageDecalObject;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private LayerMask decalLayerMask;
    [SerializeField] private Vector2 randomScaleRange = new (0.5f, 1.5f);
    [SerializeField] private float rotationOffsetRange = 360f;
    [SerializeField] private float decalPivotOffset = 0.05f;
    [SerializeField] private float raycastLength = 5f;
    [SerializeField] private int numberOfRays = 20;
    [SerializeField] private int maxDecals = 20;
    private int currDecals = 0;
    #endregion
    
    #if UNITY_EDITOR
    private void OnValidate() {
        if (spawnBloodDecals) {
            spawnBloodDecals = false;
            SpawnBloodDecalsWithSphereRaycasts();
        }
    }
    #endif

    private void SpawnBloodDecalsWithSphereRaycasts() {
        for (int i = 0; i < numberOfRays; i++) {
            Vector3 randomDirection = Random.onUnitSphere;
            Ray ray = new (transform.position, randomDirection);
            
            Debug.DrawRay(transform.position, randomDirection * raycastLength, Color.red, 0.1f);

            if (Physics.Raycast(ray, out RaycastHit hit, raycastLength, decalLayerMask)) {
                if (currDecals < maxDecals) {
                    // Debug.DrawRay(hit.point, hit.normal * 1f, Color.green, 5f);

                    GameObject decal = Instantiate(
                        damageDecalObject, 
                        Vector3.zero, 
                        Quaternion.identity
                    );

                    decal.transform.SetParent(transform);

                    Vector3 decalPosition = hit.point + hit.normal * decalPivotOffset;
                    decal.transform.position = decalPosition;
                    
                    decal.transform.forward = hit.normal;
                    float randomRotation = Random.Range(0f, 360f);
                    decal.transform.Rotate(Vector3.forward, randomRotation, Space.Self);

                    float randomScale = Random.Range(randomScaleRange.x, randomScaleRange.y);
                    decal.transform.localScale = new Vector3(randomScale, randomScale, decal.transform.localScale.z);

                    currDecals++;
                }
            }
        }
    } 
}