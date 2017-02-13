using UnityEngine;
using System.Collections.Generic;

public enum SplineWalkerMode
{
    Once,
    Loop,
    PingPong
}

/// <summary>
/// Original code based on tutorial by Jasper Flick
/// http://catlikecoding.com/unity/tutorials/curves-and-splines/
/// </summary>
public class SplineWalker : MonoBehaviour
{
    public BezierSpline spline;

    public float duration;

    public SplineWalkerMode mode;

    private float progress;
    public AnimationCurve positionCurve;
    public AnimationCurve rotationCurve;
    private bool goingForward = true;

    public MonoBehaviour[] enableOnComplete;
    public bool disableOnComplete = true;

    public SortedList<float, Quaternion> rotations;
    public bool enableRotations;
    private int rotationIndex = 1;

    private Rigidbody body;
    public DroneThruster thruster;
    public float thurstProgressThreshold = 0.75f;

    public event WalkEndedDelegate WalkEnded;
    public delegate void WalkEndedDelegate();

    void Start()
    {
        body = GetComponent<Rigidbody>();
        body.isKinematic = true;

        rotations = spline.GetRotationPoints();
    }

    private void FixedUpdate()
    {
        if (goingForward)
        {
            progress += Time.fixedDeltaTime / duration;
            if (thruster != null)
                thruster.simulateThrust = progress < thurstProgressThreshold;
            if (progress > 1f)
            {
                if (mode == SplineWalkerMode.Once)
                {
                    progress = 1f;
                    for (int i = 0; i < enableOnComplete.Length; i++)
                        enableOnComplete[i].enabled = true;
                    if (disableOnComplete)
                    {
                        this.enabled = false;
                        body.velocity = Vector3.zero;
                        body.isKinematic = false;
                        if (WalkEnded != null)
                            WalkEnded();
                        return;
                    }
                }
                else if (mode == SplineWalkerMode.Loop)
                {
                    progress -= 1f;
                }
                else
                {
                    progress = 2f - progress;
                    goingForward = false;
                }
            }
        }
        else
        {
            progress -= Time.fixedDeltaTime / duration;
            if (progress < 0f)
            {
                progress = -progress;
                goingForward = true;
            }
        }

        Vector3 position = spline.GetPoint(positionCurve.Evaluate(progress));
        body.MovePosition(position);
        if (enableRotations)
        {
            if (rotations != null && rotations.Count >= 2)
            {
                while (rotationIndex < rotations.Count && progress > rotations.Keys[rotationIndex])
                {
                    rotationIndex++;
                }

                if (rotationIndex >= (rotations.Count))
                    transform.rotation = rotations.Values[rotations.Count - 1];
                else
                {
                    float startProgress = rotationCurve.Evaluate(rotations.Keys[rotationIndex - 1]);
                    float endProgress = rotationCurve.Evaluate(rotations.Keys[rotationIndex]);
                    Quaternion startRot = rotations.Values[rotationIndex - 1];
                    Quaternion endRot = rotations.Values[rotationIndex];
                    float t = 1;
                    if (endProgress != startProgress)
                        t = (rotationCurve.Evaluate(progress) - startProgress) / (endProgress - startProgress);
                    body.MoveRotation(Quaternion.Slerp(startRot, endRot, t));
                }
            }
        }
    }
}