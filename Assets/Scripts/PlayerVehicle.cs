using UnityEngine;

public class PlayerVehicle : MonoBehaviour, IPlayerVehicle
{
    public Transform groundRootTransform;
    public Rigidbody body;

    #region IPlayerVehicle

    public Transform GroundRootTransform { get { return groundRootTransform; } }
    public Rigidbody RigidBody { get { return body;} }

    #endregion

    void Awake()
    {
        if (groundRootTransform == null)
            groundRootTransform = this.transform;

        if (body == null)
            body = this.GetComponent<Rigidbody>();
    }
}
