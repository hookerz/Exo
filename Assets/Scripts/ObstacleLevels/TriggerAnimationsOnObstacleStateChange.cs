using UnityEngine;

public class TriggerAnimationsOnObstacleStateChange : MonoBehaviour 
{
    public GameObject obstacleGameObject;
    public Animator animator;

    public string activeTrigger;
    public string notActiveTrigger;
    public string completeTrigger;
    public string failedTrigger;

    private ICompletableObstacle obstacle;
    
    void Awake() 
    {
        if (obstacleGameObject == null)
            obstacleGameObject = this.gameObject;

        if (animator == null)
            animator = GetComponent<Animator>();

        obstacle = obstacleGameObject.GetComponent<ICompletableObstacle>();
    }

    void OnEnable()
    {
        obstacle.StateChanged += Obstacle_StateChanged;
    }

    void OnDisable()
    {
        obstacle.StateChanged -= Obstacle_StateChanged;
    }

    private void Obstacle_StateChanged(ICompletableObstacle o, CompletableObstacleState oldState, CompletableObstacleState newState)
    {
        switch (newState)
        {
            case CompletableObstacleState.Active:
                animator.SetTrigger(activeTrigger);
                break;
            case CompletableObstacleState.NotActive:
                animator.SetTrigger(notActiveTrigger);
                break;
            case CompletableObstacleState.Complete:
                animator.SetTrigger(completeTrigger);
                break;
            case CompletableObstacleState.Failed:
                animator.SetTrigger(failedTrigger);
                break;
        }
    }
}
