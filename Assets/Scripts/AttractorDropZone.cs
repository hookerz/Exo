using UnityEngine;
using System.Collections;

public class AttractorDropZone : MonoBehaviour, IDropZone, ICompletableObstacle
{
    public TriggerVolumeTracker triggerVolumeTracker;
    public Transform forceLocation;
    public Rigidbody acceptableBody; // require a specific object...
    public float maxForce = 2.0f;
    public float maxDist = 2.0f;
    public float velocityFactor = 0.9f;
    public float triggerThreshold = 0.01f;
    public float snapThreshold = 0.0001f;
    public bool allowPickup = false;
    public float maxTimeTriggerTime = 0.2f;
    public float maxSnapTime = 1.0f;
    public GameObject defaultGeometry;
    public GameObject animatedGeometry;
    public GameObject outGeometry;
    public float animationTime = 2.75f;

    public ForceMode forceMode = ForceMode.Impulse;
    
    private WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
    private CompletableObstacleState obstacleState;

    public ForceField forceField;
    public float forceFieldTimeout = 2;
    public float rejectionForce = 10.0f;
    public float rejectionWaitTime = 2.0f;
    public ForceMode rejectionForceMode;

    #region ICompletableObstacle

    public event ObstacleStateChangedDelegate StateChanged;

    public GameObject GameObject { get { return this.gameObject; } }

    public CompletableObstacleState ObstacleState
    {
        get { return this.obstacleState; }
        set
        {
            if (this.obstacleState == value)
                return;

            var oldState = this.obstacleState;
            this.obstacleState = value;
            if (StateChanged != null)
                StateChanged(this, oldState, value);
        }
    }

    public void Activate()
    {
        ObstacleState = CompletableObstacleState.Active;
    }

    public void Deactivate()
    {
        ObstacleState = CompletableObstacleState.NotActive;
    }

    public void Reset()
    {
        Activate();
    }

    #endregion

    #region IDropZone

    public event AttachableDroppedDelegate AttachableDropped;

    #endregion

    void Awake()
    {
        if (triggerVolumeTracker == null)
            triggerVolumeTracker = GetComponent<TriggerVolumeTracker>();

        if (forceLocation == null)
            forceLocation = this.transform;

        if (forceField != null)
            forceField.enabled = false;
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
        if (c.attachedRigidbody == null)
            return;

        var a = c.attachedRigidbody.gameObject.GetComponent<IAttachable>();
        if (a == null)
        {
            Debug.LogError("Rigidbody object does not have component that implements IAttachable: " + c.attachedRigidbody.name);
            return;
        }

        StartCoroutine(AttractRigidBodyCoroutine(c.attachedRigidbody, a));
    }

    IEnumerator AnimateIntoPlace(Rigidbody body)
    {
        defaultGeometry.SetActive(false);
        animatedGeometry.SetActive(true);
        animatedGeometry.GetComponent<Animator>().SetTrigger("Play");
        if (forceField != null)
        {
            forceField.enabled = true;
            StartCoroutine(Utils.DelayedAction(() =>
            {
                forceField.enabled = false;
            }, forceFieldTimeout));
        }
        yield return new WaitForSeconds(animationTime);
        body.detectCollisions = false;
        animatedGeometry.SetActive(false);
        outGeometry.SetActive(true);
    }

    IEnumerator RejectBody(Rigidbody body, IAttachable a)
    {
        body.MovePosition(forceLocation.position);
        body.velocity = Vector3.zero;
        body.isKinematic = true; // prevent others from moving it while we wait...
        // TODO: progressively shake?
        yield return new WaitForSeconds(rejectionWaitTime);
        // this is pretty specific right now, need to make more generic
        body.isKinematic = false;
        a.AllowAttachment = true;
        body.useGravity = true;
        // reject towards the camera
        Vector3 towardsCameraXY = Vector3.ProjectOnPlane(Camera.main.transform.position - forceLocation.position, Vector3.up).normalized;
        Vector3 towardsCameraAndUp = (towardsCameraXY + Vector3.up).normalized; // 45 up towards camera...
        body.AddForce(rejectionForce * towardsCameraAndUp, rejectionForceMode);
        ObstacleState = CompletableObstacleState.Failed;
        yield return null;
        ObstacleState = CompletableObstacleState.Active;
    }

    IEnumerator AttractRigidBodyCoroutine(Rigidbody body, IAttachable a)
    {
        Debug.Log("Attaching body with attractor forces: " + body);

        if (body.useGravity)
            body.useGravity = false;

        if (a.IsAttached)
            a.Detach();

        // prevent future accidental attachment
        a.AllowAttachment = false;
        bool firedEvent = false;
        float startTime = Time.time;

        while (true)
        {
            Vector3 diff = forceLocation.position - body.position;
            float dist = diff.magnitude;
            float ellapsed = Time.time - startTime;

            if (!firedEvent && (dist < triggerThreshold || ellapsed > maxTimeTriggerTime))
            {
                if (acceptableBody != null && acceptableBody != body)
                {
                    Debug.Log("Rejecting body as it does not match only acceptable body: " + acceptableBody.name);
                    StartCoroutine(RejectBody(body, a));
                    yield break;
                }

                if (AttachableDropped != null)
                    AttachableDropped(a);

                ObstacleState = CompletableObstacleState.Complete;
                StartCoroutine(AnimateIntoPlace(body));
                Debug.Log("Completing DropZone: " + this.name);

                firedEvent = true;
            }

            if (ellapsed < maxSnapTime)
            {
                Vector3 dir = diff / dist;

                float t = Mathf.Clamp01(dist / maxDist);
                float mag = t * maxForce;

                body.velocity = body.velocity * velocityFactor;
                body.AddForce(mag * dir, forceMode);
            }
            else
            {
                Debug.Log("Snapping DropZone: " + this.name);
                body.MovePosition(forceLocation.position);
                break;
            }

            yield return waitForFixedUpdate;
        }

        body.velocity = Vector3.zero;

        if (allowPickup)
            a.AllowAttachment = true;
        else
        {
            body.gameObject.AddComponent<FixedJoint>();
        }
    }
}
