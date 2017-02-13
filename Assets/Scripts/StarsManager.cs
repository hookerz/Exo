using UnityEngine;

public class StarsManager : MonoBehaviour
{
    public GameObject prefab;
    public bool createStars = false;
    public int nStars = 50;
    private Star[] stars;
    public float minDistance = 100, maxDistance = 135;
    public float minDistanceMinScale = 0.35f, minDistanceMaxScale = 0.5f;
    public float maxDistanceMinScale = 0.55f, maxDistanceMaxScale = 0.7f;
    public Color minColor = Color.white, maxColor = Color.white;
    public AnimationCurve animationCurve;
    public float minAnimationTime = 5f, maxAnimationTime = 10f;
    public float animationDuration = .25f;
    private Vector3 offset = new Vector3(-180, 0, 0);


    private class Star
    {
        public GameObject obj;
        public float nextAnimationTime;
        public bool animating = false;
        public Vector3 originalScale;
    }

    void Start()
    {
        CreateStars();
    }

    public void CreateStars()
    {
        if (createStars)
        {
            stars = new Star[nStars];

            for (int i = 0; i < nStars; i++)
            {
                float distance = Random.Range(minDistance, maxDistance);
                Vector3 position = new Vector3(0, 0, distance);
                Quaternion rotation = Quaternion.Euler(Random.Range(-180, 0), Random.Range(0, 360), 0);
                position = rotation * position;
                GameObject obj = Instantiate(prefab, position, Quaternion.identity) as GameObject;
                obj.transform.SetParent(this.transform);

                float t = Mathf.Clamp01(distance - minDistance / (maxDistance - minDistance));
                float minScale = minDistanceMinScale + (maxDistanceMinScale - minDistanceMinScale) * t;
                float maxScale = minDistanceMaxScale + (maxDistanceMaxScale - minDistanceMaxScale) * t;
                float scale = Random.Range(minScale, maxScale);
                obj.transform.localScale = new Vector3(scale, scale, scale);

                Vector3 toCamera = Camera.main.transform.position - obj.transform.position;
                obj.transform.rotation = Quaternion.LookRotation(toCamera, Vector3.up) * Quaternion.Euler(offset);

                Star star = new Star();
                star.obj = obj;
                star.nextAnimationTime = Time.time + (Random.Range(0, maxAnimationTime));
                star.originalScale = star.obj.transform.localScale;

                stars[i] = star;
            }
        }
        else {
            stars = new Star[transform.childCount];

            int i = 0;
            foreach (Transform child in this.transform)
            {
                Star star = new Star();
                star.obj = child.gameObject;
                star.nextAnimationTime = Time.time + (Random.Range(0, maxAnimationTime));
                star.originalScale = star.obj.transform.localScale;

                stars[i++] = star;
            }
        }
    }
    public void DeleteStars()
    {
        if (createStars)
            foreach (Transform child in this.transform)
                DestroyImmediate(child.gameObject);
        else
        {
            for (int i = 0; i < stars.Length; i++)
            {
                Star star = stars[i];
                star.obj.transform.localScale = star.originalScale;
            }
        }
    }

    void Update()
    {
        for (int i = 0; i < stars.Length; i++)
        {
            Star star = stars[i];

            if (!star.animating && Time.time >= star.nextAnimationTime)
                star.animating = true;

            if (star.animating)
            {
                if (Time.time < (star.nextAnimationTime + animationDuration))
                {
                    float t = (Time.time - star.nextAnimationTime) / animationDuration;
                    star.obj.transform.localScale = star.originalScale * animationCurve.Evaluate(t);
                }
                else
                {
                    star.obj.transform.localScale = star.originalScale;

                    star.nextAnimationTime = Time.time + (Random.Range(minAnimationTime, maxAnimationTime));
                    star.animating = false;
                }
            }
        }
    }
}
