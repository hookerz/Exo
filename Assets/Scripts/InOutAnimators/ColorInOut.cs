using UnityEngine;

public class ColorInOut : MonoBehaviour, IInOutAnimated
{
    public Renderer objRenderer;
    public bool makeMaterialCopy = true;
    public Color activeColor = new Color(1, 1, 1, 1);
    public Color deactiveColor = new Color(1, 1, 1, 0);
    public float defaultInTime = 1.0f;
    public float defaultOutTime = 1.0f;

    public AnimationCurve inCurve;
    public AnimationCurve outCurve;

    public float DefaultInTime { get { return defaultInTime; } }
    public float DefaultOutTime { get { return defaultOutTime; } }

    void Awake()
    {
        if (objRenderer == null)
            objRenderer = GetComponent<Renderer>();

        if(makeMaterialCopy)
        {
            Material material = new Material(objRenderer.material);
            objRenderer.material = material;
        }
    }

    public void Activated()
    {
        objRenderer.material.color = activeColor;
    }

    public void ActivationProgress(float progress)
    {
        float pc = inCurve.Evaluate(progress);
        objRenderer.material.color = Color.LerpUnclamped(deactiveColor, activeColor, pc);
    }

    public void Deactivated()
    {
        objRenderer.material.color = deactiveColor;
    }

    public void DeactivationProgress(float progress)
    {
        float pc = outCurve.Evaluate(progress);
        objRenderer.material.color = Color.LerpUnclamped(activeColor, deactiveColor, pc);
    }
    public void StartActivation() { }
    public void StartDeactivation() { }
}
