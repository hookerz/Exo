using UnityEngine;

public enum AnimationDirection
{
    Activating,
    Deactivating
}

public class TranslateInOut : MonoBehaviour, IInOutAnimated
{
    public Transform objectToTranslate;
    public Vector3 deactivePosition;
    public Vector3 activePosition;

    public AnimationCurve inCurve;
    public AnimationCurve outCurve;
    public float defaultInTime = 1.0f;
    public float defaultOutTime = 1.0f;

    public float DefaultInTime { get { return defaultInTime; } }
    public float DefaultOutTime { get { return defaultOutTime; } }

    void Awake()
    {
        if (objectToTranslate == null)
            objectToTranslate = this.transform;
    }

    public void Activated()
    {
        objectToTranslate.localPosition = activePosition;
    }

    public void ActivationProgress(float progress)
    {
        float pc = inCurve.Evaluate(progress);
        objectToTranslate.localPosition = Vector3.LerpUnclamped(deactivePosition, activePosition, pc);
    }

    public void Deactivated()
    {
        objectToTranslate.localPosition = deactivePosition;
    }

    public void DeactivationProgress(float progress)
    {
        float pc = outCurve.Evaluate(progress);
        objectToTranslate.localPosition = Vector3.LerpUnclamped(activePosition, deactivePosition, pc);
    }

    public void StartActivation()
    {
        objectToTranslate.localPosition = deactivePosition;
    }

    public void StartDeactivation()
    {
        objectToTranslate.localPosition = activePosition;
    }
}
