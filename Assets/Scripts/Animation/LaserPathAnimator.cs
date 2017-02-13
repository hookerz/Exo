using UnityEngine;

public class LaserPathAnimator : MonoBehaviour
{
    public Transform origin;
    public Transform target;
    public LineRenderer lineRenderer;

    public ParticleSystem _particleSystem;

    private Animator targetAnimator;
    private Animator lineAnimator;
    private Animator capAnimator;
    private bool hasConcentrateTriggered;
    private bool hasDissipateTriggered;

    public float laserWarmupTime = 2f;

    void Awake()
    {
        targetAnimator = GetComponent<Animator>();

        lineAnimator = lineRenderer.GetComponent<Animator>();
        capAnimator = lineRenderer.GetComponent<LaserLine>().cap.GetComponent<Animator>();
    }

    void Start()
    {
        hasConcentrateTriggered = false;

        StartCoroutine(Utils.DelayedAction(() =>
        {
            targetAnimator.SetTrigger("show");
            lineAnimator.SetTrigger("show");
            capAnimator.SetTrigger("show");
        },
        laserWarmupTime));
    }

    public void OnConcentrateTriggered()
    {
        if (!hasConcentrateTriggered)
        {

            hasConcentrateTriggered = true;
            lineAnimator.SetTrigger("concentrate");
            capAnimator.SetTrigger("concentrate");

        }
    }

    public void OnDissipateTriggered()
    {
        if (!hasDissipateTriggered)
        {

            hasDissipateTriggered = true;

            lineAnimator.SetTrigger("dissipate");
            capAnimator.SetTrigger("dissipate");

            _particleSystem.Stop();
        }
    }
}
