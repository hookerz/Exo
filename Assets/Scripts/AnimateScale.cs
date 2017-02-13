using UnityEngine;
using System.Collections;

public class AnimateScale : MonoBehaviour
{
    public AnimationCurve curve;
    public float minTime = 5f, maxTime = 10f;
    public float animationDuration = .5f;
    private float nextAnimationTime;
    private bool animating = false;

    public bool updateX = true, updateY = true, updateZ = true;

    void Start()
    {
        nextAnimationTime = Time.time + (Random.Range(0, maxTime)) + Random.value;
    }

    void Update()
    {
        if (Time.time >= nextAnimationTime && !animating)
            StartCoroutine(Animate());
    }

    IEnumerator Animate()
    {
        animating = true;

        Vector3 scale = this.transform.localScale;
        while (Time.time < (nextAnimationTime + animationDuration))
        {
            float t = (Time.time - nextAnimationTime) / animationDuration;
            Vector3 newScale = scale * curve.Evaluate(t);
            if (!updateX)
                newScale.x = scale.x;
            if (!updateY)
                newScale.y = scale.y;
            if (!updateZ)
                newScale.z = scale.z;
            this.transform.localScale = newScale;

            yield return null;
        }
        this.transform.localScale = scale;

        nextAnimationTime = Time.time + (Random.Range(minTime, maxTime));
        animating = false;
    }
}
