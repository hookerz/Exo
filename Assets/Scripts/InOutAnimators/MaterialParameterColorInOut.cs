using UnityEngine;

public class MaterialParameterColorInOut : MonoBehaviour, IInOutAnimated
{
    public GameObject materialObject;
    public Material Material
    {
        set { material = value; }
    }
    private Material material;
    public bool changeSharedMaterial;
    public string parameterName;
    public Color deactiveValue = Color.black;
    public Color activeValue = Color.white;
    public float defaultInTime = 1.0f;
    public float defaultOutTime = 1.0f;

    public AnimationCurve inCurve;
    public AnimationCurve outCurve;

    public float DefaultInTime { get { return defaultInTime; } }
    public float DefaultOutTime { get { return defaultOutTime; } }

    public void Awake()
    {
        if (materialObject == null)
            materialObject = this.gameObject;

        if (changeSharedMaterial)
            material = materialObject.GetComponent<Renderer>().sharedMaterial;
        else
            material = materialObject.GetComponent<Renderer>().material;
        material.SetColor(parameterName, deactiveValue);
    }

    public void StartActivation()
    {
        material.SetColor(parameterName, deactiveValue);
    }

    public void ActivationProgress(float progress)
    {
        float t = inCurve.Evaluate(progress);
        material.SetColor(parameterName, Color.LerpUnclamped(deactiveValue, activeValue, t));
    }

    public void Activated()
    {
        material.SetColor(parameterName, activeValue);
    }

    public void StartDeactivation()
    {
        material.SetColor(parameterName, activeValue);
    }

    public void DeactivationProgress(float progress)
    {
        float t = outCurve.Evaluate(progress);
        material.SetColor(parameterName, Color.LerpUnclamped(activeValue, deactiveValue, t));
    }

    public void Deactivated()
    {
        material.SetColor(parameterName, deactiveValue);
    }
}
