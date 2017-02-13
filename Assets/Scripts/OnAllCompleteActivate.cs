using UnityEngine;
using System.Collections.Generic;

public class OnAllCompleteActivate : MonoBehaviour 
{
    public MonoBehaviour[] obstaclesToCompleteBehaviours;
    public GameObject obstacleToActivateGameObject;
    public bool test = false;
    
    private ICompletableObstacle[] obstaclesToComplete;
    private Dictionary<ICompletableObstacle, bool> completed = new Dictionary<ICompletableObstacle, bool>();
    private ICompletableObstacle obstacleToActive;

    void Awake() 
    {
        obstaclesToComplete = new ICompletableObstacle[obstaclesToCompleteBehaviours.Length];
        for (int i = 0; i < obstaclesToCompleteBehaviours.Length; i++)
        {
            obstaclesToComplete[i] = obstaclesToCompleteBehaviours[i] as ICompletableObstacle;
        }

        if (obstacleToActivateGameObject == null)
            obstacleToActivateGameObject = this.gameObject;

        obstacleToActive = obstacleToActivateGameObject.GetComponent<ICompletableObstacle>();
    }

    void OnEnable()
    {
        for (int i = 0; i < obstaclesToComplete.Length; i++)
            obstaclesToComplete[i].StateChanged += ObstacleToComplete_StateChanged;
    }

    void OnDisable()
    {
        for (int i = 0; i < obstaclesToComplete.Length; i++)
            obstaclesToComplete[i].StateChanged -= ObstacleToComplete_StateChanged;
    }

    private void ObstacleToComplete_StateChanged(ICompletableObstacle o, CompletableObstacleState oldState, CompletableObstacleState newState)
    {
        if (newState == CompletableObstacleState.Complete)
        {
            completed[o] = true;
            if (completed.Count == obstaclesToComplete.Length)
                obstacleToActive.Activate();
        }
    }

    #if UNITY_EDITOR

    void Update()
    {
        if (test)
        {
            obstacleToActive.Activate();
            test = false;
        }
    }

    #endif
}
