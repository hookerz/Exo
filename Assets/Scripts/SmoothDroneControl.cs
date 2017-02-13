using UnityEngine;
using System.Collections.Generic;

public enum TorqueCancelationMode
{
    ZeroAngularVelocity,
    NegativePreviousTorque
}

public class SmoothDroneControl : MonoBehaviour
{
    public DroneThruster thruster;
    public Rigidbody body;
    public ForceMode forceMode;
    public TorqueCancelationMode torqueCancelMode = TorqueCancelationMode.NegativePreviousTorque;
    public bool limitTorque = true;
    public float maxTorqueMag = 15.0f;
    public bool dontMatchUnlessMoving = true;
    public float minSpeedToMatchOrientation = 0.1f;
    public bool dontMatchWhenColliding = true;
    public LayerMask collisionMask;

    public bool controlThrusterJoints = false;
    public Transform[] thrusters;
    public float maxThrusterAngle;
    public float angleLerpFactor = 5.0f;

    private Vector3 prevTorque;
    public Vector3 rotationAxis;
    public float rotationAngle;

    private bool matchingOrientation = false;
    public bool MatchingOrientation
    {
        get { return matchingOrientation; }
    }

    public bool InContact
    {
        get { return HasActiveColliders(); }
    }

    private List<Collider> activeColliders = new List<Collider>();
    public bool Stopped { get { return stopped; } }
    private bool stopped;
    public float stoppingVelocityThreshold = 0.01f;
    public float minTimeBeforeStopping = 0;
    private float lastStopChange = 0;

    void Awake()
    {
        if (body == null) body = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (!controlThrusterJoints)
            return;

        Vector3 droneUp = this.transform.up;

        for (int i = 0; i < thrusters.Length; i++)
        {
            Vector3 tDirFromCenter = Vector3.ProjectOnPlane(thrusters[i].position - this.transform.position, droneUp).normalized;
            float d = Vector3.Dot(tDirFromCenter, Vector3.up);
            float targetAngle = -d * maxThrusterAngle;
            var curRot = thrusters[i].localEulerAngles;
            float angle = targetAngle;
            if (angle > maxThrusterAngle)
                angle = maxThrusterAngle;
            if (angle < -maxThrusterAngle)
                angle = -maxThrusterAngle;
            thrusters[i].localEulerAngles = new Vector3(angle, curRot.y, curRot.z);
        }
    }

    private bool HasActiveColliders()
    {
        for (int i = 0; i < activeColliders.Count; i++)
        {
            if (activeColliders[i].gameObject.activeInHierarchy && activeColliders[i].enabled)
                return true;
        }
        return false;
    }

    void FixedUpdate()
    {
        bool tentativeStopped = body.velocity.sqrMagnitude < (stoppingVelocityThreshold * stoppingVelocityThreshold);
        if (tentativeStopped != stopped)
        {
            if (!tentativeStopped || (Time.time >= (lastStopChange + minTimeBeforeStopping)))
            {
                stopped = tentativeStopped;
                lastStopChange = Time.time;

            }
        }

        matchingOrientation = false;
        if (GvrController.State != GvrConnectionState.Connected)
            return;

        // check for bad rotations, which seems to happen at startup consistently with v0.9.1
        if (GvrController.Orientation.w == 0.0f &&
            GvrController.Orientation.x == 0.0f &&
            GvrController.Orientation.y == 0.0f &&
            GvrController.Orientation.z == 0.0f)
        {
            return;
        }

        // don't allow orientation matching if no velocity, unless thrusting
        if (dontMatchUnlessMoving && body.velocity.magnitude < minSpeedToMatchOrientation && thruster.ThrustFactor == 0.0f)
        {
            return;
        }

        // don't orientation match if touching another object, unless thrusting
        if (dontMatchWhenColliding && HasActiveColliders() && thruster.ThrustFactor == 0.0f) // we have physical contact with something...
        {
            return;
        }

        matchingOrientation = true;

        switch (torqueCancelMode)
        {
            case TorqueCancelationMode.ZeroAngularVelocity:
                body.angularVelocity = Vector3.zero;
                break;
            case TorqueCancelationMode.NegativePreviousTorque:
                body.AddTorque(-prevTorque, forceMode);
                break;
        }

        Quaternion relativeRotation = GvrController.Orientation * Quaternion.Inverse(body.rotation);

        rotationAxis = Vector3.zero;
        rotationAngle = 0.0f;
        relativeRotation.ToAngleAxis(out rotationAngle, out rotationAxis);
        // we need the shortest rotation here, so if the angle is too big, flip it
        if (rotationAngle > 179.999999999999999f)
        {
            // flip the axis so we can keep the angle positive for torque limiting
            rotationAngle = 360.0f - rotationAngle;
            rotationAxis = -rotationAxis;
        }

        float angleRads = rotationAngle * Mathf.Deg2Rad;
        float torqueMag = angleRads / Time.fixedDeltaTime;
        if (limitTorque)
            torqueMag = Mathf.Clamp(torqueMag, 0.0f, maxTorqueMag);

        if (torqueMag < 0.0001f)
            return;

        Vector3 torque = rotationAxis * torqueMag;
        body.AddTorque(torque, forceMode);
        prevTorque = torque;
    }

    void OnDrawGizmos()
    {
        if (body == null)
            return;

        Vector3 x = GvrController.Orientation * Vector3.right;
        Vector3 y = GvrController.Orientation * Vector3.up;
        Vector3 z = GvrController.Orientation * Vector3.forward;

        Vector3 pt = body.transform.position;

        Gizmos.color = Color.red;
        Gizmos.DrawLine(pt, pt + x);
        Gizmos.color = Color.green;
        Gizmos.DrawLine(pt, pt + y);
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(pt, pt + z);

        Gizmos.color = Color.magenta;
        Gizmos.DrawLine(pt, pt + rotationAxis);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (dontMatchWhenColliding)
        {
            int collisionColliderLayer = 1 << collision.collider.gameObject.layer;
            if ((collisionColliderLayer & collisionMask.value) == 0)
                return;

            if (!activeColliders.Contains(collision.collider))
            {
                activeColliders.Add(collision.collider);
            }
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (dontMatchWhenColliding)
        {
            int collisionColliderLayer = 1 << collision.collider.gameObject.layer;
            if ((collisionColliderLayer & collisionMask.value) == 0)
                return;

            if (activeColliders.Contains(collision.collider))
            {
                activeColliders.Remove(collision.collider);
            }
        }
    }
}
