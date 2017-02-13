using UnityEngine;

public class FixedJointAttachable : MonoBehaviour, IAttachable
{
    public Transform attachmentPoint;
    public Rigidbody body;

    private bool allowAttachment = true;
    private Rigidbody attachedToBody;
    private FixedJoint connectingJoint;

    private Vector3 localPositionOffset = Vector3.zero;
    private Quaternion localRotationOffset = Quaternion.identity;

    void Awake()
    {
        if (attachmentPoint == null)
            attachmentPoint = this.transform;

        if (body == null)
            body = this.GetComponent<Rigidbody>();

        localPositionOffset = attachmentPoint.InverseTransformPoint(body.position);
        localRotationOffset = Quaternion.Inverse(attachmentPoint.rotation) * body.rotation;
    }

    #region IAttachable

    public event AttachableEvent Attached;
    public event AttachableEvent Detached;

    public Transform AttachPoint { get { return this.attachmentPoint; } }
    public Rigidbody Body { get { return this.body; } }
    public bool IsAttached { get { return attachedToBody != null; } }
    public bool AllowAttachment { get { return allowAttachment; } set { allowAttachment = value; } }

    public void Attach(Transform destinationTransform, Rigidbody attachToThisBody)
    {
        if (!allowAttachment)
            return;

        if (attachedToBody != null)
            Detach();

        Vector3 worldOffset = destinationTransform.rotation * localPositionOffset;
        Quaternion worldRotation = localRotationOffset * destinationTransform.rotation;

        body.velocity = Vector3.zero;
        body.angularVelocity = Vector3.zero;
        body.transform.position = destinationTransform.position + worldOffset;
        // TODO: adjust with a relative rotation as well...
        body.transform.rotation = worldRotation; 

        connectingJoint = body.gameObject.AddComponent<FixedJoint>();
        connectingJoint.connectedBody = attachToThisBody;
        connectingJoint.autoConfigureConnectedAnchor = false;
        connectingJoint.anchor = Vector3.zero;
        Vector3 localAttachPointOffset = attachToThisBody.transform.InverseTransformPoint(destinationTransform.position + worldOffset);
        connectingJoint.connectedAnchor = localAttachPointOffset;

        attachedToBody = attachToThisBody;

        if (Attached != null)
            Attached(this.gameObject, this);
    }

    public void Detach()
    {
        if (connectingJoint != null)
        {
            Destroy(connectingJoint);
            connectingJoint = null;
        }

        attachedToBody = null;

        if (Detached != null)
            Detached(this.gameObject, this);
    }

    void OnDisable()
    {
        if (IsAttached)
            Detach();
    }

    void OnDestroy()
    {
        if (IsAttached)
            Detach();
    }

    #endregion
}
