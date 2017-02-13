using UnityEngine;

public class ForceField : MonoBehaviour
{
    public Rigidbody drone;
    public float force = 1000;
    public float radius = 7.5f;
    public Collider targetCollider;
    protected bool inForceField;
    public bool enableWithTargetCollider = false;
    [HideInInspector]
    public bool on = false;
    public bool forceAlongVelocity = false;
    public ForceMode forceMode = ForceMode.Acceleration;

    virtual protected void Start()
    {
        GameObject droneObj = GameObject.FindGameObjectWithTag("Player");
        drone = droneObj.GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (inForceField && on)
        {
            float forceMagnitude = force;
            Vector3 baseDirection = -drone.position + Vector3.up * drone.transform.position.y;
            if (forceAlongVelocity && drone.velocity.magnitude > 0.01f)
            {
                baseDirection = drone.velocity.normalized;
                baseDirection.y = 0;
            }
            drone.AddForce(((baseDirection).normalized) * forceMagnitude, forceMode);
        }
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider == targetCollider)
        {
            inForceField = true;

            if(enableWithTargetCollider)
                on = true;
        }
    }

    void OnTriggerExit(Collider collider)
    {
        if (collider == targetCollider)
        {
            inForceField = false;

            if (enableWithTargetCollider)
                on = false;
        }
    }
}
