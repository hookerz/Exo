using UnityEngine;

public class BlockerDetector : MonoBehaviour, ICompletableObstacle
{
    public TriggerVolumeTracker triggerVolumeTracker;

    private CompletableObstacleState obstacleState = CompletableObstacleState.Active;

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
        this.ObstacleState = CompletableObstacleState.Active;
    }

    public void Deactivate()
    {
        this.ObstacleState = CompletableObstacleState.NotActive;
    }

    public void Reset()
    {
        Activate();
    }

    #endregion

    void Awake()
    {
        if (triggerVolumeTracker == null)
            triggerVolumeTracker = this.GetComponent<TriggerVolumeTracker>();
    }

    void OnEnable()
    {
        triggerVolumeTracker.ColliderExited += TriggerVolumeTracker_ColliderExited;
    }

    void OnDisable()
    {
        triggerVolumeTracker.ColliderExited -= TriggerVolumeTracker_ColliderExited;
    }

    private void TriggerVolumeTracker_ColliderExited(GameObject sender, ITriggerVolumeTracker collector, Collider c)
    {
        if (collector.CollidersInVolume.Count == 0)
        {
            // all colliders are clear!
            if (obstacleState == CompletableObstacleState.Active)
                ObstacleState = CompletableObstacleState.Complete;
        }
    }
}
