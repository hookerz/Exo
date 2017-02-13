using UnityEngine;

public class AnimateGems : MonoBehaviour
{
    private class Gem
    {
        public Transform transform;
        public Vector3 startScale;
        public float nextAnimationTime;
        public bool animating = false;
    }

    public AnimationCurve curve;
    public float minTime = 5f, maxTime = 10f;
    public float animationDuration = .5f;
    private Vector3 offset = new Vector3(180, 0, 0);
    private Gem[] gems;

    void Start()
    {
        gems = new Gem[this.transform.childCount];
        for (int i = 0; i < this.transform.childCount; i++)
        {
            Gem gem = new Gem();
            gem.nextAnimationTime = Time.time + (Random.Range(0, maxTime)) + Random.value;
            gem.transform = this.transform.GetChild(i);
            Vector3 toCamera = Camera.main.transform.position - gem.transform.position;
            gem.transform.rotation = Quaternion.LookRotation(toCamera, Vector3.up) * Quaternion.Euler(offset);
            gems[i] = gem;
        }
    }

    void Update()
    {
        for (int i = 0; i < gems.Length; i++)
        {
            if (Time.time >= gems[i].nextAnimationTime && !gems[i].animating) {
                gems[i].startScale = gems[i].transform.localScale;
                gems[i].animating = true;
            }

            if (gems[i].animating)
            {
                Gem gem = gems[i];

                float t = (Time.time - gem.nextAnimationTime) / animationDuration;
                gem.transform.localScale = gem.startScale * curve.Evaluate(t);

                if (Time.time >= (gem.nextAnimationTime + animationDuration))
                {
                    gem.transform.localScale = gem.startScale;

                    gem.nextAnimationTime = Time.time + (Random.Range(minTime, maxTime));
                    gem.animating = false;
                }
            }
        }
    }
}
