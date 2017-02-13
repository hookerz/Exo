using UnityEngine;
using System.Collections;

public class ActivationSwitch : MonoBehaviour, ICompletableObstacle
{
    public GameObject inOutAnimatedGameObject;
    public TriggerVolumeTracker triggerVolumeTracker;
    public bool activateOnStart = false;
    
    public GameObject defaultGeometry;
    public GameObject animatedGeometry;
    public GameObject outGeometry;
    public float animationTime = 2.75f;
    public ParallelInOutAnimator fxInOutAnimated;

    private CompletableObstacleState obstacleState = CompletableObstacleState.NotActive;
    private Animator _animator;

    public Animator towerAnimator;

    public bool testComplete = false;

    #region ICompletableObstacle

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

    public event ObstacleStateChangedDelegate StateChanged;

    public void Activate()
    {
        _animator.SetTrigger("activate");
        StartCoroutine(Utils.DelayedAction(() =>
        {
            ObstacleState = CompletableObstacleState.Active;
        }, 3.1f));
    }

    public void Deactivate()
    {
        ObstacleState = CompletableObstacleState.NotActive;
        _animator.SetTrigger("deactivate");
        StartCoroutine(Utils.DelayedAction(() =>
        {
            ObstacleState = CompletableObstacleState.NotActive;
        }, 3.1f));
    }

    public void Reset()
    {
        Activate();
    }

    #endregion

    void Awake()
    {
        if (inOutAnimatedGameObject == null)
            inOutAnimatedGameObject = this.gameObject;

        _animator = GetComponent<Animator>();

    }

    void Start()
    {
        if (activateOnStart)
        {
            Activate();
        }
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
        if (obstacleState == CompletableObstacleState.Active)
        {
            Complete();
        }
    }

    private void Complete()
    {
        ObstacleState = CompletableObstacleState.Complete;
        _animator.SetTrigger("deactivate");
        StartCoroutine(PlayFx());
    }

    IEnumerator PlayFx()
    {
        defaultGeometry.SetActive(false);
        animatedGeometry.SetActive(true);
        yield return new WaitForSeconds(animationTime);
        animatedGeometry.SetActive(false);
        outGeometry.SetActive(true);
    }

#if UNITY_EDITOR
    void Update()
    {
        if (testComplete)
        {
            Complete();
            testComplete = false;
        }
    }
#endif 

}
