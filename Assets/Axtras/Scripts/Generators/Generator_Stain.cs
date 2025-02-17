#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Generator_Stain : MonoBehaviour 
{
    #region Vars
    [Header("Control Properties")]
    [SerializeField] private bool spawnDecals = false;
    [SerializeField] private bool clearDecals = false;

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
        if (spawnDecals) {
            spawnDecals = false;
            EditorApplication.delayCall += () => SpawnDecalsWithSphereRaycasts(
                source: transform,
                spawnAsChild: true,
                castlength: raycastLength
            );
        }
        if (clearDecals) {
            clearDecals = false;
            EditorApplication.delayCall += ClearDecals;
        }
    }
    #endif

    private void SpawnDecalsWithSphereRaycasts(Transform source, bool spawnAsChild = true, float castlength = 3f) {
        GameObject decalParent = null;
        for (int i = 0; i < numberOfRays; i++) {
            Vector3 randomDirection = Random.onUnitSphere;
            Ray ray = new (source.position, randomDirection);
            
            Debug.DrawRay(source.position, randomDirection * castlength, Color.red, 0.1f);

            if (Physics.Raycast(ray, out RaycastHit hit, castlength, decalLayerMask)) {
                if (currDecals < maxDecals) {
                    // Debug.DrawRay(hit.point, hit.normal * 1f, Color.green, 5f);

                    GameObject decalGO;
                    
                    #if UNITY_EDITOR
                        // Instantiate as a prefab instance
                        decalGO = (GameObject)PrefabUtility.InstantiatePrefab(damageDecalObject);
                    Undo.RegisterCreatedObjectUndo(decalGO, "Spawn Decal");
                    #else
                        decalGO = Instantiate(damageDecalObject);
                    #endif

                    if (spawnAsChild) {
                        decalGO.transform.SetParent(source);
                    }
                    else {
                        decalParent ??= new ("DrinkerDecals");
                        decalGO.transform.SetParent(decalParent.transform);
                    }

                    Vector3 decalPosition = hit.point + hit.normal * decalPivotOffset;
                    decalGO.transform.position = decalPosition;
                    
                    decalGO.transform.forward = hit.normal;
                    float randomRotation = Random.Range(0f, 360f);
                    decalGO.transform.Rotate(Vector3.forward, randomRotation, Space.Self);

                    var decal = decalGO.GetComponent<DecalProjector>();
                    float randomScale = Random.Range(randomScaleRange.x, randomScaleRange.y);
                    decal.size = new Vector3(randomScale, randomScale, decal.size.z);
                    
                    var coll = decalGO.GetComponent<BoxCollider>();
                    coll.size = new Vector3(randomScale, randomScale, coll.size.z);

                    currDecals++;

                    #if UNITY_EDITOR
                    Undo.RegisterCreatedObjectUndo(decalGO, "Spawn Decal"); // Allows Undo
                    EditorUtility.SetDirty(decalGO); // Marks the object as dirty
                    #endif
                }
            }
        }

        #if UNITY_EDITOR
        if (decalParent != null) {
            Undo.RegisterCreatedObjectUndo(decalParent, "Create Decal Parent");
            EditorUtility.SetDirty(decalParent);
        }
        #endif
    } 
    public void SpawnDecalsWithConeRaycasts(Transform source, bool spawnAsChild = false, float coneAngle = 45f) {
        GameObject decalParent = null;
        for (int i = 0; i < numberOfRays; i++) {
            Vector3 randomDirection = Random.insideUnitSphere;
            randomDirection = Vector3.Slerp(-source.up, randomDirection, Random.Range(0f, 1f));
            if (Vector3.Angle(-source.up, randomDirection) > coneAngle / 2) {
                randomDirection = Vector3.Slerp(-source.up, randomDirection, coneAngle / Vector3.Angle(-source.up, randomDirection));
            }

            Ray ray = new (source.position, randomDirection);
            Debug.DrawRay(source.position, randomDirection * raycastLength, Color.blue, 0.1f);

            if (Physics.Raycast(ray, out RaycastHit hit, raycastLength, decalLayerMask)) {
                if (currDecals < maxDecals) {
                    GameObject decalGO = Instantiate(
                        damageDecalObject, 
                        Vector3.zero, 
                        Quaternion.identity
                    );

                    if (spawnAsChild) {
                        decalGO.transform.SetParent(source);
                    }
                    else {
                        decalParent ??= new ("DrinkerDecals");
                        decalGO.transform.SetParent(decalParent.transform);
                    }

                    Vector3 decalPosition = hit.point + hit.normal * decalPivotOffset;
                    decalGO.transform.position = decalPosition;
                    
                    decalGO.transform.forward = hit.normal;
                    float randomRotation = Random.Range(0f, 360f);
                    decalGO.transform.Rotate(Vector3.forward, randomRotation, Space.Self);

                    var decal = decalGO.GetComponent<DecalProjector>();
                    float randomScale = Random.Range(randomScaleRange.x, randomScaleRange.y);
                    decal.size = new Vector3(randomScale, randomScale, decal.size.z);
                    
                    var coll = decalGO.GetComponent<BoxCollider>();
                    coll.size = new Vector3(randomScale, randomScale, coll.size.z);

                    currDecals++;
                }
            }
        }
    }
    private void ClearDecals() {
        while (transform.childCount > 0) {
            Transform child = transform.GetChild(0);
            #if UNITY_EDITOR
            DestroyImmediate(child.gameObject);
            #else
            Destroy(child.gameObject);
            #endif
        }

        currDecals = 0;
    }
}