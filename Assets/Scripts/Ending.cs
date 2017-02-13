using UnityEngine;

public class Ending : MonoBehaviour, ICompletableObstacle
{
    public InOutLightIntensity[] inOutLightIntensities;

    public GameObject centerStructure;
    public Material skyboxMaterial;
    private Color skyboxStartColor, skyboxEndColor;
    public ParticleSystem dustFx;
    public MaterialParameterColorInOut dustColorInOutAnimation;

    public EndScreen endScreen;
    public float endScreenDelay = 5;

    public GameObject warpObj;
    public float warpFxDelay = 3;
    public GameObject[] objectsToDisable;
    public float objectsToDisableDelay = 1;

    public GameObject GameObject
    {
        get
        {
            return this.gameObject;
        }
    }

    public CompletableObstacleState ObstacleState
    {
        get
        {
            return obstacleState;
        }
        protected set
        {
            if (obstacleState == value)
                return;

            CompletableObstacleState prev = obstacleState;
            obstacleState = value;
            if (StateChanged != null)
                StateChanged(this, prev, value);
        }
    }
    private CompletableObstacleState obstacleState = CompletableObstacleState.NotActive;

    public event ObstacleStateChangedDelegate StateChanged;

    void Start()
    {
        skyboxStartColor = skyboxMaterial.GetColor("_StartColor");
        skyboxEndColor = skyboxMaterial.GetColor("_EndColor");

        centerStructure.GetComponent<ICompletableObstacle>().StateChanged += CenterStructure_StateChanged;

        GameObject[] cylinders = warpObj.GetComponent<WarpFx>().cylinders;
        for (int i = 0; i < cylinders.Length; i++)
            cylinders[i].SetActive(false);
    }

    private void CenterStructure_StateChanged(ICompletableObstacle o, CompletableObstacleState oldState, CompletableObstacleState newState)
    {
        if (newState == CompletableObstacleState.Complete)
        {
            skyboxMaterial.SetColor("_StartColor", Color.black);
            skyboxMaterial.SetColor("_EndColor", Color.black);

            GameObject[] cylinders = warpObj.GetComponent<WarpFx>().cylinders;
            for (int i = 0; i < cylinders.Length; i++)
                cylinders[i].SetActive(true);
            for (int i = 0; i < inOutLightIntensities.Length; i++)
                StartCoroutine(Utils.DeactivateInOutAnimatedCoroutine(inOutLightIntensities[i]));
            StartCoroutine(Utils.DelayedAction(() =>
            {
                for (int i = 0; i < objectsToDisable.Length; i++)
                    objectsToDisable[i].SetActive(false);
            }, objectsToDisableDelay));
        }
    }

    void OnDestroy()
    {
        skyboxMaterial.SetColor("_StartColor", skyboxStartColor);
        skyboxMaterial.SetColor("_EndColor", skyboxEndColor);
    }

    public void Activate()
    {
        Debug.Log("Ending started!");
        ObstacleState = CompletableObstacleState.Active;
        centerStructure.GetComponent<ICompletableObstacle>().Activate();
        dustFx.Stop();
        dustColorInOutAnimation.Material = dustFx.GetComponent<Renderer>().material;
        StartCoroutine(Utils.DeactivateInOutAnimatedCoroutine(dustColorInOutAnimation));
        StartCoroutine(Utils.DelayedAction(() =>
        {
            dustFx.gameObject.SetActive(false);
        }, Mathf.Max(0, dustColorInOutAnimation.defaultOutTime - 1)));
        StartCoroutine(Utils.DelayedAction(() =>
        {
            warpObj.SetActive(true);
        }, warpFxDelay));
    }

    public void Deactivate()
    {
        ObstacleState = CompletableObstacleState.NotActive;
    }

    public void Reset()
    {
        Activate();
    }
}
