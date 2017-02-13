using UnityEngine;

[ExecuteInEditMode]
public class LaserLine : MonoBehaviour
{
    public float length = 0.0f;
    public Transform target;
    public Transform origin;
    public Transform cap;
    public Transform effects;

    private Vector3[] linePoints;
    private LineRenderer lineRenderer;
    private ParticleSystem[] particles;

    public void OnTriggerParticles()
    {
        for (int i = particles.Length; --i >= 0;)
        {
            particles[i].Play();
        }
    }

    void Awake()
    {
        linePoints = new Vector3[2];

        linePoints[0] = origin.transform.position;
        linePoints[1] = target.transform.position;

        lineRenderer = GetComponent<LineRenderer>();

        particles = transform.parent.GetComponentsInChildren<ParticleSystem>();
    }

    void Update()
    {
        // Calculate termination point
        Vector3 end = Vector3.Lerp(origin.position, target.position, length);

        linePoints[1] = end;
        lineRenderer.SetPositions(linePoints);

        cap.position = end;
        effects.position = end;
    }
}
