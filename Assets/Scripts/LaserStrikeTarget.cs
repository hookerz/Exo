using UnityEngine;
using System.Collections;
using System;

public class LaserStrikeTarget : MonoBehaviour, ILaserStrikeTarget 
{
    public Transform target;
    public float strikeTime = 0.4f;
    public GameObject[] enableDurningStrike;
    public GameObject[] disableAfterStrike;
    public MeshRenderer hologramMeshRenderer;
    public bool testHologramAnimation = false;

    private CompletableObstacleState obstacleState = CompletableObstacleState.Active;

    #region ILaserStrikeTarget

    public event ObstacleProgressDelegate ProgressiveObstacleStarted;
    public event ObstacleProgressPercentDelegate ProgressiveObstaclePercent;
#pragma warning disable 0067
    public event ObstacleProgressDelegate ProgressiveObstacleCanceled;
#pragma warning restore 0067
    public event ObstacleStateChangedDelegate StateChanged;
    public event Action LaserStrikeStarted;
    public event Action LaserStrikeEnded;

    public float StrikeTime { get { return strikeTime; } }
    public Transform Target { get { return target; } }
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

    public void Activate()
    {
        ObstacleState = CompletableObstacleState.Active;
    }

    public void Deactivate()
    {
        ObstacleState = CompletableObstacleState.NotActive;
    }

    public void Reset()
    {
        Activate();
    }

    public void LaserStrikeWarmup(float length)
    {
        StartCoroutine(AnimateHologramMaterial(length));
    }

    public IEnumerator AnimateHologramMaterial(float length)
    {
        var m = Instantiate(hologramMeshRenderer.sharedMaterial);
        hologramMeshRenderer.material = m;
        float startTime = Time.time;
        while (true)
        {
            float p = Mathf.Clamp01((Time.time - startTime) / length);
            float v = Mathf.LerpUnclamped(1.0f, 0.5f, p);
            m.SetFloat("_InConeFactor", v);
            m.SetFloat("_ScanFreq2", Mathf.LerpUnclamped(-500, -1000, p));

            if (p == 1.0f)
                break;

            yield return null;
        }

        // out

        startTime = Time.time;
        while (true)
        {
            float p = Mathf.Clamp01((Time.time - startTime) / length);
            float v = Mathf.LerpUnclamped(0.5f, 1.0f, p);
            m.SetFloat("_InConeFactor", v);
            m.SetFloat("_ScanFreq2", Mathf.LerpUnclamped(-1000, -500, p));

            if (p == 1.0f)
                break;

            yield return null;
        }

        hologramMeshRenderer.gameObject.SetActive(false);
    }

    public void LaserStrikeStart()
    {
        if (LaserStrikeStarted != null)
            LaserStrikeStarted();
        Utils.SetActive(enableDurningStrike, true);

        if (ProgressiveObstacleStarted != null)
            ProgressiveObstacleStarted(this);
    }

    public void LaserStrikeProgress(float p)
    {
        if (ProgressiveObstaclePercent != null)
            ProgressiveObstaclePercent(this, p);
    }

    public void LaserStrikeEnd()
    {
        if (LaserStrikeEnded != null)
            LaserStrikeEnded();
        ObstacleState = CompletableObstacleState.Complete;
        Utils.SetActive(enableDurningStrike, false);
        Utils.SetActive(disableAfterStrike, false);
    }

    #endregion

    void Awake() 
    {

        Utils.SetActive(enableDurningStrike, false);
    }

#if UNITY_EDITOR
    void Update()
    {
        if (testHologramAnimation)
        {
            LaserStrikeWarmup(1.0f);
            testHologramAnimation = false;
        }
    }

#endif
}
