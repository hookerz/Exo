using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class InOutAnimatedLineLaser : MonoBehaviour, IInOutAnimated
{
    public LineRenderer lineRenderer;
    public Transform origin; // assume origin is this transform from now on
    public Transform target;
    public ParticleSystem fx;

    public float inTime = 0.2f;
    public float outTime = 0.3f;
    public float colorChangeTime = 0.3f;
    public float minThickness = 1.0f;
    public float maxThickness = 2.0f;
    public AnimationCurve scaleCurve;
    public AnimationCurve thicknessCurve;
    public AnimationCurve moveCurve;
    public bool on = false;
    public Color color = Color.red;

    public UnityEvent HitTarget;
    public UnityEvent NolongerHittingTarget;
    public UnityEvent ColorChangeComplete;

    private Vector3[] lineRendererPoints;
    private bool prevOn = false;
    private Vector3 startingPosition;
    private Quaternion startingOrientation;
    private Vector3 diff;
    private float dist;
    private Vector3 dir;
    private bool animating = false;

    void Awake()
    {
        if (origin == null)
            origin = this.transform;

        origin.gameObject.SetActive(false);
        startingPosition = origin.position;
        startingOrientation = origin.rotation;

        if (lineRenderer == null)
            lineRenderer = GetComponent<LineRenderer>();

        var mat = Instantiate(lineRenderer.material);
        mat.SetColor("_Color", color);
        lineRenderer.material = mat;

        if (fx != null)
            fx.startColor = color;

        lineRendererPoints = new Vector3[2];
    }

    void Update()
    {
#if UNITY_EDITOR
        if (on != prevOn)
        {
            if (on == true)
                Show();
            else
                Hide();
        }
        prevOn = on;
#endif
        if (on && !animating)
        {
            lineRendererPoints[0] = origin.position;
            lineRendererPoints[1] = target.position;
            lineRenderer.SetPositions(lineRendererPoints);
        }
    }
    
    public void SetColor(Color color, bool immediate, System.Action callback = null)
    {
        if (on == false)
        {
            this.color = color;
            Show(callback);
        }
        else
        {
            if (immediate)
            {
                this.color = color;
                lineRenderer.material.SetColor("_Color", color);

                if (fx != null)
                    fx.startColor = color;
            }
            else
            {
                StartCoroutine(AnimateColor(color, colorChangeTime, callback));
            }
        }
    }

    IEnumerator AnimateColor(Color color, float duration, System.Action callback = null)
    {
        float startTime = Time.time;

        Color startingColor = lineRenderer.material.color;

        while (true)
        {
            float t = Mathf.Clamp01((Time.time - startTime) / duration);
            Color c = Color.Lerp(startingColor, color, Mathf.SmoothStep(0, 1, t));

            lineRenderer.material.SetColor("_Color", c);

            if (fx != null)
                fx.startColor = c;

            if (t == 1.0f)
                break;

            yield return null;
        }

        if (callback != null)
            callback();

        ColorChangeComplete.Invoke();
    }

    public void Show(System.Action callback = null)
    {
        StartCoroutine(Utils.ActivateInOutAnimatedCoroutine(this, inTime, callback));
    }

    public void Hide(System.Action callback = null)
    {
        StartCoroutine(Utils.DeactivateInOutAnimatedCoroutine(this, outTime, callback));
    }

    #region IInOutAnimated

    public float DefaultInTime { get { return inTime; } }
    public float DefaultOutTime { get { return outTime; } }

    public void StartActivation()
    {
        animating = true;
        on = true;
        prevOn = true;
        origin.gameObject.SetActive(true);

        diff = target.position - origin.position;
        dist = diff.magnitude;
        dir = diff / dist;
        origin.rotation = Quaternion.LookRotation(dir);

        if (fx != null)
        {
            fx.gameObject.SetActive(true);
            fx.Play();
            fx.startColor = color;
        }

        lineRenderer.material.SetColor("_Color", color);
        lineRendererPoints[0] = origin.position;
        lineRendererPoints[1] = origin.position;
        lineRenderer.SetPositions(lineRendererPoints);
    }

    public void ActivationProgress(float t)
    {
        float scaleT = scaleCurve.Evaluate(t);
        float zscale = Mathf.LerpUnclamped(0.00001f, dist, scaleT);
        float xyscale = Mathf.LerpUnclamped(minThickness, maxThickness, thicknessCurve.Evaluate(t));
        origin.localScale = new Vector3(xyscale, xyscale, zscale);

        if (lineRenderer != null)
        {
            lineRendererPoints[1] = origin.position + (target.position - origin.position) * scaleT;
            lineRenderer.SetPositions(lineRendererPoints);
            lineRenderer.SetWidth(xyscale, xyscale);
        }
    }

    public void Activated()
    {
        animating = false;
        HitTarget.Invoke();
    }

    public void StartDeactivation()
    {
        animating = true;
        on = false;
        prevOn = false;
        NolongerHittingTarget.Invoke();

        lineRendererPoints[0] = origin.position;
        lineRendererPoints[1] = target.position;
        lineRenderer.SetPositions(lineRendererPoints);

        if (fx != null)
        {
            fx.Stop();
            fx.gameObject.SetActive(false);
        }
    }

    public void DeactivationProgress(float t)
    {
        float te = moveCurve.Evaluate(t);
        float xyscale = Mathf.LerpUnclamped(maxThickness, minThickness, thicknessCurve.Evaluate(t));
        origin.position = Vector3.Lerp(origin.position, target.position, te);
        origin.localScale = new Vector3(xyscale, xyscale, Vector3.Distance(origin.position, target.position));

        lineRendererPoints[0] = origin.position + (target.position - origin.position) * te;
        lineRenderer.SetPositions(lineRendererPoints);
        lineRenderer.SetWidth(xyscale, xyscale);
    }

    public void Deactivated()
    {
        animating = false;

        // reset position for the next one
        origin.position = startingPosition;
        origin.rotation = startingOrientation;
        origin.localScale = Vector3.one;
        origin.gameObject.SetActive(false);

        lineRendererPoints[0] = target.position;
        lineRendererPoints[1] = target.position;
        lineRenderer.SetPositions(lineRendererPoints);
    }

    #endregion
}
