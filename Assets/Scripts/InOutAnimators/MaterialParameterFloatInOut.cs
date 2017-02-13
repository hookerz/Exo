using UnityEngine;

public class MaterialParameterFloatInOut : MonoBehaviour, IInOutAnimated
{
    public Material material;
    public MeshRenderer meshRenderer;

    public bool instanceMaterialOnFirstUse = false;
    
    public string parameterName;
    public float deactiveValue = 0.0f;
    public float activeValue = 1.0f;
    public float defaultInTime = 1.0f;
    public float defaultOutTime = 1.0f;

    public AnimationCurve inCurve;
    public AnimationCurve outCurve;

    public float DefaultInTime { get { return defaultInTime; } }
    public float DefaultOutTime { get { return defaultOutTime; } }

    private bool instancedMaterial = false;

    private Material Material
    {
        get
        {
            if (instanceMaterialOnFirstUse && !instancedMaterial && meshRenderer != null)
            {
                material = Instantiate(material);
                meshRenderer.material = material;
                instancedMaterial = true;
            }

            return material;
        }
    }

    public void Awake()
    {
        if (meshRenderer == null)
            meshRenderer = GetComponent<MeshRenderer>();

        if (material == null)
            material = meshRenderer.sharedMaterial;

        if (!instanceMaterialOnFirstUse)
            material.SetFloat(parameterName, deactiveValue);
    }

    public void StartActivation()
    {
        Material.SetFloat(parameterName, deactiveValue);
    }

    public void ActivationProgress(float progress)
    {
        float t = inCurve.Evaluate(progress);
        Material.SetFloat(parameterName, Mathf.LerpUnclamped(deactiveValue, activeValue, t));
    }

    public void Activated()
    {
        Material.SetFloat(parameterName, activeValue);
    }

    public void StartDeactivation()
    {
        Material.SetFloat(parameterName, activeValue);
    }

    public void DeactivationProgress(float progress)
    {
        float t = outCurve.Evaluate(progress);
        Material.SetFloat(parameterName, Mathf.LerpUnclamped(activeValue, deactiveValue, t));
    }

    public void Deactivated()
    {
        Material.SetFloat(parameterName, deactiveValue);
    }
}
