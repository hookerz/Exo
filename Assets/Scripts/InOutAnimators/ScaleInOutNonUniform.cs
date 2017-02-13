using UnityEngine;

public class ScaleInOutNonUniform : MonoBehaviour, IInOutAnimated
{
    public Transform objectToScale;
    public Vector3 activeScale = Vector3.one;
    public Vector3 deactiveScale = Vector3.zero;
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
        objectToScale.localScale = activeScale;
    }

    public void ActivationProgress(float progress)
    {
        float pc = inCurve.Evaluate(progress);
        objectToScale.localScale = Vector3.LerpUnclamped(deactiveScale, activeScale, pc);
    }

    public void Deactivated()
    {
        objectToScale.localScale = deactiveScale;
    }

    public void DeactivationProgress(float progress)
    {
        float pc = outCurve.Evaluate(progress);
        objectToScale.localScale = Vector3.LerpUnclamped(activeScale, deactiveScale, pc);
    }

    public void StartActivation()
    {
        objectToScale.localScale = deactiveScale;
    }

    public void StartDeactivation()
    {
        objectToScale.localScale = activeScale;
    }
}
