using UnityEngine;

public class SnappingHeadingTracker : MonoBehaviour
{
    public Transform headingTransform;
    public Transform transferTo;
    public float lerpFactor = 5.0f;
    public float segments = 6.0f;
        
    void Update()
    {
        Vector3 targetForward = Vector3.ProjectOnPlane(headingTransform.forward, Vector3.up);

        float angle = Vector3.Angle(Vector3.forward, targetForward);
        Vector3 cp = Vector3.Cross(Vector3.forward, targetForward);
        float target = Mathf.Round(angle / (360.0f / segments));
        float snappedAngle = target * 360.0f / segments;
        targetForward = Quaternion.AngleAxis(snappedAngle, cp) * Vector3.forward;
        
        Quaternion targetRotation = Quaternion.LookRotation(targetForward, Vector3.up);
        transferTo.rotation = Quaternion.Slerp(transferTo.rotation, targetRotation, Time.unscaledDeltaTime * lerpFactor);
    }
}
