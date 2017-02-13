using UnityEngine;
using System;
using System.Collections;

public interface ILaserStrikeTarget : IProgressiveCompletableObstacle
{
    Transform Target { get; }
    float StrikeTime { get; }

    void LaserStrikeWarmup(float time);
    void LaserStrikeStart();
    void LaserStrikeProgress(float p);
    void LaserStrikeEnd();
    

    event Action LaserStrikeStarted;
    event Action LaserStrikeEnded;
}

public class LaserStrike : MonoBehaviour 
{
    public Transform laserOrigin;
    public InOutAnimatedLineLaser laser;
    public float laserWarmupTime = 2f;

    public float startLineStartWidth = 2, startLineEndWidth = 4;
    public float endLineStartWidth = 2, endLineEndWidth = 4;
    public AnimationCurve lineWidthCurve;
    public LineRenderer lineRenderer;

    public float startDecalScale = 0.25f, endDecalScale = 1.15f;
    public Transform laserBall;
    public Transform startDecal;
    public AnimationCurve decalCurve;

    public GameObject blackout;
    private ColorInOut blackoutColorInOut;
    public float blackoutFadeoutTime = 0.5f;

    public GameObject[] scannableGameObjects;
    public string hologramGeometryName = "Hologram Geo";
    public Material hologramMaterial;

    public RumbleCamera rumbleCamera;

    public PauseScreen pauseScreen;

    // TODO: move this out to a separate script...
    public GvrAudioSource playerAudioSource, mothershipAudioSource;
    
    void Awake() 
    {
        if (laser == null)
            laser = GetComponent<InOutAnimatedLineLaser>();
        blackoutColorInOut = blackout.GetComponent<ColorInOut>();
        startDecal.gameObject.SetActive(false);
        blackout.SetActive(false);
        if (pauseScreen == null)
            pauseScreen = GameObject.FindObjectOfType<PauseScreen>();
    }

    private void ManageEvents(bool subscribe)
    {
        for (int i = 0; i < scannableGameObjects.Length; i++)
        {
            if (scannableGameObjects[i] != null)
            {
                var s = scannableGameObjects[i].GetComponent<IScannable>();
                if (s != null)
                {
                    if (subscribe)
                        s.StateChanged += Scannable_StateChanged;
                    else
                        s.StateChanged -= Scannable_StateChanged;
                }
            }
        }
    }

    private void Scannable_StateChanged(ICompletableObstacle o, CompletableObstacleState oldState, CompletableObstacleState newState)
    {
        if (newState == CompletableObstacleState.Complete)
        {
            var t = o.GameObject.GetComponent<ILaserStrikeTarget>();
            if (t == null)
            {
                Debug.LogError("GameObject does not have a component that implements ILaserStrikeTarget: " + o.GameObject.name);
                return;
            }

            laser.target = t.Target != null ? t.Target : o.GameObject.transform;
            StartCoroutine(LaserStrikeAnimation(t));

            if (playerAudioSource != null)
                playerAudioSource.Play();

            if (mothershipAudioSource != null)
                mothershipAudioSource.Play();
        }
    }

    private IEnumerator LaserStrikeAnimation(ILaserStrikeTarget t)
    {
        pauseScreen.canPause = false;
        t.LaserStrikeWarmup(laserWarmupTime);
        yield return new WaitForSeconds(laserWarmupTime);
        if(lineRenderer != null)
            laser.Show();
        
        yield return new WaitForSeconds(laser.inTime);

        rumbleCamera.enabled = true;
        t.LaserStrikeStart();

        float startTime = Time.time;
        blackout.SetActive(true);
        blackoutColorInOut.StartActivation();

        if(lineRenderer != null)
            laserBall.gameObject.SetActive(true);

        while (true)
        {
            float p = Mathf.Clamp01((Time.time - startTime) / t.StrikeTime);
            t.LaserStrikeProgress(p);

            if(p > 0.05f)
                startDecal.gameObject.SetActive(true);

            if (lineRenderer != null) { 
                float animationP = lineWidthCurve.Evaluate(p);
                float startWidth = animationP * (startLineEndWidth - startLineStartWidth) + startLineStartWidth;
                float endWidth = animationP * (endLineEndWidth - endLineStartWidth) + endLineStartWidth;
                lineRenderer.SetWidth(startWidth, endWidth);


                Vector3 dir = (t.Target.position - this.transform.position).normalized;
                RaycastHit info;
                if (Physics.Raycast(this.transform.position, dir, out info) == true)
                {
                    laserBall.position = info.point;
                }
                else
                {
                    laserBall.position = t.Target.position;
                }
                laserBall.localScale = Mathf.LerpUnclamped(startDecalScale, endDecalScale, decalCurve.Evaluate(p)) * Vector3.one;
            }


            blackoutColorInOut.ActivationProgress(blackoutColorInOut.inCurve.Evaluate(p));

            if (p == 1.0f)
                break;

            yield return null;
        }
        blackoutColorInOut.Activated();
        t.LaserStrikeEnd();
        laser.Hide();
        rumbleCamera.enabled = false;
        laserBall.gameObject.SetActive(false);
        startDecal.gameObject.SetActive(false);

        startTime = Time.time;
        blackoutColorInOut.StartDeactivation();
        while (true)
        {
            float p = Mathf.Clamp01((Time.time - startTime) / blackoutFadeoutTime);
            blackoutColorInOut.DeactivationProgress(blackoutColorInOut.outCurve.Evaluate(p));

            if (p == 1.0f)
                break;

            yield return null;
        }
        blackoutColorInOut.Deactivated();
        blackout.SetActive(false);
        pauseScreen.canPause = true;
    }

    void OnEnable()
    {
        ManageEvents(true);
    }

    void OnDisable()
    {
        ManageEvents(false);
    }
}
