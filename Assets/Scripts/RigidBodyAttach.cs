using UnityEngine;

public class RigidBodyAttach : MonoBehaviour 
{
    public Transform attachTo;
    public Rigidbody body;
    public bool turnOffGravity = true;
    public bool makeKinematic = true;
    public bool disableGravirty = true;

    private bool previousGravityState = true;
    private bool previousKinematicState = false;
    
    void Start() 
    {
        if (attachTo == null && this.transform.parent != null)
            attachTo = this.transform.parent;

        if (body == null)
            body = GetComponent<Rigidbody>();
    }

    void OnEnable()
    {
        Start();

        if (body != null)
        {
            if (turnOffGravity)
                body.useGravity = false;

            if (makeKinematic)
                body.isKinematic = true;
        }
    }

    void OnDisable()
    {
        if (body != null)
        {
            if (turnOffGravity)
                body.useGravity = previousGravityState;
            if (makeKinematic)
                body.isKinematic = previousKinematicState;
        }
    }
    
    void FixedUpdate()
    {
        body.angularVelocity = Vector3.zero;
        body.velocity = Vector3.zero;
        body.MovePosition(attachTo.position);
        body.MoveRotation(attachTo.rotation);
    }
}
