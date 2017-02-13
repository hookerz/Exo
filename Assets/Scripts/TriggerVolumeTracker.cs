using UnityEngine;
using System.Collections.Generic;

public delegate void TriggerEventDelegate(GameObject sender, Collider c);

public interface ITriggerBroacaster
{
    event TriggerEventDelegate TriggerEnter;
    event TriggerEventDelegate TriggerStay;
    event TriggerEventDelegate TriggerExit;
}

public delegate void TriggerCollectorEventDelegate(GameObject sender, ITriggerVolumeTracker collector, Collider c);

public interface ITriggerVolumeTracker
{
    event TriggerCollectorEventDelegate ColliderEntered;
    event TriggerCollectorEventDelegate ColliderExited;
    event TriggerCollectorEventDelegate ColliderDestroyed; // a collider in the volume was destroyed

    // NOTE: this should really be an ready-only enumerator, but we don't want to take the allocation hit from foreach over an IEnumerator...
    List<Collider> CollidersInVolume { get; }
}

public class TriggerVolumeTracker : MonoBehaviour, ITriggerVolumeTracker, ITriggerBroacaster
{
    public LayerMask trackedLayerMask;
    public bool requireAttachedRigidBody = true;
    public bool checkTag = false;
    public string requiredTag = null;
    
    // uses pre-sized lists and hope this doesn't become a performance problem
    private List<Collider> collidersInVolume = new List<Collider>(10);
    private List<int> toRemove = new List<int>(10);

    #region ITriggerBroacaster

    public event TriggerEventDelegate TriggerEnter;
    public event TriggerEventDelegate TriggerStay;
    public event TriggerEventDelegate TriggerExit;

    #endregion

    #region ITriggerVolumeTracker

    public event TriggerCollectorEventDelegate ColliderEntered;
    public event TriggerCollectorEventDelegate ColliderExited;
    public event TriggerCollectorEventDelegate ColliderDestroyed; // a collider in the volume was destroyed

    // NOTE: this should really be an ready-only enumerator, but we don't want to take the allocation hit from foreach over an IEnumerator...
    public List<Collider> CollidersInVolume { get { return collidersInVolume; } }

    #endregion

    private bool InMask(Collider c)
    {
        if (requireAttachedRigidBody && c.attachedRigidbody == null)
            return false;

        int colliderLayerMask = 1 << c.gameObject.layer;
        bool inMask = (trackedLayerMask.value & colliderLayerMask) != 0;

        bool tagPassed = true;
        if (checkTag)
            tagPassed = c.gameObject.CompareTag(requiredTag);

        return inMask && tagPassed;
    }

    void CleanList()
    {
        // clean up dead objects from the list
        toRemove.Clear();
        for (int i = 0; i < collidersInVolume.Count; i++)
        {
            if (collidersInVolume[i] == null)
            {
                // can't remove while looping..
                toRemove.Add(i);
            }
        }

        for (int i = 0; i < toRemove.Count; i++)
        {
            Collider c = collidersInVolume[toRemove[i] - i];
            collidersInVolume.RemoveAt(toRemove[i] - i); // the index goes down for each one...

            if (ColliderDestroyed == null)
            {
                ColliderDestroyed(this.gameObject, this, c);
            }
        }

    }

    void OnTriggerEnter(Collider c)
    {
        if (TriggerEnter != null)
        {
            TriggerEnter(this.gameObject, c);
        }

        if (!InMask(c))
            return;

        if (!collidersInVolume.Contains(c))
        {
            collidersInVolume.Add(c);
            if (ColliderEntered != null)
            {
                ColliderEntered(this.gameObject, this, c);
            }
        }
    }

    void OnTriggerStay(Collider c)
    {
        if (TriggerStay != null)
        {
            TriggerStay(this.gameObject, c);
        }
    }

    void OnTriggerExit(Collider c)
    {
        if (TriggerExit != null)
        {
            TriggerExit(this.gameObject, c);
        }

        if (!InMask(c))
            return;

        if (collidersInVolume.Remove(c))
        {
            if (ColliderExited != null)
            {
                ColliderExited(this.gameObject, this, c);
            }
        }
    }
}
