using UnityEngine;

public class PlayAudioOnObstacleStateChange : MonoBehaviour
{
    public GameObject obstacleGameObject;
    public AudioSource source;

    public AudioClip activeClip;
    public AudioClip notActiveClip;
    public AudioClip completeClip;
    public AudioClip failedClip;

    private ICompletableObstacle obstacle;

    void Awake()
    {
        if (obstacleGameObject == null)
            obstacleGameObject = this.gameObject;

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
                if (activeClip != null)
                    source.PlayOneShot(activeClip);
                break;
            case CompletableObstacleState.NotActive:
                if (notActiveClip != null)
                    source.PlayOneShot(notActiveClip);
                break;
            case CompletableObstacleState.Complete:
                if (completeClip != null)
                    source.PlayOneShot(completeClip);
                break;
            case CompletableObstacleState.Failed:
                if (failedClip != null)
                    source.PlayOneShot(failedClip);
                break;
        }
    }
}
