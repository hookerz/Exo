using UnityEngine;

[ExecuteInEditMode]
public class MaterialAnimator : MonoBehaviour
{
    public Material material;
    public string parameterName;
    public float parameterValue;

    public void Awake()
    {
        material.SetFloat(parameterName, parameterValue);
    }

    public void Update()
    {
        material.SetFloat(parameterName, parameterValue);
    }
}
