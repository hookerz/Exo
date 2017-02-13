using UnityEngine;

public class TextColorInOut : MonoBehaviour, IInOutAnimated
{
    public Color activeColor = new Color(1, 1, 1, 1);
    public Color deactiveColor = new Color(1, 1, 1, 0);
    public float defaultInTime = 1.0f;
    public float defaultOutTime = 1.0f;

    public AnimationCurve inCurve;
    public AnimationCurve outCurve;

    public TextMesh text;

    public float DefaultInTime { get { return defaultInTime; } }
    public float DefaultOutTime { get { return defaultOutTime; } }

    void Awake()
    {
        if (text == null)
            text = GetComponent<TextMesh>();
    }

    public void Activated()
    {
        text.color = activeColor;
    }

    public void ActivationProgress(float progress)
    {
        float pc = inCurve.Evaluate(progress);
        text.color = Color.LerpUnclamped(deactiveColor, activeColor, pc);
    }

    public void Deactivated()
    {
        text.color = deactiveColor;
    }

    public void DeactivationProgress(float progress)
    {
        float pc = outCurve.Evaluate(progress);
        text.color = Color.LerpUnclamped(activeColor, deactiveColor, pc);
    }
    public void StartActivation() {
    }
    public void StartDeactivation() { }
}
