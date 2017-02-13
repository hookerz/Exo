using UnityEngine;

public delegate void AttachableEvent(GameObject sender, IAttachable attachable);

public interface IAttachable 
{
    Transform AttachPoint { get; }
    Rigidbody Body { get; }

    bool IsAttached { get; }
    bool AllowAttachment { get; set; }

    void Attach(Transform point, Rigidbody attachToThisBody);
    void Detach();

    event AttachableEvent Attached;
    event AttachableEvent Detached;
}

public delegate void AttachableDroppedDelegate(IAttachable attachable);

public interface IDropZone
{
    event AttachableDroppedDelegate AttachableDropped;
}
