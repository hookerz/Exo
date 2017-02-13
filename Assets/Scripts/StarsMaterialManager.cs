using UnityEngine;

public class StarsMaterialManager : MonoBehaviour
{
    public Material material;
    public int materialInstances = 5;
    private MaterialRecord[] materials;
    public AnimationCurve animationCurve;
    public float minAnimationTime = 5f, maxAnimationTime = 10f;
    public float animationDuration = .25f;

    private class MaterialRecord
    {
        public Material material;
        public float nextAnimationTime;
        public bool animating = false;
        public Color originalColor;
    }

    void Start()
    {
        materials = new MaterialRecord[materialInstances];
        for (int i = 0; i < materials.Length; i++) {
            MaterialRecord materialRecord = new MaterialRecord();
            materialRecord.material = new Material(material);
            materialRecord.nextAnimationTime = Time.time + (Random.Range(minAnimationTime, maxAnimationTime));
            materialRecord.originalColor = materialRecord.material.color;

            materials[i] = materialRecord;
        }

        foreach (Transform child in this.transform)
            child.gameObject.GetComponent<Renderer>().material = materials[Random.Range(0, materials.Length - 1)].material;
    }

    void Update()
    {
        for (int i = 0; i < materials.Length; i++)
        {
            MaterialRecord materialRecord = materials[i];

            if (!materialRecord.animating && Time.time >= materialRecord.nextAnimationTime)
                materialRecord.animating = true;

            if (materialRecord.animating)
            {
                if (Time.time < (materialRecord.nextAnimationTime + animationDuration))
                {
                    float t = (Time.time - materialRecord.nextAnimationTime) / animationDuration;
                    materialRecord.material.color = materialRecord.originalColor * animationCurve.Evaluate(t);
                }
                else
                {
                    materialRecord.material.color = materialRecord.originalColor;

                    materialRecord.nextAnimationTime = Time.time + (Random.Range(minAnimationTime, maxAnimationTime));
                    materialRecord.animating = false;
                }
            }
        }
    }
}
