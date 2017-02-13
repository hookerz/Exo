using UnityEngine;

public class ThrustMaterialUpdater : MonoBehaviour 
{
    public DroneThruster thruster;
    public Material mat;
    public float lerpFactor = 5.0f;
    private float current = 0.0f;
    public float powerFactor = 0.5f;

    void Awake() 
    {
        if (thruster == null)
            thruster = GetComponent<DroneThruster>();
    }
    
    void Update() 
    {
        current = Mathf.Lerp(current, Mathf.Pow(thruster.ThrustFactor, powerFactor), lerpFactor * Time.deltaTime);
        mat.SetFloat("_ThrusterPower", current);
    }
}
