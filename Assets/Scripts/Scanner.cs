using UnityEngine;
using System.Collections;

public class Scanner : MonoBehaviour
{
    public TriggerVolumeTracker triggerVolumeTracker;
    public GvrAudioSource scannerAudioSource;
    public MeshRenderer scanConeMeshRenderer;
    public GameObject scannerInOutAnimatorObject;
    public float scannerAnimationTime = 0.3f;

    private IInOutAnimated scannerInOutAnimator;
    private IScannable currentScannable = null;
    private float previousAudioVolume = 1.0f;
    private float targetAudioVolume = 1.0f;
    private float audioTransitionTime = 0.2f;
    private float audioTransitionStartTime = 0.0f;

    private float scannerTargetTime = 0.0f;
    private float scannerTarget = 0.0f;
    private float scannerCurrent = 0.0f;

    private bool scannerAllowed = true;
    public PickupConeAnimator pickupCone;

    private SmoothDroneControl droneControl;

    void OnEnable()
    {
        triggerVolumeTracker.ColliderEntered += TriggerVolumeTracker_ColliderEntered;
        triggerVolumeTracker.ColliderExited += TriggerVolumeTracker_ColliderExited;
        triggerVolumeTracker.ColliderDestroyed += TriggerVolumeTracker_ColliderDestroyed;
    }

    void OnDisable()
    {
        triggerVolumeTracker.ColliderEntered -= TriggerVolumeTracker_ColliderEntered;
        triggerVolumeTracker.ColliderExited -= TriggerVolumeTracker_ColliderExited;
        triggerVolumeTracker.ColliderDestroyed -= TriggerVolumeTracker_ColliderDestroyed;

        scannerCurrent = 0.0f;
        scannerTarget = 0.0f;
        scannerInOutAnimator.DeactivationProgress(1.0f);
        scanConeMeshRenderer.enabled = false;
    }

    void Awake()
    {
        if (scannerAudioSource == null)
            scannerAudioSource = GetComponent<GvrAudioSource>();

        if (pickupCone == null)
            pickupCone = GameObject.FindObjectOfType<PickupConeAnimator>();

        if (scannerInOutAnimatorObject == null)
            scannerInOutAnimatorObject = this.gameObject;

        scannerInOutAnimator = scannerInOutAnimatorObject.GetComponent<IInOutAnimated>();
    }

    void Start()
    {
        if (droneControl == null)
            droneControl = GameObject.FindObjectOfType<SmoothDroneControl>();
    }

    private IScannable GetScannable(Collider c, int limit = 3)
    {
        Transform cur = c.gameObject.transform;

        int i = 0;
        while (cur != null && i <= limit)
        {
            var s = cur.GetComponent<IScannable>();
            if (s != null)
                return s;
            cur = cur.parent;
            i++;
        }
        return null;
    }

    private IScannable ScanableInTracker()
    {
        for (int i = 0; i < triggerVolumeTracker.CollidersInVolume.Count; i++)
        {
            IScannable s = GetScannable(triggerVolumeTracker.CollidersInVolume[i]);
            if (s != null)
                return s;
        }
        return null;
    }

    private void TriggerVolumeTracker_ColliderEntered(GameObject sender, ITriggerVolumeTracker collector, Collider c)
    {
        if (!scannerAllowed)
            return;

        var s = GetScannable(c);
        if (currentScannable == null && s != null && !s.Scaned)
        {
            currentScannable = s;
            StartCoroutine(ScanProcess());
        }
    }

    private void TriggerVolumeTracker_ColliderExited(GameObject sender, ITriggerVolumeTracker collector, Collider c)
    {
        if (currentScannable != null && currentScannable == GetScannable(c))
        {
            currentScannable.ScanCanceled();
            currentScannable = null;
        }

    }

    private void TriggerVolumeTracker_ColliderDestroyed(GameObject sender, ITriggerVolumeTracker collector, Collider c)
    {
        if (currentScannable != null && currentScannable != GetScannable(c))
        {
            currentScannable.ScanCanceled();
            currentScannable = null;
        }
    }

