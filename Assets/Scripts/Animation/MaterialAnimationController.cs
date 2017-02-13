using UnityEngine;

[ExecuteInEditMode]
public class MaterialAnimationController : MonoBehaviour
{
    public GameObject materialObject;

    public Material Material
    {
        set { material = value; }
    }

    public bool changeSharedMaterial;
    public string parameterName;

    public Color deactiveValue = Color.black;
    public Color activeValue = Color.white;
    public Color currentColor = Color.black;

    public float lerp = 0f;

    private Material material;

    void Start()
    {
        if (materialObject == null) materialObject = this.gameObject;

        if (changeSharedMaterial)
        {
            material = materialObject.GetComponent<Renderer>().sharedMaterial;
        }
        else
        {
            material = materialObject.GetComponent<Renderer>().material;
        }

        material.SetColor(parameterName, deactiveValue);
    }

    void Update()
    {
        Color c = Color.LerpUnclamped(deactiveValue, activeValue, lerp);

        currentColor = c;

        material.SetColor(parameterName, currentColor);
    }
}
