using UnityEngine;
using UnityEngine.Events;

public interface IScannable : IProgressiveCompletableObstacle
{
    bool Scaned { get; }
    bool ScanTimeCumulative { get; }
    float ScanTime { get; }
    float CurrentScanProgress { get; }
    void ScanStart();
    void ScanProgress(float progress);
    void ScanComplete();
    void ScanCanceled();
}

[System.Serializable]
public class ScannableEvent : UnityEvent<IScannable>{ }

public class Scannable : MonoBehaviour, IScannable
{
    public float scanTime = 1.0f;
    public bool scanTimeCumulative = true;
    public GameObject[] disableOnComplete;
    public bool fakeComplete = false;

    private bool scaned = false;
    private float currentScanProgress = 0.0f;
    private CompletableObstacleState obstacleState = CompletableObstacleState.Active;
    
    

    #region IScannable

    public event ObstacleStateChangedDelegate StateChanged;
    public event ObstacleProgressDelegate ProgressiveObstacleStarted;
    public event ObstacleProgressPercentDelegate ProgressiveObstaclePercent;
    public event ObstacleProgressDelegate ProgressiveObstacleCanceled;

    public bool Scaned { get { return scaned; } }
    public float ScanTime { get { return scanTime; } }
    public GameObject GameObject { get { return gameObject; } }
    public bool ScanTimeCumulative { get { return scanTimeCumulative; } }
    public float CurrentScanProgress { get { return currentScanProgress; } }

    public CompletableObstacleState ObstacleState
    {
        get { return obstacleState; } 
        set
        {
            if (obstacleState == value)
                return;
            var oldState = obstacleState;
            obstacleState = value;
            if (StateChanged != null)
                StateChanged(this, oldState, obstacleState);
        }
    }

    public void Activate()
    {
        scaned = false;
        currentScanProgress = 0.0f;
        obstacleState = CompletableObstacleState.Active;
        SetActiveState(true);
    }

    public void Deactivate()
    {
        scaned = true;
        currentScanProgress = 0.0f;
        obstacleState = CompletableObstacleState.NotActive;
        SetActiveState(false);
    }

    public void Reset()
    {
        Activate();
    }

    public void ScanStart()
    {
        if(Debug.isDebugBuild)
            Debug.Log("[SCAN] Started on Object: " + this.gameObject.name);

        if (!scanTimeCumulative)
            currentScanProgress = 0.0f;

        if (ProgressiveObstacleStarted != null)
            ProgressiveObstacleStarted(this);
    }

    public void ScanProgress(float progress)
    {
        currentScanProgress = progress;

        if (ProgressiveObstaclePercent != null)
            ProgressiveObstaclePercent(this, progress);
    }

    public void ScanComplete()
    {
        if (Debug.isDebugBuild)
            Debug.Log("[SCAN] Complete on Object: " + this.gameObject.name);
        scaned = true;
        currentScanProgress = 100.0f;
        ObstacleState = CompletableObstacleState.Complete;
        SetActiveState(false);
    }

    public void ScanCanceled()
    {
        if (Debug.isDebugBuild)
            Debug.Log("[SCAN] Canceled on Object: " + this.gameObject.name);

        if (!scanTimeCumulative)
            currentScanProgress = 0.0f;

        if (ProgressiveObstacleCanceled != null)
            ProgressiveObstacleCanceled(this);
    }

    #endregion

    void SetActiveState(bool active)
    {
        if (disableOnComplete == null)
            return;

        for (int i = 0; i < disableOnComplete.Length; i++)
        {
            var go = disableOnComplete[i];
            if (go != null)
                go.SetActive(active);
        }
    }

#if UNITY_EDITOR
    void Update()
    {
        if (this.obstacleState == CompletableObstacleState.Active && fakeComplete == true)
        {
            ScanComplete();
        }
    }

#endif

}