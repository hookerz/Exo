using UnityEngine;

public class ManageObjectsFromCompletableObstacle : MonoBehaviour
{
    public GameObject completableObstacleGameObject;
    public GameObject[] enableOnActive;
    public GameObject[] disableOnActive;
    public GameObject[] enableOnNotActive;
    public GameObject[] disableOnNotActive;
    public GameObject[] enableOnComplete;
    public GameObject[] disableOnComplete;
    public GameObject[] enableOnFailed;
    public GameObject[] disableOnFailed;

    private ICompletableObstacle completableObstacle;

    void Awake()
    {
        if (completableObstacleGameObject == null)
            completableObstacleGameObject = this.gameObject;

        completableObstacle = completableObstacleGameObject.GetComponent<ICompletableObstacle>();
    }

    void OnEnable()
    {
        if (completableObstacle != null)
        {
            completableObstacle.StateChanged += CompletableObstacle_StateChanged;
            SetByState(completableObstacle.ObstacleState);
        }
    }

    void OnDisable()
    {
        if (completableObstacle != null)
        {
            completableObstacle.StateChanged -= CompletableObstacle_StateChanged;
        }
    }

    private void SetByState(CompletableObstacleState newState)
    {
        switch (newState)
        {
            case CompletableObstacleState.Active:
                Utils.SetActive(enableOnActive, true);
                Utils.SetActive(disableOnActive, false);
                break;
            case CompletableObstacleState.NotActive:
                Utils.SetActive(enableOnNotActive, true);
                Utils.SetActive(disableOnNotActive, false);
                break;
            case CompletableObstacleState.Complete:
                Utils.SetActive(enableOnComplete, true);
                Utils.SetActive(disableOnComplete, false);
                break;
            case CompletableObstacleState.Failed:
                Utils.SetActive(enableOnFailed, true);
                Utils.SetActive(disableOnFailed, false);
                break;
        }
    }

    private void CompletableObstacle_StateChanged(ICompletableObstacle o, CompletableObstacleState oldState, CompletableObstacleState newState)
    {
        SetByState(newState);
    }
}
