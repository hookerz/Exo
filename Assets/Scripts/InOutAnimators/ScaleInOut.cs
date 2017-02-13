using UnityEngine;

public class ScaleInOut : MonoBehaviour, IInOutAnimated
{
    public Transform objectToScale;
    public float activeScale = 1.0f;
    public float deactiveScale = 0.00001f;
    public float defaultInTime = 1.0f;
    public float defaultOutTime = 1.0f;

    public AnimationCurve inCurve;
    public AnimationCurve outCurve;

    public float DefaultInTime { get { return defaultInTime; } }
    public float DefaultOutTime { get { return defaultOutTime; } }

    void Awake()
    {
        if (objectToScale == null)
            objectToScale = this.transform;
    }

    public void Activated()
    {
        objectToScale.localScale = activeScale * Vector3.one;
    }

    public void ActivationProgress(float progress)
    {
        float pc = inCurve.Evaluate(progress);
        objectToScale.localScale = Mathf.LerpUnclamped(deactiveScale, activeScale, pc) * Vector3.one;
    }

    public void Deactivated()
    {
        objectToScale.localScale = deactiveScale * Vector3.one;
    }

    public void DeactivationProgress(float progress)
    {
        float pc = outCurve.Evaluate(progress);
        objectToScale.localScale = Mathf.LerpUnclamped(deactiveScale, activeScale, pc) * Vector3.one;
    }
    public void StartActivation() { }
    public void StartDeactivation() { }
}
