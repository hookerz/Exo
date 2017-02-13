using UnityEngine;

public class HeadingTracker : MonoBehaviour
{
    public Transform headingTransform;
    public Transform transferTo;
    public float lerpFactor = 5.0f;

    void Update()
    {
        Vector3 targetForward = Vector3.ProjectOnPlane(headingTransform.forward, Vector3.up);
        Quaternion targetRotation = Quaternion.LookRotation(targetForward, Vector3.up);
        transferTo.rotation = Quaternion.Slerp(transferTo.rotation, targetRotation, Time.unscaledDeltaTime * lerpFactor);
    }
}
