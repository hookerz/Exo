using UnityEngine;
using System.Collections;

public class RumbleCamera : MonoBehaviour 
{
    public Transform toRumble;
    public GvrViewer viewer;
    public float rumbleTime = 3.0f;
    public float maxDistance = 0.1f;
    public AnimationCurve maxDistanceCurve;
    public bool playOnEnabled = false;
        
    void Awake() 
    {
        if (toRumble == null)
            toRumble = this.transform;

        if (viewer == null)
            viewer = FindObjectOfType<GvrViewer>();
    }

    void OnEnable()
    {
        if (playOnEnabled)
            StartCoroutine(ShakeTransform(rumbleTime));
    }

    public void Shake(float time)
    {
        StartCoroutine(ShakeTransform(time));
    }

    IEnumerator ShakeTransform(float rumbleTime)
    {
        float startTime = Time.time;
        float maxDist = maxDistanceCurve.Evaluate(0) * maxDistance;
        Vector3 targetPosition = Random.onUnitSphere * maxDist;

        while (true)
        {
            float p = Mathf.Clamp01((Time.time - startTime) / rumbleTime);
            maxDist = maxDistanceCurve.Evaluate(p) * maxDistance;
            targetPosition = Random.onUnitSphere * maxDist;
            toRumble.localPosition = targetPosition;

            if (p == 1.0f) break;

            yield return null;
        }

        toRumble.localPosition = Vector3.zero;
    }
}