    IEnumerator ScanProcess()
    {
        if (currentScannable == null)
            yield break;

        currentScannable.ScanStart();

        if (scannerAudioSource.isPlaying)
            scannerAudioSource.Stop();

        float startTime = Time.time;
        if (currentScannable.ScanTimeCumulative)
        {
            startTime -= currentScannable.CurrentScanProgress * currentScannable.ScanTime;
            scannerAudioSource.time = currentScannable.CurrentScanProgress * currentScannable.ScanTime;
        }

        scannerAudioSource.Play();

        SetAudioVolumeTarget(1.0f, 0.1f);

        while (true)
        {
            if (currentScannable == null)
            {
                SetAudioVolumeTarget(0.0f, 0.2f);
                scanConeMeshRenderer.enabled = false;
                yield break;
            }

            float p = Mathf.Clamp01((Time.time - startTime) / currentScannable.ScanTime);
            currentScannable.ScanProgress(p);

            if (p == 1.0)
            {
                break;
            }

            yield return null;
        }

        if (currentScannable != null)
        {
            currentScannable.ScanComplete();
        }

        currentScannable = null;
    }

    private void SetAudioVolumeTarget(float target, float transitionTime)
    {
        previousAudioVolume = scannerAudioSource.volume;
        targetAudioVolume = target;
        audioTransitionStartTime = Time.time;
        audioTransitionTime = transitionTime;
    }

    public void SetScannerAllowed(bool allowed)
    {
        if (scannerAllowed == allowed)
            return;

        scannerAllowed = allowed;

        RefreshScanner();
    }
    public void RefreshScanner()
    {
        bool scannableInVolume = triggerVolumeTracker.CollidersInVolume.Count != 0;

        if (scannerAllowed)
        {
            if (scannableInVolume)
            {
                // TODO: check if any 
                scannerCurrent = 0.0f;
                var s = ScanableInTracker();
                if (currentScannable == null && s != null && !s.Scaned)
                {
                    Debug.Log("SetScannerAllowed::currentScannable = " + s);
                    currentScannable = s;
                    StartCoroutine(ScanProcess());
                }
            }
        }
        else
        {
            if (currentScannable != null)
            {
                currentScannable.ScanCanceled();
                currentScannable = null;
            }
        }
    }

    public void Update()
    {
        if (scannerAudioSource.volume != targetAudioVolume)
        {
            float p = Mathf.Clamp01((Time.time - audioTransitionStartTime) / audioTransitionTime);
            scannerAudioSource.volume = Mathf.SmoothStep(previousAudioVolume, targetAudioVolume, p);
        }

        bool scannableInVolume = false;
        if (scannerAllowed && !pickupCone.on && !droneControl.Stopped)
        {
            for (int i = 0; i < triggerVolumeTracker.CollidersInVolume.Count; i++)
            {
                Collider collider = triggerVolumeTracker.CollidersInVolume[i];
                if (collider.gameObject.activeInHierarchy && collider.enabled)
                {
                    scannableInVolume = true;
                    break;
                }
            }
        }

        float t = scannableInVolume ? 1.0f : 0.0f;
        if (t != scannerTarget
            || (scannableInVolume && !scanConeMeshRenderer.enabled))
        {
            scanConeMeshRenderer.enabled = true;
            scannerTarget = t;
            scannerTargetTime = Time.time;
        }

        if (scannerTarget != scannerCurrent)
        {
            scanConeMeshRenderer.enabled = true;
            float p = Mathf.Clamp01((Time.time - scannerTargetTime) / scannerAnimationTime);
            if (scannerTarget == 1.0f) // target is on
            {
                scannerInOutAnimator.ActivationProgress(p);
                scannerCurrent = p;
            }
            else
            {
                scannerInOutAnimator.DeactivationProgress(p);
                scannerCurrent = 1 - p;
                if (p == 1.0f)
                    scanConeMeshRenderer.enabled = false;
            }
        }
    }
}
