using UnityEngine;

public class CompletableObstacleCollection : MonoBehaviour, ICompletableObstacleCollection
{
    public GameObject[] obstacleGameObjects;
    public bool activeOnStart = true;

    private ICompletableObstacle[] obstacles;
    private CompletableObstacleState state;
    private int completedCount = 0;
    private int failedCount = 0;

    void Awake()
    {
        obstacles = new ICompletableObstacle[obstacleGameObjects.Length];
        for (int i = 0; i < obstacleGameObjects.Length; i++)
        {
            var obstacle = obstacleGameObjects[i].GetComponent<ICompletableObstacle>();
            if (obstacle != null)
            {
                obstacle.StateChanged += Obstacle_StateChanged;
                obstacles[i] = obstacle;
            }
            else
            {
                Debug.LogError(obstacleGameObjects[i].name + " does not implement required interface ICompletableObstacle on: " + this.name);
            }
        }
    }

    private void RecomputeCounts()
    {
        completedCount = 0;
        failedCount = 0;
        for (int i = 0; i < obstacles.Length; i++)
        {
            if (obstacles[i].ObstacleState == CompletableObstacleState.Complete)
                completedCount++;
            if (obstacles[i].ObstacleState == CompletableObstacleState.Failed)
                failedCount++;
        }
    }

    private void Obstacle_StateChanged(ICompletableObstacle o, CompletableObstacleState oldState, CompletableObstacleState newState)
    {
        if (newState == CompletableObstacleState.Complete || newState == CompletableObstacleState.Failed)
        {
            // re-compute completedCount to be safe...
            RecomputeCounts();

            if(ProgressiveObstaclePercent != null)
            {
                float p = ((float)completedCount / (float)obstacles.Length);
                ProgressiveObstaclePercent(this, p);
            }

            if (completedCount == obstacles.Length)
            {
                // we are done!
                ObstacleState = CompletableObstacleState.Complete;
            }
            else if (CompletedCount + failedCount == obstacles.Length)
            {
                // one or more failed obstacles
                ObstacleState = CompletableObstacleState.Failed;
            }
            //else not done yet
        }
    }

    #region ICompletableObstacleCollection

    public int CollectionLength { get { return obstacles.Length; } }
    public int CompletedCount { get { return completedCount; } }
    public GameObject GameObject { get { return this.gameObject; } }

    public CompletableObstacleState ObstacleState
    {
        get { return state; }
        protected set
        {
            if (state == value)
                return;

            CompletableObstacleState old = state;
            state = value;
            if (StateChanged != null)
            {
                StateChanged(this, old, value);
            }
        }
    }

    public event ObstacleProgressDelegate ProgressiveObstacleCanceled;
    public event ObstacleProgressPercentDelegate ProgressiveObstaclePercent;
#pragma warning disable 0067
    public event ObstacleProgressDelegate ProgressiveObstacleStarted;
#pragma warning restore 0067
    public event ObstacleStateChangedDelegate StateChanged;

    public void Activate()
    {
        completedCount = 0;
        for (int i = 0; i < obstacles.Length; i++)
        {
            obstacles[i].Activate();
        }

        ObstacleState = CompletableObstacleState.Active;
    }

    public void Deactivate()
    {
        completedCount = 0;
        for (int i = 0; i < obstacles.Length; i++)
        {
            obstacles[i].Deactivate();
        }

        ObstacleState = CompletableObstacleState.NotActive;
    }

    public void Reset()
    {
        completedCount = 0;
        Activate();
        if (ProgressiveObstacleCanceled != null)
            ProgressiveObstacleCanceled(this);
    }

    #endregion

    void Start()
    {
        if (activeOnStart)
            Activate();
        else
            Deactivate();
    }
}
