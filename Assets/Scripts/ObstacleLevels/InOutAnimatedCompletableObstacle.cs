using UnityEngine;
using UnityEngine.Events;

public enum ActivationAnimationOrdering
{
    ActiveDurningAnimation,
    ActiveAfterAnimation,
}

public class InOutAnimatedCompletableObstacle : MonoBehaviour, ICompletableObstacle
{
    public GameObject inOutAnimatorObject;
    public bool activateOnStart;
    public ActivationAnimationOrdering ordering = ActivationAnimationOrdering.ActiveAfterAnimation;

    private CompletableObstacleState obstacleState = CompletableObstacleState.NotActive;
    private IInOutAnimated inOutAnimated;

    public UnityEvent activeEvent;
    public UnityEvent notActiveEvent;
    public UnityEvent completeEvent;
    public UnityEvent failedEvent;

    #region ICompletableObstacle

    public GameObject GameObject { get { return this.gameObject; } }

    public CompletableObstacleState ObstacleState
    {
        get { return obstacleState; }
        protected set
        {
            if (obstacleState == value)
                return;

            CompletableObstacleState prev = obstacleState;
            obstacleState = value;
            if (StateChanged != null)
            {
                StateChanged(this, prev, value);

                switch (obstacleState)
                {
                    case CompletableObstacleState.Active:
                        activeEvent.Invoke();
                        break;
                    case CompletableObstacleState.NotActive:
                        notActiveEvent.Invoke();
                        break;
                    case CompletableObstacleState.Complete:
                        completeEvent.Invoke();
                        break;
                    case CompletableObstacleState.Failed:
                        failedEvent.Invoke();
                        break;
                }
            }
        }
    }

    public event ObstacleStateChangedDelegate StateChanged;

    public void Activate()
    {
        if (inOutAnimated == null)
        {
            ObstacleState = CompletableObstacleState.Active;
            return;
        }

        if (ordering == ActivationAnimationOrdering.ActiveDurningAnimation)
        {
            ObstacleState = CompletableObstacleState.Active;
            StartCoroutine(Utils.ActivateInOutAnimatedCoroutine(inOutAnimated));
        }
        else
        {
            StartCoroutine(Utils.ActivateInOutAnimatedCoroutine(inOutAnimated, () => 
            {
                ObstacleState = CompletableObstacleState.Complete;
            }));
        }
    }

    public void Deactivate()
    {
        if (inOutAnimated == null)
        {
            ObstacleState = CompletableObstacleState.NotActive;
            return;
        }

        if (ordering == ActivationAnimationOrdering.ActiveDurningAnimation)
        {
            StartCoroutine(Utils.DeactivateInOutAnimatedCoroutine(inOutAnimated, () =>
            {
                ObstacleState = CompletableObstacleState.NotActive;
            }));
        }
        else
        {
            ObstacleState = CompletableObstacleState.NotActive;
            StartCoroutine(Utils.DeactivateInOutAnimatedCoroutine(inOutAnimated));
        }
    }

    public void Reset()
    {
        Activate();
    }

    #endregion

    void Awake() 
    {
        if (inOutAnimatorObject == null)
            inOutAnimatorObject = this.gameObject;

        inOutAnimated = inOutAnimatorObject.GetComponent<IInOutAnimated>();
    }

    void Start()
    {
        if (activateOnStart)
        {
            Activate();
        }
    }
}
