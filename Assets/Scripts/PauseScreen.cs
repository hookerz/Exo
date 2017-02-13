using UnityEngine;
using System.Collections;

public class PauseScreen : MonoBehaviour
{
    public bool startPaused;
    public float pauseTimeScale = 0.0000000001f;
    private bool isPaused, disconnectedScreenIsOn = false, disconnectTransitioning = false;
    public bool canPause = true;

    public ColorInOut textAnimator;
    public ColorInOut disconnectedAnimator;
    private int previousFrameRate;

    public GameObject[] objectsToDisableEarly;
    private bool[] objectsToDisableEarlyPreviousActive;
    public GameObject[] objectsToDisableLate;
    private bool[] objectsToDisableLatePreviousActive;

    private Animator animator;

    public Texture tutorialTexture;
    public MeshRenderer tutorialRenderer;
    private bool changedInitialTexture = false;

    public delegate void GamePausedHandler(bool paused);
    public event GamePausedHandler GamePaused;


    void Awake()
    {
        animator = GetComponent<Animator>();

        textAnimator.gameObject.SetActive(false);

        objectsToDisableEarlyPreviousActive = new bool[objectsToDisableEarly.Length];
        for (int i = 0; i < objectsToDisableEarly.Length; i++)
            objectsToDisableEarlyPreviousActive[i] = objectsToDisableEarly[i].activeSelf;

        objectsToDisableLatePreviousActive = new bool[objectsToDisableLate.Length];
        for (int i = 0; i < objectsToDisableLate.Length; i++)
            objectsToDisableLatePreviousActive[i] = objectsToDisableLate[i].activeSelf;

        Ending ending = GameObject.FindObjectOfType<Ending>();
        ending.StateChanged += Ending_StateChanged;

        if (startPaused)
            StartCoroutine(PauseGame());
    }

    private void Ending_StateChanged(ICompletableObstacle o, CompletableObstacleState oldState, CompletableObstacleState newState)
    {
        if (newState == CompletableObstacleState.Active)
            canPause = false;
    }

    IEnumerator PauseGame()
    {
        isPaused = true;

        for (int i = 0; i < objectsToDisableEarly.Length; i++)
            objectsToDisableEarly[i].SetActive(false);

        animator.SetTrigger("pause");

        previousFrameRate = Application.targetFrameRate;
        Application.targetFrameRate = 60;

        textAnimator.gameObject.SetActive(true);
        textAnimator.Deactivated();
        bool finished = false;
        StartCoroutine(Utils.ActivateInOutAnimatedCoroutine(textAnimator, () =>
        {
            Time.timeScale = pauseTimeScale;
            if (GamePaused != null)
                GamePaused(true);
            for (int i = 0; i < objectsToDisableLate.Length; i++)
                objectsToDisableLate[i].SetActive(false);
            finished = true;
        }));
        while (!finished)
            yield return null;

        while (true)
        {
            if (!disconnectedScreenIsOn && GvrController.State != GvrConnectionState.Connected)
            {
                disconnectedAnimator.gameObject.SetActive(true);
                disconnectedAnimator.Activated();
                textAnimator.gameObject.SetActive(false);
                disconnectedScreenIsOn = true;
            }
            else if (disconnectedScreenIsOn && GvrController.State == GvrConnectionState.Connected)
            {
                disconnectedAnimator.gameObject.SetActive(false);
                disconnectedAnimator.Deactivated();
                textAnimator.gameObject.SetActive(true);
                disconnectedScreenIsOn = false;
            }

            if (GvrController.State == GvrConnectionState.Connected && GvrController.AppButton)
            {
                break;
            }

            yield return null;
        }

        Time.timeScale = 1.0f;
        if (GamePaused != null)
            GamePaused(false);
        for (int i = 0; i < objectsToDisableLate.Length; i++)
            objectsToDisableLate[i].SetActive(objectsToDisableLatePreviousActive[i]);
        finished = false;
        StartCoroutine(Utils.DeactivateInOutAnimatedCoroutine(textAnimator, () =>
        {
            animator.SetTrigger("unpause");

            Application.targetFrameRate = previousFrameRate;
            textAnimator.gameObject.SetActive(false);

            for (int i = 0; i < objectsToDisableEarly.Length; i++)
                objectsToDisableEarly[i].SetActive(objectsToDisableEarlyPreviousActive[i]);
            finished = true;
        }));

        while (!finished)
            yield return null;

        if (!changedInitialTexture)
        {
            changedInitialTexture = true;
            tutorialRenderer.material.mainTexture = tutorialTexture;
        }

        isPaused = false;
    }

    void Update()
    {
        if (GvrController.State == GvrConnectionState.Connected)
        {
            if (GvrController.AppButtonUp && !isPaused && canPause)
            {
                StartCoroutine(PauseGame());
            }
        }

        if (!isPaused && canPause)
        {
            if (!disconnectedScreenIsOn && GvrController.State != GvrConnectionState.Connected)
            {
                if (!disconnectTransitioning)
                    StartCoroutine(ControllerIsDisconnected());
            }
            else if (disconnectedScreenIsOn && GvrController.State == GvrConnectionState.Connected)
            {
                if (!disconnectTransitioning)
                    StartCoroutine(ControllerIsConnected());
            }
        }
    }

    IEnumerator ControllerIsDisconnected()
    {
        disconnectTransitioning = true;

        disconnectedScreenIsOn = true;

        for (int i = 0; i < objectsToDisableEarly.Length; i++)
            objectsToDisableEarly[i].SetActive(false);
        animator.SetTrigger("pause");

        previousFrameRate = Application.targetFrameRate;
        Application.targetFrameRate = 60;

        disconnectedAnimator.gameObject.SetActive(true);
        disconnectedAnimator.Deactivated();
        bool finished = false;
        StartCoroutine(Utils.ActivateInOutAnimatedCoroutine(disconnectedAnimator, () =>
        {
            Time.timeScale = pauseTimeScale;
            if (GamePaused != null)
                GamePaused(true);
            for (int i = 0; i < objectsToDisableLate.Length; i++)
                objectsToDisableLate[i].SetActive(false);
            finished = true;
        }));
        while (!finished)
            yield return null;

        disconnectTransitioning = false;
    }

    IEnumerator ControllerIsConnected()
    {
        disconnectTransitioning = true;

        Time.timeScale = 1.0f;
        if (GamePaused != null)
            GamePaused(false);
        for (int i = 0; i < objectsToDisableLate.Length; i++)
            objectsToDisableLate[i].SetActive(objectsToDisableLatePreviousActive[i]);
        bool finished = false;
        StartCoroutine(Utils.DeactivateInOutAnimatedCoroutine(disconnectedAnimator, () =>
        {
            animator.SetTrigger("unpause");

            Application.targetFrameRate = previousFrameRate;
            disconnectedAnimator.gameObject.SetActive(false);

            for (int i = 0; i < objectsToDisableEarly.Length; i++)
                objectsToDisableEarly[i].SetActive(objectsToDisableEarlyPreviousActive[i]);
            finished = true;
        }));

        while (!finished)
            yield return null;

        disconnectedScreenIsOn = false;

        disconnectTransitioning = false;
    }
}
