using UnityEngine;

public class ThrusterControl : MonoBehaviour 
{
    public DroneThruster droneThruster;
    public SmoothDroneControl smoothDroneControl;

    public Transform[] thrusters;
    private float[] angles = new float[4];
    private float[] angles2 = new float[4];
    public float maxAngleRotationComp;
    public float maxAngleThrust;
    public float angleLerpFactor = 5.0f;
    [Range(0, 1)]
    public float blend = 0.5f;
    [Range(1, 180.0f)]
    public float angleDenom = 30.0f;
    public float minDelayBeforeMatchingRotation = 0.1f;
    public float snapError = 0.1f;

    private float lastNotMatchTime = 0.0f;
    private bool previousMatching = false;

    void Awake() 
    {
        if (smoothDroneControl == null)
            smoothDroneControl = GetComponent<SmoothDroneControl>();

        if (droneThruster == null)
            droneThruster = GetComponent<DroneThruster>();

        for (int i = 0; i < angles.Length; i++)
        {
            angles[i] = 0.0f;
            angles2[i] = 0.0f;
        }
    }
    
    void Update() 
    {
        float thrustTargetAngle = 0.0f;
        float currentThrustAngle = 0.0f;
        float rotationTargetAngle = 0.0f;
        float blendTarget = 0.0f;
        if (GvrController.IsTouching)
        {
            thrustTargetAngle = -maxAngleThrust * droneThruster.ThrustFactor;
            blendTarget = 0.0f;
        }
        else
        {
            thrustTargetAngle = 0.0f;
            blendTarget = 1.0f;
        }

        blend = Mathf.Lerp(blend, blendTarget, angleLerpFactor * Time.deltaTime);

        Vector3 droneUp = this.transform.up;
        Vector3 projectedRotationAxis = Vector3.ProjectOnPlane(smoothDroneControl.rotationAxis, droneUp).normalized;

        if (previousMatching != smoothDroneControl.MatchingOrientation)
        {
            lastNotMatchTime = Time.time;
        }

        bool matchOnRotation = smoothDroneControl.MatchingOrientation && (Time.time - lastNotMatchTime) > minDelayBeforeMatchingRotation;

        for (int i = 0; i < thrusters.Length; i++)
        {
            var curRot = thrusters[i].localEulerAngles;
            currentThrustAngle = Mathf.Lerp(angles[i], thrustTargetAngle, angleLerpFactor * Time.deltaTime);
            angles[i] = currentThrustAngle;

            if (matchOnRotation)
            {
                Vector3 tDirFromCenter = Vector3.ProjectOnPlane(thrusters[i].position - this.transform.position, droneUp).normalized;
                float d = Vector3.Dot(tDirFromCenter, projectedRotationAxis);
                if (d > 0.0)
                    d = 1.0f - d;
                else
                    d = -1.0f - d;
                float factor = (smoothDroneControl.rotationAngle / angleDenom);
                float target = d * maxAngleRotationComp * factor;
                rotationTargetAngle = Mathf.Lerp(angles2[i], target, angleLerpFactor * Time.deltaTime);
                angles2[i] = rotationTargetAngle;
            }
            else
            {
                float target = 0.0f;
                rotationTargetAngle = Mathf.Lerp(angles2[i], target, angleLerpFactor * Time.deltaTime);
                angles2[i] = rotationTargetAngle;
            }

            float targetAngle = Mathf.Lerp(currentThrustAngle, rotationTargetAngle, blend);
            float angle = targetAngle;

            if (angle > maxAngleRotationComp)
                angle = maxAngleRotationComp;
            if (angle < -maxAngleRotationComp)
                angle = -maxAngleRotationComp;

            if (angle < snapError && angle > -snapError)
                angle = 0.0f;

            thrusters[i].localEulerAngles = new Vector3(angle, curRot.y, curRot.z);
        }

        previousMatching = smoothDroneControl.MatchingOrientation;
    }
}
