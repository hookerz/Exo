using UnityEngine;

public class UpdateCone : MonoBehaviour
{
    public Material materialToUpdate;
    public Transform origin;
    public Transform end;

    [Range(1, 90)]
    public float halfAngle = 20.0f;

    void Start()
    {
        materialToUpdate.SetVector("_ConeOrigin", origin.position);
        materialToUpdate.SetVector("_ConeEnd", end.position);
        materialToUpdate.SetFloat("_ConeLength", Vector3.Distance(origin.position, end.position));
        // this won't change
        materialToUpdate.SetFloat("_ConeHalfAngle", halfAngle * Mathf.Deg2Rad);
        materialToUpdate.SetFloat("_ConeCosHalfAngle", Mathf.Cos(halfAngle * Mathf.Deg2Rad));
    }
    
    void Update()
    {
        materialToUpdate.SetVector("_ConeOrigin", origin.position);
        materialToUpdate.SetVector("_ConeEnd", end.position);
        materialToUpdate.SetFloat("_ConeLength", Vector3.Distance(origin.position, end.position));
    }

    void OnDisable()
    {
        materialToUpdate.SetFloat("_ConeLength", 0.0f);
    }

#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        if (origin != null && end != null)
            Gizmos.DrawLine(origin.position, end.position);
    }
#endif
}
