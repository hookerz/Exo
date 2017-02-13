using UnityEngine;
using System.Collections;
using System;

public class EulerRotateInOut : MonoBehaviour, IInOutAnimated 
{
    public Transform transformToControl;

    public float inTime = 0.3f;
    public float outTime = 0.3f;

    public bool localRotation = true;

    public Vector3 startRotation = Vector3.zero;
    public Vector3 endRotation = Vector3.zero;

    public AnimationCurve inCurve;
    public AnimationCurve outCurve;

    void Awake()
    {
        if (transformToControl == null)
            transformToControl = this.transform;
    }

    public float DefaultInTime { get { return inTime; } }
    public float DefaultOutTime { get { return outTime; } }

    void SetRotation(Vector3 rotation)
    {
        if (localRotation)
        {
            transformToControl.localEulerAngles = rotation;
        }
        else
        {
            transformToControl.rotation = Quaternion.Euler(rotation);
        }
    }
    
    public void StartActivation()
    {
        SetRotation(startRotation);
    }

    public void ActivationProgress(float progress)
    {
        float t = outCurve.Evaluate(progress);
        SetRotation(Vector3.LerpUnclamped(startRotation, endRotation, t));
    }

    public void Activated()
    {
        SetRotation(endRotation);
    }

    public void StartDeactivation()
    {
        SetRotation(endRotation);
    }

    public void DeactivationProgress(float progress)
    {
        float t = outCurve.Evaluate(progress);
        SetRotation(Vector3.LerpUnclamped(endRotation, startRotation, t));
    }

    public void Deactivated()
    {
        SetRotation(startRotation);
    }
}
