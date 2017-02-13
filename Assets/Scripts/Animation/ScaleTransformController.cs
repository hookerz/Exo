using UnityEngine;

public class ScaleTransformController : MonoBehaviour
{
    public Transform target;
    public float value = 1f;

    private float baseScale;

    void Start()
    {
        baseScale = target.localScale.x;
    }

    void Update()
    {
        float s = baseScale * value;

        target.localScale = new Vector3(s, s, s);
    }
}
