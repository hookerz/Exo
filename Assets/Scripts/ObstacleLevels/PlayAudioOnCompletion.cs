using UnityEngine;

public class PlayAudioOnCompletion : MonoBehaviour
{
    public GameObject completeableGameObject;
    public GvrAudioSource source;
    public AudioClip completeClip;
    public AudioClip failedClip;
    public float delayInSeconds = 0.0f;
    
    private ICompletableObstacle obstacle;
    
    void Awake()
    {
        if (source == null)
            source = GetComponent<GvrAudioSource>();

        if (completeableGameObject == null)
            completeableGameObject = this.gameObject;

        obstacle = completeableGameObject.GetComponent<ICompletableObstacle>();

        if (obstacle == null)
        {
            Debug.LogError(completeableGameObject.name + " does not implement the interface ICompletableObstacle on: " + this.gameObject.name);
        }
        else
        {
            obstacle.StateChanged += Obstacle_StateChanged;
        }
    }
    
    private void PlayClip(AudioClip clip)
    {
        if (delayInSeconds > 0.0f)
        {
            StartCoroutine(Utils.DelayedAction(() => { source.PlayOneShot(clip); }, delayInSeconds));
        }
        else
        {
            source.PlayOneShot(clip);
        }
    }

    private void Obstacle_StateChanged(ICompletableObstacle o, CompletableObstacleState oldState, CompletableObstacleState newState)
    {
        switch (newState)
        {
            case CompletableObstacleState.Complete:
                if (completeClip != null)
                    PlayClip(completeClip);
                break;
            case CompletableObstacleState.Failed:
                if (failedClip != null)
                    PlayClip(failedClip);
                break;
        }
    }
}
