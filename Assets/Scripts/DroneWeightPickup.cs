using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public enum RelaseButton
{
    TouchPadClick,
    AppButton,
}

public class DroneWeightPickup : MonoBehaviour 
{
    public GameObject triggerVolumeTrackerObject;
    public Transform attachPoint;
    public Rigidbody attachToBody;
    public float animationTime = 1.0f;
    public RelaseButton releaseButton = RelaseButton.TouchPadClick;

    private WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
    private ITriggerVolumeTracker triggerVolumeTracker;
    private IAttachable attachable;

    public UnityEvent objectAttaching;
    public UnityEvent objectAttached;
    public UnityEvent objectDetached;

    void Awake()
    {
        if (triggerVolumeTrackerObject != null)
            triggerVolumeTrackerObject = this.gameObject;

        triggerVolumeTracker = triggerVolumeTrackerObject.GetComponent<ITriggerVolumeTracker>();
    }

    void OnEnable()
    {
        triggerVolumeTracker.ColliderEntered += TriggerVolumeTracker_ColliderEntered;
    }

    void OnDisable()
    {
        triggerVolumeTracker.ColliderEntered -= TriggerVolumeTracker_ColliderEntered;
    }

    private void TriggerVolumeTracker_ColliderEntered(GameObject sender, ITriggerVolumeTracker collector, Collider c)
    {
        if (attachable != null)
        {
            if (Debug.isDebugBuild)
                Debug.Log("Attached rejected due to exiting attachment: " + attachable + " on object: " + this.name);
            return;
        }

        if (c.attachedRigidbody == null)
        {
            Debug.LogError("collider object passed the layer filter but did not have an attached rigid body, skipping pickup: " + c.gameObject.name);
            return;
        }

        IAttachable a = c.attachedRigidbody.GetComponent<IAttachable>();
        if (a == null)
        {
            Debug.LogError("collider object passed the layer filter and had a rigid body but did not implement the required interface IAttachable, skipping pickup: " + c.attachedRigidbody.name);
            return;
        }

        if (!a.AllowAttachment)
        {
            if (Debug.isDebugBuild)
                Debug.Log("Attachable does not allow attachment at this time: " + a.Body.name);
            return;
        }

        attachable = a;
        attachable.Detached += Attachable_Detached;
        objectAttaching.Invoke();
        StartCoroutine(AnimateWeightIntoPlace(attachable, animationTime, Utils.SmoothStepEase));
    }

    private void Attachable_Detached(GameObject sender, IAttachable attachable)
    {
        this.attachable.Detached -= Attachable_Detached;
        this.attachable = null;
        objectDetached.Invoke();
    }

    void Update()
    {
        if (GvrController.State != GvrConnectionState.Connected)
            return;

        bool release = false;
        switch (releaseButton)
        {
            case RelaseButton.AppButton:
                release = GvrController.AppButtonUp;
                break;
            case RelaseButton.TouchPadClick:
                release = GvrController.ClickButtonUp;
                break;
        }

        if (release && attachable != null && attachable.IsAttached) // NOTE: its possible to be tracking the attachable while animating before actually being attached
        {
            attachable.Detach();
        }
    }

    public IEnumerator AnimateWeightIntoPlace(IAttachable attachable, float time, System.Func<float, float> easing)
    {
        Transform destinationTransform = attachPoint;
        Rigidbody body = attachable.Body;
        body.isKinematic = true;

        Vector3 localOffset = attachable.AttachPoint.InverseTransformPoint(body.position);
        Vector3 worldOffset = Vector3.zero;
        Quaternion localRotationOffset = Quaternion.Inverse(attachable.AttachPoint.rotation) * body.rotation;
        
        Vector3 startPosition = attachable.AttachPoint.position;
        Quaternion startRotation = attachable.AttachPoint.rotation;

        float startTime = Time.time;

        while (true)
        {
            float t = Mathf.Clamp01((Time.time - startTime) / time);
            float te = easing(t);

            Vector3 pos = Vector3.Lerp(startPosition, destinationTransform.position, te);
            Quaternion quat = Quaternion.Slerp(startRotation, destinationTransform.rotation, te);

            worldOffset = quat * localOffset;
            pos += worldOffset;

            Quaternion worldRotation = localRotationOffset * quat;

            body.MovePosition(pos);
            body.MoveRotation(worldRotation);

            if (t == 1.0f)
            {
                break;
            }

            yield return waitForFixedUpdate;
        }

        body.isKinematic = false;
        attachable.Attach(destinationTransform, attachToBody);
    }
}
