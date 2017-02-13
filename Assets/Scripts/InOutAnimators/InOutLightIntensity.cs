using UnityEngine;

public class InOutLightIntensity : MonoBehaviour, IInOutAnimated
{
    public Light lightToControl;
    public bool manageEnabledState = false;

    public float activeValue = 1.0f;
    public float deactiveValue = 0.0f;

    public float defaultInTime = 1.0f;
    public float defaultOutTime = 1.0f;

    public AnimationCurve inCurve;
    public AnimationCurve outCurve;

    public float DefaultInTime { get { return defaultInTime; } }
    public float DefaultOutTime { get { return defaultOutTime; } }

    void Awake()
    {
        if (lightToControl == null)
            lightToControl = this.GetComponent<Light>();
    }

    public void StartActivation()
    {
        if (manageEnabledState && lightToControl.enabled == false)
            lightToControl.enabled = true;

        lightToControl.intensity = deactiveValue;
    }

    public void ActivationProgress(float progress)
    {
        float pc = inCurve.Evaluate(progress);
        lightToControl.intensity = Mathf.LerpUnclamped(deactiveValue, activeValue, pc);
    }

    public void Activated()
    {
        lightToControl.intensity = activeValue;
    }

    public void StartDeactivation()
    {
        if (manageEnabledState && lightToControl.enabled == false)
            lightToControl.enabled = true;

        lightToControl.intensity = activeValue;
    }

    public void DeactivationProgress(float progress)
    {
        float pc = outCurve.Evaluate(progress);
        lightToControl.intensity = Mathf.LerpUnclamped(activeValue, deactiveValue, pc);
    }

    public void Deactivated()
    {
        lightToControl.intensity = deactiveValue;

        if (manageEnabledState && lightToControl.enabled == true)
            lightToControl.enabled = false;
    }
}
